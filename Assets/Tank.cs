using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Tank : MonoBehaviour, BombInteraction, MovementInteraction
{
    public string team;

    public TankData data;
    private float speed = 10f;
    public BoxCollider2D myCollider;
    private Vector2 movement;
    
    public int bombLimit = 2;

    // Start is called before the first frame update
    void Start()
    {
        data = MapLoader.Instance.tanks[name];
        myCollider = GetComponent<BoxCollider2D>();

        bombLimit = data.bmbLimit;
        speed = data.speed;
    }

    void Update()
    {
        if (PlaceBomb() && bombLimit > 0)
        {
            GameObject bomb = Instantiate(Resources.Load<GameObject>("Bomb"), this.transform.position, this.transform.rotation);
            bomb.GetComponent<Bomb>().SetParent(this);
            bombLimit--;
        }
    }

    void FixedUpdate()
    {
        if (speed > 0)
        {
            //get Input for horizontal (a,d,LEFT,RIGHT) and vertical (w,s,UP,DOWN) axis
            this.movement = GenerateMovement();
            Vector2 mv_raw = this.movement;

            //if movement is not null, move player
            if (!this.movement.Equals(Vector2.zero))
            {
                //change speed to always be the same (strafing provides no benefits)
                if (this.movement.sqrMagnitude > 1)
                {
                    this.movement.Normalize();
                }
                this.movement *= speed * Time.fixedDeltaTime;

                Move(this.movement);
            }
        }
    }

    public Vector2 GetMovement() { return movement; }

    Vector2 Move(Vector2 movement)
    {
        //collision detection:
        //find all collidable objects within movement range
        RaycastHit2D[] hits = Physics2D.BoxCastAll(this.transform.position, myCollider.size, 0, movement, speed * Time.fixedDeltaTime, LayerMask.GetMask("Tank", "Obstruction"));

        //iterate over each hit
        int i = 0;
        //abs limit is/was a safety precaution, avoiding endless loops
        int abslimit = 100;
        float distance;
        List<GameObject> done = new List<GameObject>();
        while (abslimit > 0 && i < hits.Length)
        {
            if (!hits[i].collider.gameObject.Equals(this.gameObject) && !done.Contains(hits[i].collider.gameObject))
            {
                done.Add(hits[i].collider.gameObject);
                MovementInteraction interaction = hits[i].collider.gameObject.GetComponent<MovementInteraction>();

                if (interaction != null && Vector2.Angle(movement, hits[i].collider.gameObject.transform.position - transform.position) < 80) {
                    //check x value of hits normal (tells direction of collision)
                    if (hits[i].normal.x != 0 && Mathf.Abs(hits[i].point.y - this.transform.position.y) - myCollider.size.y / 2 < 0)
                    {
                        //get horizontal distance from self to whatever was hit
                        distance = hits[i].point.x - transform.position.x - ((myCollider.size.x) / 2 * Mathf.Sign(movement.x));
                        movement.x = distance + (Mathf.Abs(movement.x) > Mathf.Abs(distance) ? interaction.moveX(movement.x - distance) : movement.x - distance);

                        //redo collision with new horizontal distance
                        hits = Physics2D.BoxCastAll(transform.position, myCollider.size, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                        i = -1;
                    }
                    //same for y/vertical
                    else if (hits[i].normal.y != 0 && Mathf.Abs(hits[i].point.x - this.transform.position.x) - myCollider.size.x / 2 < 0)
                    {
                        distance = hits[i].point.y - transform.position.y - ((myCollider.size.y) / 2 * Mathf.Sign(movement.y));
                        movement.y = distance + (Mathf.Abs(movement.y) > Mathf.Abs(distance) ? interaction.moveY(movement.y - distance) : movement.y - distance);

                        hits = Physics2D.BoxCastAll(transform.position, myCollider.size, 0, movement, movement.magnitude, LayerMask.GetMask("Tank", "Obstruction"));
                        i = -1;
                    }
                }
            }
            i++;
            abslimit--;
        }
        if (abslimit == 0) Debug.Log("Limit reached");
        //translate tank by movement, after every collision has taken effect
        transform.Translate(movement, Space.World);

        return movement;
    }

    public void Kill()
    {
        GameObject d = Resources.Load<GameObject>("Death");
        d.GetComponent<SpriteRenderer>().sprite = MapLoader.Instance.icons["death"][UnityEngine.Random.Range(0, MapLoader.Instance.icons["death"].Length)];
        Instantiate(d, this.transform.position, Quaternion.Euler(0,0,0));

        MapLoader.Instance.TankDeath(this.gameObject);

        Destroy(this.gameObject);
    }

    public void SetTankData(TankData data)
    {
        this.data = data;
    }

    protected abstract Vector2 GenerateMovement();

    public abstract Vector3 GetTarget();

    protected abstract bool PlaceBomb();

    public abstract bool Shoot();

    public void explode()
    {
        Kill();
    }

    public float moveX(float movx)
    {
        if (movx == 0) return 0;
        else if (movx > 0) return Math.Min(movx / 2, Move(new Vector2(movx / 2, 0)).x);
        else return Math.Max(movx / 2, Move(new Vector2(movx / 2, 0)).x);
    }

    public float moveY(float movy)
    {
        if (movy == 0) return 0;
        else if (movy > 0) return Math.Min(movy / 2, Move(new Vector2(0, movy / 2)).y);
        else return Math.Max(movy / 2, Move(new Vector2(0, movy / 2)).y);
    }

    public float movementCost()
    {
        return 0.5f;
    }
}