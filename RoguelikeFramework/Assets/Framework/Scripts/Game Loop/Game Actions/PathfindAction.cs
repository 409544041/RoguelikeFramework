﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindAction : GameAction
{
    public Vector2Int goal;
    bool firstTurn = true;
    bool takesStairs = true;
    //Constuctor for the action; must include caller!
    public PathfindAction(Vector2Int location, bool takesStairs = true)
    {
        //Construct me! Assigns caller by default in the base class
        goal = location;
        this.takesStairs = takesStairs;
    }

    public override IEnumerator TakeAction()
    {
        Path path = Pathfinding.FindPath(caller.location, goal);
        if (path.Cost() < 0)
        {
            Debug.LogWarning("Monster cannot find path to location! Aborting");
            yield return GameAction.Abort;
        }

        while (path.Count() > 0)
        {
            Vector2Int next = path.Pop();
            MoveAction act;
            if (takesStairs)
            {
                act = new MoveAction(next);
            }
            else
            {
                act = new MoveAction(next, true, false);
            }

            caller.UpdateLOS();

            if (!firstTurn && caller.view.visibleMonsters.FindAll(x => (x.faction & caller.faction) == 0).Count > 0)
            {
                yield break;
            }

            act.Setup(caller);
            while (act.action.MoveNext())
            {
                yield return act.action.Current;
            }
            firstTurn = false;

            //yield return new WaitForSeconds(.05f);

            yield return GameAction.StateCheck;
        }
    }

    //Called after construction, but before execution!
    //This is THE FIRST spot where caller is not null! Heres a great spot to actually set things up.
    public override void OnSetup()
    {

    }
}