﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#if  UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class CustomTile : MonoBehaviour
{
    //Stuff that will change a lot, and should be visible
    [Header("Active gameplay elements")]
    public bool isVisible = false;
    public bool beenSeen = false;
    public bool dirty = true;

    public int x, y;
    
    //Stuff that will not change a lot, and should not be (too) visible
    [Header("Static elements")] 
    public string name;
    public string description;
    public Sprite sprite;
    public float movementCost;
    public bool blocksVision;
    public Color color = Color.white;
    public Monster currentlyStanding;

    //Floor visualization
    public List<Item> itemsOnFloor;
    private Item displayedItem;

    private bool hidden = true;
    private SpriteRenderer renderer;

    //Stuff used for convenience editor hacking, and should never be seen.
    
    /* If you see this and don't know what this is, ask me! It's super useful
     * for hacking up the editor, and making things easy. The #if's in this file
     * are used to make the sprite in the sprite renderer equal the sprite in this file,
     * so you can't forget to not change both. */
    #if UNITY_EDITOR
    private Sprite currentSprite;
    private Color currentColor;
    #endif
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Starts as on, so that Unity 
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
        
        RebuildGraphics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reveal()
    {
        isVisible = true;
        beenSeen = true;
        dirty = true;
    }

    public void Clear()
    {
        isVisible = false;
        dirty = true;
    }

    private void LateUpdate()
    {
        if (dirty)
        {
            RebuildGraphics();
        }
    }

    //Todo: account for visible or not in this function. Till then, just rebuild graphics every time
    public void AddItem(Item i)
    {
        i.Drop();
        i.SetLocation(new Vector2Int(x, y));
        itemsOnFloor.Add(i);
        if (displayedItem)
        {
            displayedItem.DisableSprite();
        }
        displayedItem = i;
        i.EnableSprite();
        RebuildGraphics(); //Extremely slow and stupid, do this better
    }

    public Item RemoveItem(int idx)
    {
        Item toRemove = itemsOnFloor[idx];
        itemsOnFloor.RemoveAt(idx);
        if (displayedItem == toRemove)
        {
            if (itemsOnFloor.Count > 0)
            {
                displayedItem = itemsOnFloor[itemsOnFloor.Count - 1];
            }
            else
            {
                displayedItem = null;
            }
            RebuildGraphics(); //Extrememly slow and stupid, do this better
        }
        return toRemove;
    }

    public void RebuildMapData()
    {
        Map.singleton.blocksVision[x, y] = blocksVision;
        Map.singleton.moveCosts[x, y] = movementCost;
    }

    public bool BlocksMovement()
    {
        return movementCost < 0;
    }

    public void Highlight(Color hcolor)
    {
        renderer.color = hcolor;
    }
    
    public void StopHighlight()
    {
        RebuildGraphics();
    }

    private void RebuildGraphics()
    {
        renderer.sprite = sprite;
        if (isVisible)
        {
            renderer.color = color;
            if (hidden)
            {
                hidden = false;
                renderer.enabled = true;
            }
            if (displayedItem)
            {
                displayedItem.EnableSprite();
            }
            else
            {
                if (itemsOnFloor.Count > 0)
                {
                    displayedItem = itemsOnFloor[itemsOnFloor.Count - 1];
                    displayedItem.EnableSprite();
                }
            }
        }
        else
        {
            //TODO: Item coloring on tiles that are not visible anymore
            if (beenSeen)
            {
                float gray = color.grayscale / 2;
                renderer.color = new Color(gray, gray, gray);
            }
            else
            {
                if (hidden)
                {
                    renderer.enabled = false;
                    if (displayedItem)
                    {
                        displayedItem.DisableSprite();
                    }
                }
                else
                {
                    displayedItem.EnableSprite();
                }
                renderer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }

        }

        dirty = false;
    }


#if  UNITY_EDITOR
    
    //Editor only functions - For convenience

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }
        if (currentSprite)
        {
            SpriteRenderer render = GetComponent<SpriteRenderer>();
            if (sprite == currentSprite)
            {
                sprite = render.sprite;
            }
            else
            {
                render.sprite = sprite;
            }

            currentSprite = sprite;
            color = render.color;
        }
        else
        {
            //Don't change anything!
            currentSprite = sprite;
        }
    }
#endif
}
