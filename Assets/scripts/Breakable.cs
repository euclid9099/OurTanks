using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, BombInteraction
{
    public void explode()
    {
        //load explosion effect
        GameObject explosion = Instantiate((GameObject)Resources.Load("Explosion"), this.transform.position, this.transform.rotation);
        ParticleSystem system = explosion.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = system.main;
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.5f);


        ParticleSystem.EmissionModule em = system.emission;
        em.SetBurst(0, new ParticleSystem.Burst(0, new ParticleSystem.MinMaxCurve(20)));

        ParticleSystem.ShapeModule sh = system.shape;
        sh.radius = 1;

        explosion.GetComponent<ParticleSystemRenderer>().material.mainTexture = GetComponent<SpriteRenderer>().sprite.texture;

        Destroy(this.gameObject);
    }
}
