﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBeingMovedTo : BtNode
{
    public override NodeState Evaluate(Blackboard blackboard)
    {
        if (blackboard.target == null)
        {
            return NodeState.FAILURE;
        }

        // If only one player location is being tracked this will always fail as it's not enough data to get a direction.
        if (blackboard.rememberedItems[(int)MemoryType.Player].Length < 2)
        {
            Debug.LogAssertion("Can't get player movement direction as remembered player positions is less than one. Please set " + blackboard.owner.name + " rememberedItemCounts (index " + (int)MemoryType.Player + " to at least 2");
            return NodeState.FAILURE;
        }

        // // If either of the two latest player locations aren't tracking a real object then return fail.
        // if (!blackboard.rememberedItems[(int)MemoryType.Player][0].GetComponent<AIRememberedItem>().activeMemory | !blackboard.rememberedItems[(int)MemoryType.Player][1].GetComponent<AIRememberedItem>().activeMemory)
        // {
        //     return NodeState.FAILURE;
        // }

        Vector3 agentToTargetdirection = blackboard.target.position - blackboard.owner.transform.position;
        Vector2 agentToTargetdirection2D = new Vector2(agentToTargetdirection.x, agentToTargetdirection.z).normalized;

        // Fetch the index ref for the two newest remembered player positions.
        // These should be arrays.
        float newestTime = float.MinValue;
        float secondNewestTime = float.MinValue;
        int newestIndex = int.MaxValue;
        int secondNewestIndex = int.MaxValue;
        int i = 0;

        foreach (AIRememberedItem element in blackboard.rememberedItems[(int)MemoryType.Player])
        {
            if (element.activeMemory)
            {
                if (element.timeUpdated > newestTime)
                {
                    secondNewestTime = newestTime;
                    secondNewestIndex = newestIndex;
                    newestTime = element.timeUpdated;
                    newestIndex = i;
                }
                else if (element.timeUpdated > secondNewestTime)
                {
                    secondNewestTime = element.timeUpdated;
                    secondNewestIndex = i;
                }
            }
            i++;
        }

        if (newestIndex == int.MaxValue | secondNewestIndex == int.MaxValue)
        {
            return NodeState.FAILURE;
        }

        Vector3 targetMovementDirection = blackboard.rememberedItems[(int)MemoryType.Player][newestIndex].position - blackboard.rememberedItems[(int)MemoryType.Player][secondNewestIndex].position;
        Vector2 targetMovementDirection2D = new Vector2(targetMovementDirection.x, targetMovementDirection.z).normalized;

        float angle = Vector3.Dot(agentToTargetdirection2D, targetMovementDirection2D);
        angle = Mathf.Acos(angle);
        angle *= Mathf.Rad2Deg;

        if (angle <= 270 && angle >= 90)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }

    public override string getName()
    {
        return "IsBeingMovedTo";
    }
}