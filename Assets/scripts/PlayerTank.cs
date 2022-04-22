using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    protected override Vector2 GetMovement()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    protected override bool PlaceBomb()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
