using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour {
    public bool[,] itemShape;
    public GameObject itemSlotBehaviorPrefab;

    // Use this for initialization
    void Start () {
        bool[,] shape = new bool[2, 2];
        shape[0, 0] = true;
        shape[1, 0] = true;
        shape[1, 1] = true;
        CreateShape(shape);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateShape(bool[,] shape)
    {
        int slotCount = 0;
        int initialX = shape.GetLength(0) / 2;
        int initialY = shape.GetLength(1) / 2;
        for (int j = shape.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                if (shape[i, j])
                {
                    var slot = Instantiate(itemSlotBehaviorPrefab, this.transform);

                    AddDragControlsToSlot(slot);

                    Vector2 slotSize = slot.GetComponent<BoxCollider2D>().size;
                    slot.gameObject.name = "Slot" + slotCount;
                    slot.transform.position = new Vector3(-initialX + slotSize.x * i, -initialY + slotSize.y * j);
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
        //print("OnBeginDrag");
        DragMovement(data);
    }

    public void OnDrag(PointerEventData data)
    {
        DragMovement(data);
        //print("OnDrag");
    }

    public void OnEndDrag(PointerEventData data)
    {
        DragMovement(data);
        //print("OnEndDrag");
    }

    public void DragMovement(PointerEventData data)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(data.position);
        pos.z = 0;
        this.transform.position = pos;
    }
}
