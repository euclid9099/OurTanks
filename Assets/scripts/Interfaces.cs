//Interface for everything which interacts with tank movement
interface MovementInteraction
{
    float moveX(float movx);
    float moveY(float movy);
    int movementCost();
}

interface BombInteraction
{
    void explode();
}

interface ProjInteraction
{
    void hit();
}