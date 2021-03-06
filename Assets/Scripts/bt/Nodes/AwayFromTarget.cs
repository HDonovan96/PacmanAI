﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AwayFromTarget : BtNode
{
    private NavMeshAgent m_agent;
    private float m_fleeDistance;

    public AwayFromTarget(float fleeDistance)
    {
        m_fleeDistance = fleeDistance;
    }

    public override NodeState Evaluate(Blackboard blackboard)
    {
        if (m_agent == null)
        {
            m_agent = blackboard.owner.GetComponent<NavMeshAgent>();
        }

        // if target is null, we can't move towards it!
        if (blackboard.target == null)
        {
            return NodeState.FAILURE;
        }

        if (Vector3.Distance(blackboard.owner.transform.position, blackboard.target.position) > m_fleeDistance)
        {
            return NodeState.FAILURE;
        }

        Vector3 targetPos = blackboard.owner.transform.position - blackboard.target.position;
        targetPos *= m_fleeDistance;
        targetPos += blackboard.target.position;
        m_agent.SetDestination(targetPos);

        if (Vector3.Distance(blackboard.owner.transform.position, blackboard.target.position) < m_fleeDistance)
        {
            return NodeState.RUNNING;
        }

        return NodeState.SUCCESS;
    }

    public override string getName()
    {
        return "TowardsTarget";
    }
}