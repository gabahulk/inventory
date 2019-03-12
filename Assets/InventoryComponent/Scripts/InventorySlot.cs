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
    public int ID{ get; set; }
    public GameObject CurrentItemSlot { get; set; }
    public bool Occupied{ get; set; }

    // Use this for initialization
    void Start () {
    }

	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            CurrentItemSlot = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemSlot"))
        {
            SetSlotSpriteToBaseSprite();
            CurrentItemSlot = null;
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
