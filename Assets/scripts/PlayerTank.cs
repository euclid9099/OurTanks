using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    protected override Vector2 GenerateMovement()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public override Vector3 GetTarget()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    protected override bool PlaceBomb()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override bool Shoot()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }
}
