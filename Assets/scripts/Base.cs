using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = MapLoader.PathnameToSprite(MapLoader.Instance.path + "/Assets/Resources/Campaigns/" + MapLoader.Instance.campaign + "/icons/" + MapLoader.Instance.tanks[transform.parent.gameObject.name].tankBase);
    }

    // Update is called once per frame
    void Update()
    {
        //slowly rotate to fit movement
        Vector2 movement = GetComponentInParent<Tank>().GetMovement();
        if (!movement.Equals(Vector2.zero))
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, 0, (float)(180 * Mathf.Atan2(movement.y, movement.x) / Mathf.PI + (Quaternion.Angle(this.transform.rotation, Quaternion.Euler(0, 0, (float)(180 * Mathf.Atan2(movement.y, movement.x) / Mathf.PI))) < 90 ? 0 : 180))), Time.deltaTime * 30);
        }
    }
}
