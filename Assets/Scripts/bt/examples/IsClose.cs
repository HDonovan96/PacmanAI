﻿// Original Author: Joseph Walton-Rivers
// Collaborators: Harry Donovan
// References: 
// Dependencies: Assets\Scripts\bt\BtNode.cs
// Description: Checks if a remembered object is within a given range. If no memoryType is given then the blackboard target is used.

using UnityEngine;

public class IsClose : BtNode
{
    private float m_distanceLimit = 10;

    bool useBlackboardTarget;
    private MemoryType memoryType;

    // If no target is passed it is assumed to use the blackboards target.
    public IsClose(float distanceLimit)
    {
        m_distanceLimit = distanceLimit;
        useBlackboardTarget = true;
    }

    public IsClose(float distanceLimit, MemoryType memoryType)
    {
        m_distanceLimit = distanceLimit;
        this.memoryType = memoryType;
        useBlackboardTarget = false;
    }

    public override NodeState Evaluate(Blackboard blackboard)
    {
        float distance = float.MaxValue;

        if (useBlackboardTarget)
        {
            if (blackboard.target == null)
            {
                return NodeState.FAILURE;
            }

            distance = Vector3.Distance(blackboard.owner.transform.position, blackboard.target.position);
        }
        else
        {
            float newDist;
            foreach (AIRememberedItem element in blackboard.rememberedItems[(int)memoryType])
            {
                if (element.activeMemory)
                {
                    newDist = Vector3.Distance(element.position, blackboard.owner.transform.position);
                    if (newDist < distance)
                    {
                        distance = newDist;
                    }
                }
            }
        }

        if (distance < m_distanceLimit)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    public override string getName()
    {
        return "isClose";
    }

}