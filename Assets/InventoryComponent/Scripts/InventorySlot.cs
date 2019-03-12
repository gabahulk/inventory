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
    public Item Item { get; set; }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            SetSlotSpriteToBaseSprite();
        }
    }

    public void SetSlotSpriteToValidSprite()
    {
        GetComponent<SpriteRenderer>().sprite = validInventorySlotSprite;
    }

    public void SetSlotSpriteToInvalidSprite()
    {
        GetComponent<SpriteRenderer>().sprite = invalidInventorySlotSprite;
    }

    public void SetSlotSpriteToBaseSprite()
    {
        GetComponent<SpriteRenderer>().sprite = baseInventorySlotSprite;
    }
}
