using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Tank"))
        {
            other.gameObject.GetComponent<Tank>().Kill();
        }
    }
}
