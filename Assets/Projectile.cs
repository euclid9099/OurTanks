using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TO DO: implement kill tank, 
public class Projectile : MonoBehaviour
{
    public int team = 0;

    private BoxCollider2D myCollider;
    private Rigidbody2D rb;
    public ParticleSystem projTrail;
    public ParticleSystem explosionSm;
    private int bounceCounter;
    private float speed;
    

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        this.transform.Translate(Vector2.right);
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        projTrail.Play();
        explosionSm.Pause();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        
    }

    public void SetParameters(int bounces, float velocity, Quaternion direction)
    {
        this.bounceCounter = bounces;
        this.speed = velocity;
        this.transform.rotation = direction;
    }
    
    
    void OnCollisionEnter2D(Collision2D hit)
    {
        //
        if(hit.gameObject.name.Equals("wall")){
            TurnProjectile(hit.GetContact(0).normal);
            bounceCounter = bounceCounter - 1;
        }
        else if(hit.gameObject.name.Equals("tank")){
            //first merge to get team
        }
        else if(hit.gameObject.name.Equals("Projectile(Clone)")){
            DestroyProjectile();
        }

        if(bounceCounter == 0){
            DestroyProjectile();
        }
    }

    public void DestroyProjectile() 
    {
        //Disable Projectile (=invisible), start explosion, kill proj after explosion
        this.gameObject.SetActive(false);
        this.rb.velocity = this.transform.right * 0;
        explosionSm.Play();
        Destroy(gameObject, 1.4f);
    }

    public void TurnProjectile (Vector2 mirrorAxis)
    {
        //mirror the direction Vector over the normal Vector of the obsticle
        Vector2 projDir = Vector2.Reflect(this.transform.right, mirrorAxis);

        //calculate the angle of the new Direction Vector
        float dirAngle = (180 * Mathf.Atan2(projDir.y, projDir.x) / Mathf.PI);

        if (dirAngle < 0)
        {
            dirAngle = 360 + dirAngle;
        }

        //Set the angle of the new Direction and restore speed
        this.transform.rotation = Quaternion.Euler(0,0, dirAngle);
        this.rb.velocity = this.transform.right * speed;
    }
}