using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToItemMap
{
    public Transform InventorySlotTransform;
    public Transform ItemSlotTransform;


    public InventoryToItemMap(Transform InventorySlotTransform, Transform ItemSlotTransform)
    {
        this.InventorySlotTransform = InventorySlotTransform;
        this.ItemSlotTransform = ItemSlotTransform;
    }
}

public class Inventory : MonoBehaviour {
    public GameObject inventorySlotPrefab;
    GameObject slotsParent;

    List<Item> Items = new List<Item>();

    GameObject currentItem;
    BoxCollider2D inventoryCollider;

    public int height;
    public int width;
    public float minimumDistance = 0.9f;

    List<InventorySlot> inventorySlots;
    //public bool[,] inventoryShape; //It should be like this
    private void OnEnable()
    {
        inventoryCollider = GetComponent<BoxCollider2D>();
        Item.DragBegun += DragBegunEventHandler;
        Item.DragEnded += DragEndedEventHandler;
    }

    private void OnDisable()
    {
        inventoryCollider = GetComponent<BoxCollider2D>();
        Item.DragBegun -= DragBegunEventHandler;
        Item.DragEnded -= DragEndedEventHandler;
    }

    // Use this for initialization
    void Start () {
        inventoryCollider.size = new Vector2(width, height);
        inventorySlots = new List<InventorySlot>();
        SetupSlots(CreateShape(width, height));
        //TODO: insert box collider with inventory size
    }

	// Update is called once per frame
	void Update () {
        if (currentItem)
        {
            HandleSlotCollision();
        }
    }

    protected void SetupSlots(bool[,] shape)
    {
        CreateSlotsParent();
        InstantiateSlots(shape);
    }

    protected void InstantiateSlots(bool[,] shape)
    {
        int slotCount = 0;
        int initialX = shape.GetLength(0) / 2;
        int initialY = shape.GetLength(1) / 2;
        for (int j = shape.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0 ; i < shape.GetLength(0); i++)
            {
                if (shape[i, j])
                {
                    var slot = Instantiate(inventorySlotPrefab, this.transform);
                    Vector2 slotSize = slot.GetComponent<SpriteRenderer>().bounds.size;
                    slot.gameObject.name = "Slot " + slotCount;
                    slot.transform.position = new Vector3(-initialX + slotSize.x * i, -initialY + slotSize.y * j);
                    slot.transform.parent = slotsParent.transform;
                    slotCount++;
                    inventorySlots.Add(slot.GetComponent<InventorySlot>());
                }
            }
        }
    }

    protected void CreateSlotsParent()
    {
        slotsParent = new GameObject("Slots");
        slotsParent.transform.parent = this.transform;
    }


    bool[,] CreateShape(int width,int height) //TODO:this should be a config rather than a method
    {
        bool[,] shape = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                shape[i, j] = true;
            }
        }

        return shape;
    }

    public void HandleSlotCollision()
    {
        List<InventorySlot> closestInventorySlots = GetClosestInventorySlots(currentItem.GetComponent<Item>());
        bool isValidPosition = IsPositionValidToInsertItem(closestInventorySlots, currentItem);
        PaintSlots(closestInventorySlots, isValidPosition);
    }

    private List<InventorySlot> GetClosestInventorySlots(Item currentItem)
    {
        List<InventorySlot> closestInventorySlots = new List<InventorySlot>();
        foreach (var itemSlot in currentItem.itemSlots)
        {
            var distance = Mathf.Infinity;
            InventorySlot slotClosestToItemSlot = null;
            foreach (var inventorySlot in inventorySlots)
            {
                var distanceBetweenItemSlotAndInventorySlot = Vector2.Distance(inventorySlot.transform.position, itemSlot.transform.position);
                if (distance > distanceBetweenItemSlotAndInventorySlot && distanceBetweenItemSlotAndInventorySlot <= minimumDistance)
                {
                    slotClosestToItemSlot = inventorySlot;
                    distance = distanceBetweenItemSlotAndInventorySlot;
                    float color = distanceBetweenItemSlotAndInventorySlot - 0.9f;
                    color *= 10;
                    Debug.DrawLine(inventorySlot.transform.position, itemSlot.transform.position, new Color(color, 1, color));
                }
                else
                {
                    Debug.DrawLine(inventorySlot.transform.position, itemSlot.transform.position, new Color(1, 0, 0));
                }
            }
            if (slotClosestToItemSlot != null && !closestInventorySlots.Contains(slotClosestToItemSlot))
            {
                closestInventorySlots.Add(slotClosestToItemSlot);
            }
        }

        return closestInventorySlots;
    }

    private InventoryToItemMap GetInventorySlotToItemSlotMapping(InventorySlot inventorySlot, Item item)
    {
        GameObject closestItemSlot = item.itemSlots[0];
        var distance = Mathf.Infinity;
        foreach (var itemSlot in item.itemSlots)
        {
            var distanceBetweenItemSlotAndInventorySlot = Vector2.Distance(inventorySlot.transform.position, itemSlot.transform.position);
            if (distance > distanceBetweenItemSlotAndInventorySlot)
            {
                closestItemSlot = itemSlot;
                distance = distanceBetweenItemSlotAndInventorySlot;
            }
        }

        InventoryToItemMap map = new InventoryToItemMap(inventorySlot.transform, closestItemSlot.transform);
        return map;
    }

    private bool IsPositionValidToInsertItem(List<InventorySlot> candidateInventorySlots, GameObject currentItem)
    {
        bool isValidPosition = true;
        if (candidateInventorySlots.Count != currentItem.GetComponent<Item>().itemSlots.Count)
        {
            isValidPosition = false;
        }

        foreach (var slot in candidateInventorySlots)
        {
            if (slot.Item != null)
            {
                isValidPosition = false;
            }
        }

        return isValidPosition;
    }

    private void PaintSlots(List<InventorySlot> chosenSlots, bool isValidPosition)
    {
        foreach (var inventorySlot in inventorySlots)
        {
            if (chosenSlots.Contains(inventorySlot))
            {
                if (isValidPosition)
                {
                    inventorySlot.SetSlotSpriteToValidSprite();
                }
                else
                {
                    inventorySlot.SetSlotSpriteToInvalidSprite();
                }
            }
            else
            {
                inventorySlot.SetSlotSpriteToBaseSprite();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            var item = collision.transform.parent.gameObject;
            if (currentItem != item)
            {
                currentItem = item;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            var item = collision.transform.parent.gameObject.GetComponent<Item>();
            if (!IsItemInsideInventory(item) && currentItem)
            {
                currentItem = null;
            }
        }
    }

    private bool IsItemInsideInventory(Item item)
    {
        foreach (var slot in item.itemSlots)
        {
            Collider2D collider = slot.GetComponent<BoxCollider2D>();
            if (collider.IsTouching(inventoryCollider))
            {
                return true;
            }
        }

        return false;
    }

    private void DragBegunEventHandler(Item item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            foreach (var slot in inventorySlots)
            {
                if (slot.Item == item)
                {
                    slot.Item = null;
                }
            }
        }
    }

    private void DragEndedEventHandler()
    {
        if (currentItem)
        {
            var item = currentItem.GetComponent<Item>();
            List<InventorySlot> closestInventorySlots = GetClosestInventorySlots(item);
            bool isValidPosition = IsPositionValidToInsertItem(closestInventorySlots, currentItem);
            if (isValidPosition)
            {
                InventoryToItemMap map = GetInventorySlotToItemSlotMapping(closestInventorySlots[0], item);
                currentItem.GetComponent<Item>().UpdateAnchor(map.InventorySlotTransform, map.ItemSlotTransform);
                foreach (var slot in closestInventorySlots)
                {
                    slot.Item = item;
                }
                Items.Add(currentItem.GetComponent<Item>());
            }
        }
    }
}
