using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TO DO: implement kill tank, 
public class Projectile : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        traveledDistance += speed * Time.fixedDeltaTime;
        if(bounceCounter < parent.projectileBounces)
        {
            Physics2D.IgnoreCollision(this.myCollider, parent.myCollider, false);
        }
    }

    public void SetParameters(int bounces, float velocity, Quaternion direction, Tank parent)
    {
        this.bounceCounter = bounces;
        this.speed = velocity;
        this.transform.rotation = direction;
        this.parent = parent;
    }
    
    
    void OnCollisionEnter2D(Collision2D hit)
    {
        Debug.Log(hit.gameObject.name);
        if(hit.gameObject.name.Equals("wall"))
        {
            TurnProjectile(hit.GetContact(0).normal);
            bounceCounter = bounceCounter - 1;
        }
        else if(hit.gameObject.GetComponent<Tank>() != null)
        {
            Tank tank = hit.gameObject.GetComponent<Tank>();
            tank.Kill();
            DestroyProjectile(1.5f);
        }
        else if(hit.gameObject.name.Equals("Projectile(Clone)"))
        {
            DestroyProjectile(1);
        }

        if(bounceCounter == 0){
            DestroyProjectile(1);
        }
    }

    //howTarget returns if the Projectile points towards the target or away from it
    public void SpawnProtectionBridge()
    {
        RaycastHit2D[] obstacles = Physics2D.RaycastAll(this.transform.position, this.transform.right, 1.7f);

        for (int i = 0; i < obstacles.Length; i++)
        {
            String name = obstacles[i].collider.gameObject.name;
            if(name.Equals("Player"))
            {
                if(obstacles[i].collider.gameObject.GetComponent<Tank>().Equals(parent)){
                    obstacles[i].collider.gameObject.GetComponent<Tank>().Kill();
                    DestroyProjectile(1.5f);
                    break;
                }
            }else if(name.Equals("wall"))
            {
                parent.Kill();
                DestroyProjectile(1.5f);
                break;
            }
        }
    }

    public void DestroyProjectile(float explosionRadius) 
    {
        //Disable Projectile (=invisible), start explosion, kill proj after explosion
        GameObject explosion = Instantiate((GameObject)Resources.Load("Explosion"), this.transform.position, this.transform.rotation);
        ParticleSystem.MainModule main = explosion.GetComponent<ParticleSystem>().main;
        main.startSize = new ParticleSystem.MinMaxCurve(explosionRadius/4,explosionRadius/2);

        ParticleSystem.EmissionModule em = explosion.GetComponent<ParticleSystem>().emission;
        em.SetBurst(0, new ParticleSystem.Burst(0,new ParticleSystem.MinMaxCurve((int)(20 * explosionRadius * explosionRadius))));

        ParticleSystem.ShapeModule sh = explosion.GetComponent<ParticleSystem>().shape;
        sh.radius = explosionRadius;
        
        this.gameObject.SetActive(false);
        this.rb.velocity = this.transform.right * 0;
        Destroy(gameObject, 1.4f);
    }

    public void TurnProjectile (Vector2 mirrorAxis)
    {
        //mirror the direction Vector over the normal Vector of the obsticle
        Vector2 projDir = Vector2.Reflect(this.transform.right, mirrorAxis);

        //calculate the angle of the new Direction Vector
        float dirAngle = CalculateAngleFromVector2(projDir);

        //Set the angle of the new Direction and restore speed
        this.transform.rotation = Quaternion.Euler(0,0, dirAngle);
        this.rb.velocity = this.transform.right * speed;
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
}