using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
        if (other.gameObject.CompareTag("Tank"))
        {
            other.gameObject.GetComponent<Tank>().Kill();
        }
    }
}
