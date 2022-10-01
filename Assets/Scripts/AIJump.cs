using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code for calculating AI jump trajectories for specific target 
public class JumpInfo : IComparable<JumpInfo>
{
    public Vector2 jumpVelocity = new Vector2();
    public float jumpTime;

    public JumpInfo(Vector2 _jumpVelocity, float _jumpTime)
    {
        jumpVelocity = _jumpVelocity;
        jumpTime = _jumpTime;
    }

    public int CompareTo(JumpInfo other)
    {

        if (other == null)
        {
            return 1;
        }
        return this.jumpTime.CompareTo(other.jumpTime);
    }
}

public static class AIJump
{
    public static bool InJumpRange(Vector2 aiPosition, Vector2 targetPosition, PathfindingAgentInfo aiInfo)
    {
        Vector2 targetDisplacement = targetPosition - aiPosition;
        bool inXRange = Mathf.Abs(targetDisplacement.x) <= aiInfo.jumpRange;
        bool inYRange = targetDisplacement.y > 0.0f && targetDisplacement.y <= aiInfo.jumpHeight;
        return inXRange && inYRange;
    }

    public static JumpInfo GetJumpParameters(Vector2 startPosition, Vector2 targetPosition, PathfindingAgentInfo agentInfo, float gravity, LayerMask collisionMask)
    {
        JumpInfo shortestJump = new JumpInfo(Vector2.zero, 0.0f);
        Vector2 displacement = targetPosition - startPosition;
        int numSteps = 2 * (Mathf.Abs(Mathf.CeilToInt(displacement.x)) + Mathf.CeilToInt(displacement.y));

        for (int jump = 0; jump < agentInfo.jumpHeight; jump++)
        {
            float jumpSpeed = Mathf.Sqrt(-2 * gravity * (jump + 1.1f));
            float sqrtTerm = 2 * gravity * displacement.y + jumpSpeed * jumpSpeed;
            if (sqrtTerm > 0.0f)
            {
                float timeA = (-jumpSpeed + Mathf.Sqrt(sqrtTerm)) / gravity;
                float timeB = (-jumpSpeed - Mathf.Sqrt(sqrtTerm)) / gravity;
                float velA = displacement.x / timeA;
                float velB = displacement.x / timeB;

                if (Mathf.Abs(velA) < 9.0f && timeA > 0.0f)
                {
                    if (shortestJump.jumpTime <= 0 || shortestJump.jumpTime > timeA)
                    {
                        Vector2 velocity = new Vector2(velA, jumpSpeed);
                        JumpInfo potentialJump = new JumpInfo(velocity, timeA);
                        if (CheckValidJump(startPosition, potentialJump, numSteps, agentInfo, gravity, collisionMask))
                        {
                            shortestJump = potentialJump;
                        }
                    }
                }

                if (Mathf.Abs(velB) < 9.0f && timeB > 0.0f)
                {
                    if (shortestJump.jumpTime <= 0 || shortestJump.jumpTime > timeB)
                    {
                        Vector2 velocity = new Vector2(velB, jumpSpeed);
                        JumpInfo potentialJump = new JumpInfo(velocity, timeB);
                        if (CheckValidJump(startPosition, potentialJump, numSteps, agentInfo, gravity, collisionMask))
                        {
                            shortestJump = new JumpInfo(velocity, timeB);
                        }
                    }
                }

                if (shortestJump.jumpTime > 0)
                {
                    break;
                }
            }
        }

        return shortestJump;
    }

    static bool CheckValidJump(Vector2 startPosition, JumpInfo jumpInfo, int numSteps, PathfindingAgentInfo agentInfo, float gravity, LayerMask collisionMask)
    {
        float jumpTime = jumpInfo.jumpTime;
        Vector2 jumpVelocity = jumpInfo.jumpVelocity;

        bool validTrajectory = true;
        float deltaTimeRatio = (jumpTime / (float)numSteps);
        //Vector2 colliderCenter = new Vector2(gridOwner.bounds.center.x, gridOwner.bounds.center.y);
        Vector2 NodeToCenter = new Vector2(0.0f, agentInfo.agentCollider.size.y * 0.5f - 0.5f);
        for (int k = 0; k < numSteps; k++)
        {
            float bestTimeStep = deltaTimeRatio * k;
            float sX = jumpVelocity.x * bestTimeStep;
            float sY = jumpVelocity.y * bestTimeStep + 0.5f * gravity * bestTimeStep * bestTimeStep;
            Vector2 collisionBound = new Vector2(1.0f, agentInfo.agentCollider.size.y);

            Collider2D[] collisions = Physics2D.OverlapBoxAll(startPosition + new Vector2(sX, sY), collisionBound, 0.0f, collisionMask);

            if (k != 0 && collisions.Length > 0)
            {
                validTrajectory = false;
                break;
            }
        }

        if (validTrajectory)
        {
            return true;
        }
        return false;
    }
}
