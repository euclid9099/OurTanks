using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    private float explosionRadius = 5;
    private float detectiondistance = 4;
    private float timer = 5;
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
        //count down bomb timer
        timer -= Time.fixedDeltaTime;
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
        if (timer < 0)
        {
            explode();
        }
    }

    void Update()
    {
        if (timer > 1)
        {
            if (sprites[(int)Math.Floor(2 * timer) % sprites.Length] != spriteRenderer.sprite)
            {
                spritePhase++;
                spriteRenderer.sprite = sprites[spritePhase % sprites.Length];
            }
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

    private void explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, LayerMask.GetMask("Tank", "Obstruction"));
        foreach (Collider2D hit in hits)
        {
            Debug.Log("hit " + hit.gameObject);
        }
        if (parent != null) parent.bombLimit += 1;
        Destroy(this.gameObject);
    }
}