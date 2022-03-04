using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    float rotationSpeed = 4;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.transform.rotation.eulerAngles);

    }

    // Update is called once per frame
    void Update()
    {
        //get Mousecursor (in px) and convert it in vector of game units
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.z = 0;
        //rotate the tower towards the cursor        
        this.transform.rotation = Quaternion.Euler(0,0, (float)(180 * Mathf.Atan2(cursor.y - transform.position.y, cursor.x - transform.position.x) / Mathf.PI));
    }
}
