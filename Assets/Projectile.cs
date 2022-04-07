using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    private BoxCollider2D myCollider;
    private float traveledDistance;
    private int bounceCounter;
    private float speed;
    private char travelType;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        traveledDistance = 0;
        this.transform.Translate(Vector2.right);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate(){
        MoveProjectile();
    }

    public void SetParameters(int bounces, float velocity, char flyingType, Quaternion direction){
        this.bounceCounter = bounces;
        this.speed = velocity;
        this.travelType = flyingType;
        this.transform.rotation = direction;
    }
    
    void MoveProjectile(){
        //move the projectile form our postition to the given direction (with our defined speed per time/frame)
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
    }
    
}
