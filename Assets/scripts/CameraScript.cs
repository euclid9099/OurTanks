using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject target;
    public int horizontalSaveDistance;
    public int verticalSaveDistance;

    // Update is called once per frame
    void Update()
    {
        if (target != null && GameManager.Instance != null && GameManager.Instance.levelsize[0] != 0)
        {

            //initiate with -1 (never valid)
            float targetX;
            float targetY;

            //find horizontal target position
            if (2 * horizontalSaveDistance > GameManager.Instance.levelsize[0])
            {
                targetX = (GameManager.Instance.levelsize[0] - 1) / 2f;
            } else
            {
                if (target.transform.position.x > GameManager.Instance.levelsize[0] - horizontalSaveDistance - 1)
                {
                    targetX = GameManager.Instance.levelsize[0] - 1 - horizontalSaveDistance;
                }
                else if (target.transform.position.x < horizontalSaveDistance)
                {
                    targetX = horizontalSaveDistance;
                }
                else
                {
                    targetX = target.transform.position.x;
                }
            }

            //find vertical target position
            if (2 * verticalSaveDistance > GameManager.Instance.levelsize[1])
            {
                targetY = -(GameManager.Instance.levelsize[1] - 1) / 2.0f;
            } else
            {
                if (target.transform.position.y < -GameManager.Instance.levelsize[1] + verticalSaveDistance + 1)
                {
                    targetY = -(GameManager.Instance.levelsize[1] - 1 - verticalSaveDistance);
                }
                else if (target.transform.position.y > -verticalSaveDistance) {
                    targetY = -verticalSaveDistance;
                }
                else
                {
                    targetY = target.transform.position.y;
                }
            }

            //move object on screen
            this.transform.position = new Vector3(targetX, targetY, -10);
        }
    }
}
