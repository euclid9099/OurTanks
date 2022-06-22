using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//To Do: implement kill Bomb, Tank, Proj with interfaces
public class Projectile : MonoBehaviour, ProjInteraction, Hazard
{
    public Tank parent;

    private BoxCollider2D myCollider;
    private Rigidbody2D rb;
    public ParticleSystem projTrail;
    private int bounceCounter;
    private float speed;
    public float traveledDistance;


    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(this.myCollider, parent.myCollider, true);
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        traveledDistance = 0;
        projTrail.Play();
    }

    private void OnDestroy()
    {
        if (parent != null && parent.GetComponentInChildren<Tower>() != null) parent.GetComponentInChildren<Tower>().activeBullets--;
    }

void FixedUpdate()
    {
        traveledDistance += speed * Time.fixedDeltaTime;
        if (bounceCounter < parent.data.projBounces)
        {
            Physics2D.IgnoreCollision(this.myCollider, parent.myCollider, false);
        }
    }

    //gets called when a collision is detected by the Unity Collision system
    //depending of the object hit, the proj bounces or explodes
    void OnCollisionEnter2D(Collision2D hit)
    {
        Debug.Log(hit.gameObject.name);
        if (hit.gameObject.GetComponent<ProjInteraction>() != null)
        {
            if (hit.gameObject.GetComponent<ProjInteraction>().hit())
            {
                this.hit();
            } else
            {
                TurnProjectile(hit.GetContact(0).normal);
                bounceCounter = bounceCounter - 1;
            }
        } else
        {
            Physics2D.IgnoreCollision(hit.collider, this.myCollider);
            this.rb.velocity = this.transform.right * speed;
        }

        if (bounceCounter == 0)
        {
            this.hit();
        }
    }

    public void SetParameters(int bounces, float velocity, Quaternion direction, Tank parent)
    {
        this.bounceCounter = bounces;
        this.speed = velocity;
        this.transform.rotation = direction;
        this.parent = parent;
    }

    protected void Explode(float explosionRadius)
    {
        //Disable Projectile (=invisible), start explosion, kill proj after explosion
        GameObject explosion = Instantiate((GameObject)Resources.Load("Explosion"), this.transform.position, this.transform.rotation);
        ParticleSystem.MainModule main = explosion.GetComponent<ParticleSystem>().main;
        main.startSize = new ParticleSystem.MinMaxCurve(explosionRadius / 4, explosionRadius / 2);

        ParticleSystem.EmissionModule em = explosion.GetComponent<ParticleSystem>().emission;
        em.SetBurst(0, new ParticleSystem.Burst(0, new ParticleSystem.MinMaxCurve((int)(20 * explosionRadius * explosionRadius))));

        ParticleSystem.ShapeModule sh = explosion.GetComponent<ParticleSystem>().shape;
        sh.radius = explosionRadius;

        this.gameObject.SetActive(false);
        this.rb.velocity = this.transform.right * 0;
        Destroy(gameObject, 0.5f);
    }

    public void TurnProjectile(Vector2 mirrorAxis)
    {
        //mirror the direction Vector over the normal Vector of the obsticle
        Vector2 projDir = Vector2.Reflect(this.transform.right, mirrorAxis);

        //calculate the angle of the new Direction Vector
        float dirAngle = CalculateAngleFromVector2(projDir);

        //Set the angle of the new Direction and restore speed
        this.transform.rotation = Quaternion.Euler(0, 0, dirAngle);
        this.rb.velocity = this.transform.right * speed;

        // move the projectile away from the hit object so the gap between blocks leads to death
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
    }

    private float CalculateAngleFromVector2(Vector2 direction)
    {
        float dirAngle = (180 * Mathf.Atan2(direction.y, direction.x) / Mathf.PI);

        if (dirAngle < 0)
        {
            dirAngle = 360 + dirAngle;
        }

        return dirAngle;
    }

    public bool hit()
    {
        Explode(1);
        return true;
    }

    public bool bounces()
    {
        return false;
    }

    public Vector2 avoidingPath(Vector2 position)
    {
        float distance = ((Vector2)this.transform.position - position).magnitude;
        if (distance < 3)
        {
            return (position - (Vector2)this.transform.position).normalized * (1 / distance);
        }
        return Vector2.zero;
    }
}