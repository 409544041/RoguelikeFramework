﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class InputTracking : MonoBehaviour
{
    //Public functions for accessing all of this
    public static Queue<PlayerAction> actions = new Queue<PlayerAction>();

    public static bool HasNextAction()
    { 
        return actions.Count > 0;
    }

    public static PlayerAction PopNextAction()
    {
        if (HasNextAction())
        {
            return actions.Dequeue();
        }
        else
        {
            return PlayerAction.NONE;
        }
    }

    public static PlayerAction PeekNextAction()
    {
        if (HasNextAction())
        {
            return actions.Peek();
        }
        else
        {
            return PlayerAction.NONE;
        }
    }

    public static void PushAction(PlayerAction act)
    {
        actions.Enqueue(act);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool addedAction = false;
        
        //Add movements
        if (Left())
        {
            if (Up())
            {
                actions.Enqueue(PlayerAction.MOVE_UP_LEFT);
            }
            else if (Down())
            {
                actions.Enqueue(PlayerAction.MOVE_DOWN_LEFT);
            }
            else
            {
                actions.Enqueue(PlayerAction.MOVE_LEFT);
            }

            addedAction = true;
        }
        else if (Right())
        {
            if (Up())
            {
                actions.Enqueue(PlayerAction.MOVE_UP_RIGHT);
            }
            else if (Down())
            {
                actions.Enqueue(PlayerAction.MOVE_DOWN_RIGHT);
            }
            else
            {
                actions.Enqueue(PlayerAction.MOVE_RIGHT);
            }

            addedAction = true;
        }
        else if (Down())
        {
            actions.Enqueue(PlayerAction.MOVE_DOWN);
            addedAction = true;
        }
        else if (Up())
        {
            actions.Enqueue(PlayerAction.MOVE_UP);
            addedAction = true;
        }
        else if (UpLeft())
        {
            actions.Enqueue(PlayerAction.MOVE_UP_LEFT);
        }
        else if (UpRight())
        {
            actions.Enqueue(PlayerAction.MOVE_UP_RIGHT);
        }
        else if (DownLeft())
        {
            actions.Enqueue(PlayerAction.MOVE_DOWN_LEFT);
        }
        else if (DownRight())
        {
            actions.Enqueue(PlayerAction.MOVE_DOWN_RIGHT);
        }
    }

    private bool Left()
    {
        return (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.LeftArrow));
    }

    private bool Right()
    {
        return (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.RightArrow));
    }

    private bool Up()
    {
        return (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.UpArrow));
    }

    private bool Down()
    {
        return (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.DownArrow));
    }

    private bool UpLeft()
    {
        return (Input.GetKeyDown(KeyCode.Y));
    }

    private bool UpRight()
    {
        return (Input.GetKeyDown(KeyCode.U));
    }

    private bool DownLeft()
    {
        return (Input.GetKeyDown(KeyCode.B));
    }

    private bool DownRight()
    {
        return (Input.GetKeyDown(KeyCode.N));
    }

}