using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public GameObject inventorySlotPrefab;
    GameObject slotsParent;

    GameObject currentItem;
    BoxCollider2D inventoryCollider;

    public int height;
    public int width;
    public float minimumDistance = 0.9f;

    List<InventorySlot> inventorySlots;
    bool isValidPosition = true;
    //public bool[,] inventoryShape; //It should be like this
    private void OnEnable()
    {
        inventoryCollider = GetComponent<BoxCollider2D>();
        Item.DragEnded += DragEndedEventHandler;
    }

    private void OnDisable()
    {
        inventoryCollider = GetComponent<BoxCollider2D>();
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
                    slot.GetComponent<InventorySlot>().ID = slotCount;
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
        isValidPosition = true;
        List<InventorySlot> closestInventorySlots = new List<InventorySlot>();
        foreach (var itemSlot in currentItem.GetComponent<Item>().itemSlots)
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

        if (closestInventorySlots.Count != currentItem.GetComponent<Item>().itemSlots.Count)
        {
            isValidPosition = false;
        }
        foreach (var inventorySlot in inventorySlots)
        {
            if (closestInventorySlots.Contains(inventorySlot))
            {
                if (inventorySlot.Occupied)
                {
                    isValidPosition = false;
                }
            }
            else
            {
                inventorySlot.SetSlotSpriteToBaseSprite();
            }
        }

        foreach (var slot in closestInventorySlots)
        {
            if (isValidPosition)
            {
                slot.SetSlotSpriteToValidSprite();
            }
            else
            {
                slot.SetSlotSpriteToInvalidSprite();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    private void DragEndedEventHandler()
    {
        if (currentItem && isValidPosition)
        {
            //Get valid slots HERE!!
            //currentItem.GetComponent<Item>().UpdateAnchor();
        }
    }
}
