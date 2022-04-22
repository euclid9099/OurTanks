using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    private float explosionRadius = 3;
    private float detectionDistance = 2;
    private float timer = 2;
    private Tank parent = null;

    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private int spritePhase;
    
    void Start()
    {
        //get list of bomb icons
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        sprites = MapLoader.Instance.icons["bomb"];

        //set attributes from parents tankdata
        TankData pdata = MapLoader.Instance.tanks[parent.name];
        if (pdata.noUnset())
        {
            this.explosionRadius = (float)pdata.bmbExplosion;
            this.detectionDistance = (float)pdata.bmbDetection;
            this.timer = (float)pdata.bmbTimer;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D[] hits;

        //if there are tanks within detection distance
        if (timer > 0.5 && (hits = Physics2D.OverlapCircleAll(this.transform.position, detectionDistance, LayerMask.GetMask("Tank"))).Length > 0)
        {
            //loop over every tank in range
            foreach (Collider2D hit in hits)
            {
                //if it's an enemy tank, reduce time to detection
                if (hit.gameObject.GetComponent<Tank>().team != this.parent.team)
                {
                    timer = 0.5f;
                    break;
                }
            }
        }
    }

    void Update()
    {
        //count down bomb timer
        timer -= Time.deltaTime;
        if (timer > 1)
        {
            if (sprites[(int)(2 * timer) % sprites.Length] != spriteRenderer.sprite)
            {
                spritePhase++;
                spriteRenderer.sprite = sprites[spritePhase % sprites.Length];
            }
        } else if (timer < 0)
        {
            Explode();
        } else
        {
            spritePhase++;
            spriteRenderer.sprite = sprites[spritePhase % sprites.Length];
        }
    }

    public void SetParent(Tank parent)
    {
        this.parent = parent;
    }

    private void Explode()
    {
        //load explosion effect - scale properly
        GameObject explosion = Instantiate((GameObject)Resources.Load("Explosion"), this.transform.position, this.transform.rotation);
        ParticleSystem.MainModule main = explosion.GetComponent<ParticleSystem>().main;
        main.startSize = new ParticleSystem.MinMaxCurve(explosionRadius/4,explosionRadius/2);

        ParticleSystem.EmissionModule em = explosion.GetComponent<ParticleSystem>().emission;
        em.SetBurst(0, new ParticleSystem.Burst(0,new ParticleSystem.MinMaxCurve((int)(20 * explosionRadius * explosionRadius))));

        ParticleSystem.ShapeModule sh = explosion.GetComponent<ParticleSystem>().shape;
        sh.radius = explosionRadius;

        //kill everything in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, LayerMask.GetMask("Tank", "Obstruction"));
        foreach(Collider2D hit in hits)
        {
            BombInteraction interaction = hit.gameObject.GetComponent<BombInteraction>();
            if (interaction != null)
            {
                interaction.explode();
            }
        }

        //kill self
        if (parent != null) parent.bombLimit += 1;
        Destroy(this.gameObject);
    }
}