using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class InventorySlot : MonoBehaviour
{
    public Sprite baseInventorySlotSprite;
    public Sprite validInventorySlotSprite;
    public Sprite invalidInventorySlotSprite;

    public bool HasItem { get; set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        print("ajsdiasjidasjidasi");
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            if (HasItem)
            {
                renderer.sprite = invalidInventorySlotSprite;
            }
            else
            {
                renderer.sprite = validInventorySlotSprite;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<SpriteRenderer>().sprite = baseInventorySlotSprite;
    }
}
