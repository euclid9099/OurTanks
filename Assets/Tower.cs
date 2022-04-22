using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int projectileBounces = 2;
    private float projectileVelocity = 25f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //get Mousecursor (in px) and convert it in vector of game units
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //rotate the tower towards the cursor        
        this.transform.rotation = Quaternion.Euler(0,0, (float)(180 * Mathf.Atan2(cursor.y - transform.position.y, cursor.x - transform.position.x) / Mathf.PI));
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootProjectile();
        }
        
    }

    void ShootProjectile()
    {
        //Loading Object and get copy of it; set parameters for the projectile
        GameObject projectile = Instantiate(Resources.Load<GameObject>("Projectile"),  this.transform.position, this.transform.rotation);
        projectile.GetComponent<Projectile>().SetParameters(projectileBounces, projectileVelocity, this.transform.rotation, this.GetComponentInParent<Tank>());
    }
}
