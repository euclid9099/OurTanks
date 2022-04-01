using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//class to hold all relevant information for 1 tank;
public class TankData
{
    public string tankBase;
    public string tower;
    public float speed = -1;

    public int bltLimit       = -1;
    public float bltSpeed     = -1;
    public float bltAccel     = -1;
    public float bltBounces   = -1;
    public float bltSize      = -1;
                              
    public int bmbLimit       = -1;
    public float bmbExplosion = -1;
    public float bmbDetection = -1;
    public float bmbTimer     = -1;

    public TankData() { }

    public TankData(string tankBase, string tower, float speed, int bltLimit, float bltSpeed, float bltAccel, int bltBounces, float bltSize, int bmbLimit, float bmbExplosion, float bmbDetection, float bmbTimer)
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

    //returns true only if no attribute is -1
    public bool noUnset()
    {
        return (tankBase != null && tower != null &&
            bltLimit != -1 && bltSpeed != -1 && bltAccel != -1 && bltBounces != -1 && bltSize != -1 &&
            bmbLimit != -1 && bmbExplosion != -1 && bmbDetection != -1 && bmbTimer != -1);
    }
}
