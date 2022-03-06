using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    private float explosionRadius = 5;
    private float detectiondistance = 4;
    private float timer = 2;
    private Tank parent = null;
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private int spritePhase;
    
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>("BombSprites");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D[] hits;

        //if there are tanks within detection distance
        if (timer > 0.5 && (hits = Physics2D.OverlapCircleAll(this.transform.position, detectiondistance, LayerMask.GetMask("Tank"))).Length > 0)
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
        //load explosion effect
        GameObject explosion = Instantiate((GameObject)Resources.Load("Explosion"), this.transform.position, this.transform.rotation);
        var main = explosion.GetComponent<ParticleSystem>().main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(0, 2 * explosionRadius);
        main.maxParticles = (int)(20 * explosionRadius * explosionRadius);

        //kill self
        if (parent != null) parent.bombLimit += 1;
        Destroy(this.gameObject);
    }
}