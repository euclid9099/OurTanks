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
            movement *= speed * Time.deltaTime;

            //slowly rotate to fit movement
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0,0,(float)(180 * Math.Atan2(movement.y, movement.x) / Math.PI)), Time.deltaTime * 25);

            //collision detection:
            //find all collidable objects within movement range
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, myCollider.size, 0, movement, speed * Time.deltaTime, LayerMask.GetMask("Tank", "Obstruction"));

            //iterate over each hit
            foreach (RaycastHit2D hit in hits)
            {
                //first element is self, ignore
                if(!hit.Equals(hits[0]))
                {
                    //check x value of hits normal
                    if (hit.normal.x != 0)
                    {
                        //if there is a sign mismatch (e.g. hit.normal.x = -1 and movement.x = 1), there would be a collision. Decrease speed
                        if ((hit.normal.x < 0 && movement.x > 0) || (hit.normal.x > 0 && movement.x < 0))
                        {
                            movement.x *= hit.distance;
                        }
                    }
                    //same for y
                    else if (hit.normal.y != 0)
                    {
                        if ((hit.normal.y < 0 && movement.y > 0) || (hit.normal.y > 0 && movement.y < 0))
                        {
                            movement.y *= hit.distance;
                        }
                    }
                }
            }
            //translate tank by movement, after every collision has taken effect
            transform.Translate(movement, Space.World);
        }
    }
}
