using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class Tank : MonoBehaviour
{
    public int team = 0;

    private TankData data;
    private float speed = 10f;
    private BoxCollider2D myCollider;
    private Vector3 movement;
    
    public int bombLimit = 2;

    // Start is called before the first frame update
    void Start()
    {
        data = MapLoader.Instance.tanks[name];
        myCollider = GetComponent<BoxCollider2D>();

        GetComponent<SpriteRenderer>().sprite = MapLoader.PathnameToSprite(MapLoader.Instance.path + "/Assets/Resources/Campaigns/" + MapLoader.Instance.campaign + "/icons/" + data.tankBase);

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = MapLoader.PathnameToSprite(MapLoader.Instance.path + "/Assets/Resources/Campaigns/" + MapLoader.Instance.campaign + "/icons/" + data.tower);

        bombLimit = data.bmbLimit;
        speed = data.speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //get Input for horizontal (a,d,LEFT,RIGHT) and vertical (w,s,UP,DOWN) axis
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 mv_raw = movement;

        //if movement is not null, move player
        if (!movement.Equals(Vector3.zero))
        {
            //change speed to always be the same (strafing provides no benefits)
            movement.Normalize();
            movement *= speed * Time.fixedDeltaTime;

            //slowly rotate to fit movement
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0,0,(float)(180 * Math.Atan2(movement.y, movement.x) / Math.PI + (Quaternion.Angle(this.transform.rotation, Quaternion.Euler(0, 0, (float)(180 * Math.Atan2(movement.y, movement.x) / Math.PI))) < 90 ? 0 : 180))), Time.fixedDeltaTime * speed * 3);

            //collision detection:
            //find all collidable objects within movement range
            Vector2 effectiveCollision = myCollider.size * 0.9f;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(this.transform.position, effectiveCollision, 0, movement, speed * Time.fixedDeltaTime, LayerMask.GetMask("Tank", "Obstruction"));

            //iterate over each hit
            int i = 1;
            //abs limit is/was a safety precaution, avoiding endless loops
            int abslimit = 10;
            float customDistance;
            while (abslimit > 0 && i < hits.Length)
            {
                //check x value of hits normal (tells direction of collision)
                if (hits[i].normal.x != 0)
                {
                    //get horizontal distance from self to whatever was hit
                    customDistance = hits[i].point.x - transform.position.x - ((myCollider.size.x / 2) * Math.Sign(movement.x));

                    movement.x = customDistance;
                    //redo collision with new horizontal distance
                    hits = Physics2D.BoxCastAll(transform.position, effectiveCollision, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                    i = 0;
                }
                //same for y/vertical
                else if (hits[i].normal.y != 0)
                {
                    customDistance = hits[i].point.y - transform.position.y - ((myCollider.size.y / 2) * Math.Sign(movement.y));

                    movement.y = customDistance;
                    hits = Physics2D.BoxCastAll(transform.position, effectiveCollision, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                    i = 0;
                }
                i++;
                abslimit--;
            }
            if (abslimit == 0) Debug.Log("Limit reached");
            //translate tank by movement, after every collision has taken effect
            transform.Translate(movement, Space.World);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && bombLimit > 0) {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Bomb"), this.transform.position, this.transform.rotation);
            bomb.GetComponent<Bomb>().SetParent(this);
            bombLimit--;
        }
    }

    public void Kill()
    {
        GameObject d = Resources.Load<GameObject>("Death");
        d.GetComponent<SpriteRenderer>().sprite = MapLoader.Instance.icons["death"][UnityEngine.Random.Range(0, MapLoader.Instance.icons["death"].Length)];
        Instantiate(d, this.transform.position, Quaternion.Euler(0,0,0));
        Destroy(this.gameObject);
    }

    public void SetTankData(TankData data)
    {
        this.data = data;
    }
}
