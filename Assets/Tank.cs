using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class Tank : MonoBehaviour
{
    private float speed = 10f;
    private BoxCollider2D myCollider;
    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //get Input for horizontal (a,d,LEFT,RIGHT) and vertical (w,s,UP,DOWN) axis
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        //if movement is not null, move player
        if (!movement.Equals(Vector3.zero))
        {
            //change speed to always be the same (strafing provides no benefits)
            movement.Normalize();
            movement *= speed * Time.fixedDeltaTime;

            //slowly rotate to fit movement
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0,0,(float)(180 * Math.Atan2(movement.y, movement.x) / Math.PI)), Time.fixedDeltaTime * speed * 3);

            //collision detection:
            //find all collidable objects within movement range
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, myCollider.size * 0.95f, 0, movement, speed * Time.fixedDeltaTime, LayerMask.GetMask("Tank", "Obstruction"));

            //iterate over each hit
            int i = 1;
            //abs limit is/was a safety precaution, avoiding endless loops
            int abslimit = 10;
            float customDistance;
            while (abslimit > 0 && i < hits.Length)
            {
                //check x value of hits normal (tells direction of collision)
                if (hits[i].normal.x != 0)
                {
                    //get horizontal distance from self to whatever was hit
                    customDistance = hits[i].point.x - transform.position.x - ((myCollider.size.x / 2) * (movement.x / Math.Abs(movement.x)));

                    movement.x = customDistance;
                    //redo collision with new horizontal distance
                    hits = Physics2D.BoxCastAll(transform.position, myCollider.size * 0.95f, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                    i = 0;
                }
                //same for y/vertical
                else if (hits[i].normal.y != 0)
                {
                    customDistance = hits[i].point.y - transform.position.y - ((myCollider.size.y / 2) * (movement.y / Math.Abs(movement.y)));

                    movement.y = customDistance;
                    hits = Physics2D.BoxCastAll(transform.position, myCollider.size * 0.95f, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                    i = 0;
                }
                i++;
                abslimit--;
            }
            if (abslimit == 0) Debug.Log("Limit reached");
            //translate tank by movement, after every collision has taken effect
            transform.Translate(movement, Space.World);
        }
    }
}
