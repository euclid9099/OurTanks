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

    public int projLimit       = -1;
    public float projSpeed     = -1;
    public int projBounces   = -1;
                              
    public int bmbLimit       = -1;
    public float bmbExplosion = -1;
    public float bmbDetection = -1;
    public float bmbTimer     = -1;

    public float botProjFrequncy = -1;
    public int botCalcRebounds   = -1;
    public float botBmbFrequncy  = -1;
    public float botPrefDistance = -1;
    public float botPositionFocus= -1;

    public TankData() { }

    public TankData(string tankBase, string tower, float speed, int projLimit, float projSpeed, int projBounces, int bmbLimit, float bmbExplosion, float bmbDetection, float bmbTimer)
    {
        this.tankBase = tankBase;
        this.tower = tower;
        this.speed = speed;

        this.projLimit = projLimit;
        this.projSpeed = projSpeed;
        this.projBounces = projBounces;

        this.bmbLimit = bmbLimit;
        this.bmbExplosion = bmbExplosion;
        this.bmbDetection = bmbDetection;
        this.bmbTimer = bmbTimer;
    }

    //returns true only if no attribute is -1
    public bool noUnset()
    {
        return (tankBase != null && tower != null &&
            projLimit != -1 && projSpeed != -1 && projBounces != -1 &&
            bmbLimit != -1 && bmbExplosion != -1 && bmbDetection != -1 && bmbTimer != -1);
    }

    public bool noUnsetBot()
    {
        return (noUnset() && botProjFrequncy != -1 && botCalcRebounds != -1 && botBmbFrequncy != -1 && botPrefDistance != -1 && botPositionFocus != -1);
    }
}
