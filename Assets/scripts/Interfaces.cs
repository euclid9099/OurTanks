using UnityEngine;

//Interface for everything which interacts with tank movement
interface MovementInteraction
{
    float moveX(float movx);
    float moveY(float movy);
    float movementCost();
}

interface BombInteraction
{
    void explode();
}

interface ProjInteraction
{
    bool hit();
}

interface Hazard
{
    Vector2 avoidingPath(Vector2 position);
}