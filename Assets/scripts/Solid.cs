using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : MonoBehaviour, MovementInteraction
{
    public float movementCost()
    {
        return Mathf.Infinity;
    }

    public float moveX(float movx)
    {
        return 0;
    }

    public float moveY(float movy)
    {
        return 0;
    }
}
