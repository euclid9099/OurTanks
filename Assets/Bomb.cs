using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float explosionRadius;
    private float detectiondistance = 2;
    private float timer = 3;
    private Tank parent = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer > 0.5 && Physics2D.CircleCastAll(this.transform.position, detectiondistance, Vector2.zero, 0, LayerMask.GetMask("Tank")).Length > 0)
        {
            //timer = 0.5f;
        }
        if (timer < 0)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, LayerMask.GetMask("Tank"));
            foreach(Collider2D hit in hits)
            {
                Debug.Log("hit " + hit.gameObject);
            }
            parent.bombLimit += 1;
            Destroy(this.gameObject);
        }
    }

    public void SetParent(Tank parent)
    {
        this.parent = parent;
    }
}
