﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public struct StairConnection
{
    public string connectsTo;
    public bool next;
    public int numConnections;
    [Tooltip("The number where connections begin, ie, the first stair in this group connects to this stair in the next floor. MUST MATCH BETWEEN FLOORS")]
    public int connectionsStartAt;
}

[CreateAssetMenu(fileName = "New Stairway", menuName = "Dungeon Generator/Machines/Stairway Placer", order = 2)]
public class StairPlacer : Machine
{
    [Header("Tile numbers")]
    public int upStairsIndex;
    public int downStairsIndex;

    [Header("Stair settings")]
    public List<StairConnection> stairsUp;
    public List<StairConnection> stairsDown;

    List<Vector2Int> ups = new List<Vector2Int>();
    List<Vector2Int> downs = new List<Vector2Int>();

    public int numExtraEntrances;
    public int numExtraExits;

    public List<int> excludeRoom;

    List<int> roomsToConnect = new List<int>();

    // Activate is called to start the machine
    public override void Activate()
    {
        ups.Clear();
        downs.Clear();
        roomsToConnect.Clear();
        for (int i = 0; i < generator.rooms.Count; i++)
        {
            roomsToConnect.Add(i);
        }

        foreach (int i in excludeRoom)
        {
            roomsToConnect.Remove(i);
        }

        //Shuffle!
        roomsToConnect = roomsToConnect.OrderBy(x => UnityEngine.Random.Range(int.MinValue, int.MaxValue)).ToList();

        int c = 0;
        //Place up stairs
        for (int i = 0; i < stairsUp.Count; i++)
        {
            StairConnection connection = stairsUp[i];
            for (int j = 0; j < connection.numConnections; j++)
            {
                Room r = generator.rooms[roomsToConnect[c]];
                Vector2Int loc = r.GetOpenSpace(1, generator.map);

                generator.map[loc.x, loc.y] = upStairsIndex;

                ups.Add(loc);
                c++;
            }
        }

        for (int i = c; i < c + numExtraEntrances; i++)
        {
            Room r = generator.rooms[roomsToConnect[i]];
            Vector2Int loc = r.GetOpenSpace(1, generator.map);
            ups.Add(loc);
        }

        //Shuffle!
        roomsToConnect = roomsToConnect.OrderBy(x => UnityEngine.Random.Range(int.MinValue, int.MaxValue)).ToList();

        c = 0;
        //Place down stairs
        //Place up stairs
        for (int i = 0; i < stairsDown.Count; i++)
        {
            StairConnection connection = stairsDown[i];
            for (int j = 0; j < connection.numConnections; j++)
            {
                Room r = generator.rooms[roomsToConnect[c]];
                Vector2Int loc = r.GetOpenSpace(1, generator.map);

                generator.map[loc.x, loc.y] = downStairsIndex;

                downs.Add(loc);
                c++;
            }
        }

        for (int i = c; i < c + numExtraExits; i++)
        {
            Room r = generator.rooms[roomsToConnect[i]];
            Vector2Int loc = r.GetOpenSpace(1, generator.map);
            downs.Add(loc);
        }
    }

    public override void PostActivation(Map m)
    {
        int c = 0;
        for (int i = 0; i < stairsUp.Count; i++)
        {
            for (int j = 0; j < stairsUp[i].numConnections; j++)
            {
                Stair stairs = m.GetTile(ups[c]) as Stair;
                if (stairs)
                {
                    if (stairsUp[i].next)
                    {
                        stairs.connectsToFloor = m.index - 1;
                    }
                    else
                    {
                        stairs.connectsToFloor = LevelLoader.singleton.GetIndexOf(stairsUp[i].connectsTo);
                    }
                    stairs.stairPair = stairsUp[i].connectionsStartAt + j;
                }
                else
                {
                    Debug.LogError("Grabbed tile that was not stair!", this);
                }
                c++;
            }
        }

        for (int i = 0; i < ups.Count; i++)
        {
            m.entrances.Add(ups[i]);
        }

        c = 0;
        for (int i = 0; i < stairsDown.Count; i++)
        {
            for (int j = 0; j < stairsDown[i].numConnections; j++)
            {
                Stair stairs = m.GetTile(downs[c]) as Stair;
                if (stairs)
                {
                    if (stairsDown[i].next)
                    {
                        stairs.connectsToFloor = m.index + 1;
                    }
                    else
                    {
                        stairs.connectsToFloor = LevelLoader.singleton.GetIndexOf(stairsDown[i].connectsTo);
                    }
                    stairs.stairPair = stairsDown[i].connectionsStartAt + j;
                }
                else
                {
                    Debug.LogError("Grabbed tile that was not stair!", this);
                }
                c++;
            }
        }

        for (int i = 0; i < downs.Count; i++)
        {
            m.exits.Add(downs[i]);
        }

        m.numStairsUp = stairsUp.Sum(x => x.numConnections);
        m.numStairsDown = stairsDown.Sum(x => x.numConnections);
    }
}
