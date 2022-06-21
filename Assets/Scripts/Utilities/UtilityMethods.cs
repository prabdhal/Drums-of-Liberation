using UnityEngine;
using UnityEngine.AI;

public static class UtilityMethods
{
    public static Vector2 AgentVelocityToVector2DInput(NavMeshAgent agent)
    {
        float xValue;
        float yValue;
        // Get the NavMeshAgent's desired velocity direction relative from it's actual position
        Vector3 desiredVelocityRelativeToAgent = agent.transform.InverseTransformDirection(agent.desiredVelocity);
        // Normalize the vector so it doesn't have a magnitude beyond 1.0f
        desiredVelocityRelativeToAgent.Normalize();
        // X value will be the X value of the vector
        xValue = desiredVelocityRelativeToAgent.x;
        // Y value will be the Z value of the vector
        yValue = desiredVelocityRelativeToAgent.z;
        // It's worth noting that you should scale this 2D vector by a desired speed on a scale of 0 - 1
        return new Vector2(xValue, yValue);
    }
}
