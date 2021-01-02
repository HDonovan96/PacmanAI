﻿// Title: Blackboard.cs
// Author: Joseph Walton-Rivers
// Collaborators: Harry Donovan
// Date Last Edited: 23/12/2020
// Last Edited By: Harry Donovan
// References: 
// File Source: Assets\Scripts\bt\Blackboard.cs
// Dependencies: Assets\Scripts\AIRememberedItem.cs
// Description: Blackboard used for each ai agent. Expanded to allow each ai agent to have a pool of items it remembers the locations of. These pools of gameobjects are generated at initilisation and then updated throughout runtime.

using System;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public GameObject owner;
    public GameObject target;
    GameObject[][] rememberedItems;

    public Blackboard(GameObject owner, GameObject itemPool, int[] maxRememberedItems)
    {
        this.owner = owner;

        string[] enumNames = Enum.GetNames(typeof(MemoryType));
        int numOfMemTypes = Enum.GetNames(typeof(MemoryType)).Length;
        rememberedItems = new GameObject[numOfMemTypes][];

        int loopLength;
        if (numOfMemTypes > maxRememberedItems.Length)
        {
            loopLength = maxRememberedItems.Length;
        }
        else
        {
            loopLength = numOfMemTypes;
        }

        for (int i = 0; i < loopLength; i++)
        {
            rememberedItems[i] = new GameObject[maxRememberedItems[i]];
        }

        GameObject spawnObject = new GameObject();
        GameObject agentSubPool = GameObject.Instantiate(spawnObject, Vector3.zero, new Quaternion(), itemPool.transform);
        agentSubPool.name = owner.name + "ItemPool";

        GameObject[] typeSubPools = new GameObject[loopLength];

        for (int i = 0; i < loopLength; i++)
        {
            typeSubPools[i] = GameObject.Instantiate(spawnObject, Vector3.zero, new Quaternion(), agentSubPool.transform);
            typeSubPools[i].name = enumNames[i] + "Pool";

            for (int j = 0; j < rememberedItems[i].Length; j++)
            {
                rememberedItems[i][j] = GameObject.Instantiate(spawnObject, Vector3.zero, new Quaternion(), typeSubPools[i].transform);
                rememberedItems[i][j].name = "RememberedItem" + j;
                AIRememberedItem aIRememberedItem = rememberedItems[i][j].AddComponent<AIRememberedItem>();
                aIRememberedItem.Initialise(this, (MemoryType)i);
            }
        }

        GameObject.Destroy(spawnObject);
    }

    // Replaces the furthest away (from the owner) remembered item with the new itemToAdd.
    // If no remembered item is further than the itemToAdd the item is not added.
    // If a remembered item is null (so has not yet been set) then it is automatically replaced.
    public void UpdateRememberedItems(MemoryType memoryType, GameObject itemToAdd)
    {
        float dist = Vector3.Distance(owner.transform.position, itemToAdd.transform.position);

        int i = 0;
        int replacementIndex = int.MaxValue;
        foreach (GameObject element in rememberedItems[(int)memoryType])
        {
            // If the gameobject is already being remembered then it is not added.
            // Uses a distance which would cause problems if multiple of the same object could be next to each other.
            // However for this application that can't happen so this implementation is suitable.
            if (Vector3.Distance(itemToAdd.transform.position, element.transform.position) < 1.0f)
            {
                replacementIndex = int.MaxValue;
                break;
            }

            float newDist = Vector3.Distance(element.transform.position, owner.transform.position);

            if (newDist > dist)
            {
                dist = newDist;
                replacementIndex = i;
            }

            i++;
        }

        if (replacementIndex != int.MaxValue)
        {
            rememberedItems[(int)memoryType][replacementIndex].GetComponent<AIRememberedItem>().UpdateLocation(itemToAdd.transform.position);
        }
    }
}