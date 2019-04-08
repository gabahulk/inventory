using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour {
    public bool[,] itemShape;
    public GameObject itemSlotPrefab;

    public List<GameObject> itemSlots = new List<GameObject>();

    public delegate void BeginDragEventHandler(Item item);
    public static event BeginDragEventHandler DragBegun;

    public delegate void EndDragEventHandler();
    public static event EndDragEventHandler DragEnded;

    Transform anchorPosition;
    Transform referenceSlot;

    // Use this for initialization
    void Start () {
        bool[,] shape = new bool[2, 1];
        shape[0, 0] = true;
        shape[1, 0] = true;
        //shape[1, 1] = true;
        CreateShape(shape);
    }

	// Update is called once per frame
	void Update () {

	}

    void CreateShape(bool[,] shape)
    {
        int slotCount = 0;
        float initialX = shape.GetLength(0) / 2;
        float initialY = shape.GetLength(1) / 2;
        for (int j = shape.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                if (shape[i, j])
                {
                    var slot = Instantiate(itemSlotPrefab, this.transform);

                    AddDragControlsToSlot(slot);

                    itemSlots.Add(slot);

                    Vector2 slotSize = slot.GetComponent<SpriteRenderer>().bounds.size;
                    slot.gameObject.name = "Slot" + slotCount;
                    slot.transform.position = new Vector3(transform.position.x -initialX + slotSize.x * i, transform.position.y - initialY + slotSize.y * j, -1);
                    slot.transform.parent = this.transform;
                    slotCount++;
                }
            }
        }
    }

    void AddDragControlsToSlot(GameObject slot)
    {
        EventTrigger trigger = slot.GetComponent<EventTrigger>();

        EventTrigger.Entry onBeginDragEventEntry = new EventTrigger.Entry();
        onBeginDragEventEntry.eventID = EventTriggerType.BeginDrag;
        onBeginDragEventEntry.callback.AddListener((eventData) => { OnBeginDrag((PointerEventData)eventData); });
        trigger.triggers.Add(onBeginDragEventEntry);

        EventTrigger.Entry onDragEventEntry = new EventTrigger.Entry();
        onDragEventEntry.eventID = EventTriggerType.Drag;
        onDragEventEntry.callback.AddListener((eventData) => { OnDrag((PointerEventData)eventData); });
        trigger.triggers.Add(onDragEventEntry);

        EventTrigger.Entry onEndDragEventEntry = new EventTrigger.Entry();
        onEndDragEventEntry.eventID = EventTriggerType.EndDrag;
        onEndDragEventEntry.callback.AddListener((eventData) => { OnEndDrag((PointerEventData)eventData); });
        trigger.triggers.Add(onEndDragEventEntry);
    }


    public void OnBeginDrag(PointerEventData data) {
        DragMovement(data);
        DragBegun(this);
    }

    public void OnDrag(PointerEventData data)
    {
        DragMovement(data);
    }

    public void OnEndDrag(PointerEventData data)
    {
        DragMovement(data);
        DragEnded();
        MoveToValidPosition();
    }

    public void DragMovement(PointerEventData data)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(data.position);
        pos.z = -1;
        this.transform.position = pos;
    }

    public void Bind(Transform destination, Transform slot)
    {
        anchorPosition = destination;
        referenceSlot = slot;
    }

    public void MoveToValidPosition()
    {
        referenceSlot.parent = null;
        this.transform.parent = referenceSlot;
        referenceSlot.position = new Vector3(anchorPosition.position.x, anchorPosition.position.y, referenceSlot.position.z);

        this.transform.parent = null;
        referenceSlot.parent = this.transform;
    }

}
