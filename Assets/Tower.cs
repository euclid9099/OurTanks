using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    Tank parent;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.PathnameToSprite(GameManager.Instance.path + "/Assets/Resources/Campaigns/" + GameManager.Instance.campaign + "/icons/" + GameManager.Instance.tanks[transform.parent.gameObject.name].tower);
    }

    // Update is called once per frame
    void Update()
    {
        //get Mousecursor (in px) and convert it in vector of game units
        Vector3 cursor = parent.GetTarget();

        //rotate the tower towards the cursor        
        this.transform.rotation = Quaternion.Euler(0,0, (float)(180 * Mathf.Atan2(cursor.y - transform.position.y, cursor.x - transform.position.x) / Mathf.PI));
        if(parent.Shoot())
        {
            ShootProjectile();
        }
        
    }

    void ShootProjectile()
    {
        //Loading Object and get copy of it; set parameters for the projectile
        GameObject projectile = Instantiate(Resources.Load<GameObject>("Projectile"),  this.transform.position, this.transform.rotation);
        Debug.Log(parent.data.projBounces);
        projectile.GetComponent<Projectile>().SetParameters(parent.data.projBounces, parent.data.projSpeed, this.transform.rotation, parent);
    }
}