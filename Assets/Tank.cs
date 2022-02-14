using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    void Update()
    {
        //get Input for horizontal (a,d,LEFT,RIGHT) and vertical (w,s,UP,DOWN) axis
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        //set rotation based on movement
        transform.eulerAngles = new Vector3(0,0, (float)(180 * Math.Atan2(movement.y, movement.x) / Math.PI));

        //if movement is not null, move player
        if(!movement.Equals(Vector3.zero))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
    }
}
