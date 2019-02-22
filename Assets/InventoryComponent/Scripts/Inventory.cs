﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public GameObject inventorySlotPrefab;
    GameObject slotsParent;

    public int height;
    public int width;

    GameObject[,] inventorySlots;
    
    //public bool[,] inventoryShape; //It should be like this

    // Use this for initialization
    void Start () {
        inventorySlots = new GameObject[width, height];
        SetupSlots(CreateShape(width, height));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected void SetupSlots(bool[,] shape)
    {
        CreateSlotsParent();
        InstantiateSlots(shape);
    }

    protected void InstantiateSlots(bool[,] shape)
    {
        int slotCount = 0;
        int initialX = width / 2;
        int initialY = height / 2;
        for (int j = shape.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0 ; i < shape.GetLength(0); i++)
            {
                if (shape[i, j])
                {
                    var slot = Instantiate(inventorySlotPrefab, this.transform);
                    Vector2 slotSize = slot.GetComponent<BoxCollider2D>().size;
                    slot.gameObject.name = "Slot " + slotCount;
                    slot.transform.position = new Vector3(-initialX + slotSize.x * i, -initialY + slotSize.y * j);
                    slot.transform.parent = slotsParent.transform;
                    slotCount++;
                    inventorySlots[i, j] = slot;
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
}
