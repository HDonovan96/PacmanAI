﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRandom : BtNode
{
    private MemoryType m_memoryType;
    private bool m_weightedRandom;

    public TargetRandom(MemoryType memoryType, bool weightedRandom = true)
    {
        m_memoryType = memoryType;
        m_weightedRandom = weightedRandom;
    }

    public override NodeState Evaluate(Blackboard blackboard)
    {
        List<AIRememberedItem> possibleItems = new List<AIRememberedItem>();
        foreach (AIRememberedItem element in blackboard.rememberedItems[(int)m_memoryType])
        {
            if (element.activeMemory)
            {
                possibleItems.Add(element);
            }
        }

        if (possibleItems.Count == 0)
        {
            return NodeState.FAILURE;
        }

        if (!m_weightedRandom)
        {
            int rand = Random.Range(0, possibleItems.Count);
            if (possibleItems[rand] != blackboard.target)
            {
                blackboard.target = possibleItems[rand];
            }
        }
        else
        {
            float[] targetAngles = new float[possibleItems.Count];
            int i = 0;
            foreach (AIRememberedItem element in possibleItems)
            {
                Vector3 dir = (element.position - blackboard.owner.transform.position).normalized;
                targetAngles[i] = Vector3.Angle(blackboard.owner.transform.forward, dir);
                i++;
            }

            // sortedIndexList is used to store the sorted index positions of the target angles.
            // List is sorted into ascending order.
            List<int> sortedIndexList = new List<int>();
            sortedIndexList.Add(0);
            bool sortedIntoList;

            for (i = 1; i < targetAngles.Length; i++)
            {
                sortedIntoList = false;
                for (int j = 0; j < sortedIndexList.Count; j++)
                {
                    if (targetAngles[i] <= targetAngles[sortedIndexList[j]])
                    {
                        sortedIndexList.Insert(j, i);
                        sortedIntoList = true;
                        break;
                    }
                }

                if (!sortedIntoList)
                {
                    sortedIndexList.Add(i);
                }
            }

            // Weighted targetting is logarithmic, favouring items that have a smaller angle from the agents current facing.
            // Max random range is 180 rather than 360 due to Vector3.Angle returning a non-directional angle.
            float rand = Random.Range(0.0f, 180.0f);
            Debug.Log("Rand = " + rand);
            rand = -34.66f * Mathf.Log(rand) + 180.0f;

            Debug.Log("Rand = " + rand);
            foreach (int element in sortedIndexList)
            {
                Debug.Log(targetAngles[element]);
            }

            // If no target is found within the bound formed by rand then the target is set to the first item in the possibleItems list.
            bool setTarget = false;
            for (i = 0; i < sortedIndexList.Count; i++)
            {
                if (rand < targetAngles[sortedIndexList[i]])
                {
                    if (possibleItems[sortedIndexList[i]] != blackboard.target)
                    {
                        blackboard.target = possibleItems[sortedIndexList[i]];
                        Debug.Log("Targeted = " + targetAngles[sortedIndexList[i]]);
                        setTarget = true;
                        break;
                    }
                }
            }

            if (!setTarget)
            {
                if (possibleItems[0] != blackboard.target)
                {
                    blackboard.target = possibleItems[0];
                }
                else if (possibleItems.Count > 1)
                {
                   
                    blackboard.target = possibleItems[1];
                    
                }
            }
        }
        m_nodeState = NodeState.SUCCESS;
        return m_nodeState;
    }

    public override string getName()
    {
        return "TargetRandom";
    }
}