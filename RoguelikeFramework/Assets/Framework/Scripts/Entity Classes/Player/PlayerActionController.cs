﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerActionController : ActionController
{
    public override IEnumerator DetermineAction()
    {
        if (InputTracking.HasNextAction())
        {
            Player.player.view.CollectEntities(Map.current);
            PlayerAction action = InputTracking.PopNextAction();
            switch (action)
            {
                //Handle Movement code
                case PlayerAction.MOVE_UP:
                    nextAction = new MoveAction(monster.location + Vector2Int.up);
                    break;
                case PlayerAction.MOVE_DOWN:
                    nextAction = new MoveAction(monster.location + Vector2Int.down);
                    break;
                case PlayerAction.MOVE_LEFT:
                    nextAction = new MoveAction(monster.location + Vector2Int.left);
                    break;
                case PlayerAction.MOVE_RIGHT:
                    nextAction = new MoveAction(monster.location + Vector2Int.right);
                    break;
                case PlayerAction.MOVE_UP_LEFT:
                    nextAction = new MoveAction(monster.location + new Vector2Int(-1, 1));
                    break;
                case PlayerAction.MOVE_UP_RIGHT:
                    nextAction = new MoveAction(monster.location + new Vector2Int(1, 1));
                    break;
                case PlayerAction.MOVE_DOWN_LEFT:
                    nextAction = new MoveAction(monster.location + new Vector2Int(-1, -1));
                    break;
                case PlayerAction.MOVE_DOWN_RIGHT:
                    nextAction = new MoveAction(monster.location + new Vector2Int(1, -1));
                    break;
                case PlayerAction.WAIT:
                    nextAction = new WaitAction();
                    break;
                case PlayerAction.DROP_ITEMS:
                    Debug.Log("Dropping items!");
                    UIController.singleton.OpenInventoryDrop();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.PICK_UP_ITEMS:
                    //Intelligently pick up items, opening dialouge box if needed.
                    PickupSmartDetection();

                    //Check if dialouge box is opened; if so, freeze until transaction is done
                    if (UIController.WindowsOpen)
                    {
                        yield return new WaitUntil(() => !UIController.WindowsOpen);
                    }
                    break;
                case PlayerAction.OPEN_INVENTORY:
                    UIController.singleton.OpenInventoryInspect();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.EQUIP:
                    UIController.singleton.OpenEquipmentInspect();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.UNEQUIP:
                    UIController.singleton.OpenEquipmentUnequip();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.APPLY:
                    UIController.singleton.OpenInventoryApply();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.CAST_SPELL:
                    UIController.singleton.OpenAbilities();
                    yield return new WaitUntil(() => !UIController.WindowsOpen);
                    break;
                case PlayerAction.FIRE:
                    nextAction = new RangedAttackAction();
                    break;
                case PlayerAction.ASCEND:
                    if (AutoStairs(true))
                    {
                        nextAction = new ChangeLevelAction(true);
                    }
                    else
                    {
                        //TODO: Highlight path to nearest stair!
                        yield return new WaitUntil(() => InputTracking.HasNextAction());
                        PlayerAction newAction = InputTracking.PopNextAction();
                        if (newAction == PlayerAction.ASCEND)
                        {
                            PathToNearestStair(true);
                        }
                    }
                    break;
                case PlayerAction.DESCEND:
                    if (AutoStairs(false))
                    {
                        nextAction = new ChangeLevelAction(false);
                    }
                    else
                    {
                        //TODO: Highlight path to nearest stair!
                        //TODO: This whole waiting and checking thing should be reworked
                        //as a variant of the FindNearest Action
                        yield return new WaitUntil(() => InputTracking.HasNextAction());
                        PlayerAction newAction = InputTracking.PopNextAction();
                        if (newAction == PlayerAction.DESCEND)
                        {
                            PathToNearestStair(false);
                        }
                    }
                    break;
                case PlayerAction.AUTO_ATTACK:
                    nextAction = new AutoAttackAction();
                    break;
                case PlayerAction.AUTO_EXPLORE:
                    nextAction = new AutoExploreAction();
                    break;

                //Handle potentially weird cases (Thanks, Nethack design philosophy!)
                case PlayerAction.ESCAPE_SCREEN:
                    //TODO: Open up the pause menu screen here
                    RogueUIPanel.ExitTopLevel(); //This really, really shouldn't do anything. Let it happen, though!
                    break;
                case PlayerAction.ACCEPT:
                case PlayerAction.NONE:
                    Debug.Log("Player read an input set to do nothing", this);
                    break;
                default:
                    Debug.LogError($"Player read an input that has no switch case: {action}");
                    break;
            }
        }
    }

    private bool AutoStairs(bool up)
    {
        Stair stair = Map.current.GetTile(monster.location) as Stair;
        return (stair && !(stair.upStair ^ up));
    }

    private void PathToNearestStair(bool up)
    {
        List<Vector2Int> goals;
        if (up)
        {
            goals = Map.current.entrances.GetRange(0, Map.current.numStairsUp);
        }
        else
        {
            goals = Map.current.exits.GetRange(0, Map.current.numStairsDown);
        }

        goals = goals.FindAll(x => !Map.current.GetTile(x).isHidden);
        if (goals.Count == 0)
        {
            Debug.Log("Console: You don't know of any matching stairs!");
            return;
        }
        nextAction = new FindNearestAction(goals);
    }

    //Item pickup, but with a little logic for determining if a UI needs to get involved.
    private void PickupSmartDetection()
    {
        CustomTile tile = Map.current.GetTile(monster.location);
        Inventory onFloor = tile.GetComponent<Inventory>();
        switch (onFloor.Count)
        {
            case 0:
                return; //Exit early
            case 1:
                //Use the new pickup action system to just grab whatever is there.
                //If this breaks, the problem now lies in that file, instead of cluttering Player.cs
                nextAction = new PickupAction(0);
                break;
            default:
                //Open dialouge box
                UIController.singleton.OpenInventoryPickup();
                break;

        }
    }

    public override IEnumerator DetermineTarget(Targeting targeting, BoolDelegate setValidityTo)
    {
        UIController.singleton.OpenTargetting(targeting, setValidityTo);
        yield return new WaitUntil(() => !UIController.WindowsOpen);
    }
}
