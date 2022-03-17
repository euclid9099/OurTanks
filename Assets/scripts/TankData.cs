using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to hold all relevant information for 1 tank;
public class TankData
{
    public Sprite tankBase;
    public Sprite tower;
    public float? speed;

    public int? bltLimit;
    public float? bltSpeed;
    public float? bltAccel;
    public float? bltBounces;
    public float? bltSize;

    public int? bmbLimit;
    public float? bmbExplosion;
    public float? bmbDetection;
    public float? bmbTimer;

    public TankData() { }

    public TankData(Sprite tankBase, Sprite tower, float speed, int bltLimit, float bltSpeed, float bltAccel, int bltBounces, float bltSize, int bmbLimit, float bmbExplosion, float bmbDetection, float bmbTimer)
    {
        this.tankBase = tankBase;
        this.tower = tower;
        this.speed = speed;

        this.bltLimit = bltLimit;
        this.bltSpeed = bltSpeed;
        this.bltAccel = bltAccel;
        this.bltBounces = bltBounces;
        this.bltSize = bltSpeed;

        this.bmbLimit = bmbLimit;
        this.bmbExplosion = bmbExplosion;
        this.bmbDetection = bmbDetection;
        this.bmbTimer = bmbTimer;
    }

    //returns true only if no attribute is null
    public bool noNull()
    {
        return (tankBase != null && tower != null && tower != null &&
            bltLimit != null && bltSpeed != null && bltAccel != null && bltBounces != null && bltSize != null &&
            bmbLimit != null && bmbExplosion != null && bmbDetection != null && bmbTimer != null);
    }
}
