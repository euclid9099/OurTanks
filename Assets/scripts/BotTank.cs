using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotTank : Tank
{
    private Tank enemy;
    private float timelastchanged = 0;
    private Vector2 targetPosition = Vector2.zero;

    private float timelastshot = 0;
    private float timelastbombed = 0;
    private float randomoffset = 0;

    public new void Start()
    {
        base.Start();
        timelastshot = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botProjFrequncy / 2 + data.botProjFrequncy);
        timelastbombed = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botBmbFrequncy / 2 + data.botBmbFrequncy);
        timelastchanged = Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botPositionFocus / 2 + data.botPositionFocus;
        if (data.speed != 0 && data.botPathfinding != 0) InvokeRepeating("FindPath", 0f, 0.5f);
        randomoffset = Random.Range(0, 10000);
    }

    public override Vector3 GetTarget()
    {
        timelastchanged -= Time.deltaTime;
        if (enemy == null || timelastchanged < 0)
        {
            timelastchanged = Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botPositionFocus / 2 + data.botPositionFocus;
            string[] opposingTeams = GameManager.Instance.teams.Keys.Where(e => !e.Equals(team)).ToArray();

            if (opposingTeams.Length > 0)
            {
                GameObject[] opposingTeam = GameManager.Instance.teams[opposingTeams[Random.Range(0, opposingTeams.Length)]].ToArray();
                
                enemy = opposingTeam[Random.Range(0, opposingTeam.Length)].GetComponent<Tank>();
            }
        }
        return enemy == null ? Vector3.zero : enemy.transform.position;
    }

    public override bool Shoot()
    {
        if (data.botProjFrequncy != 0 && timelastshot < 0)
        {
            timelastshot = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0,2) == 1 ? 1 : -1) * data.botProjFrequncy / 2 + data.botProjFrequncy);
            return Physics2D.LinecastAll(enemy.transform.position, this.transform.position).Where(e => e.collider.gameObject.GetComponent<ProjInteraction>() != null).Count() == 2;
        } else
        {
            timelastshot -= Time.deltaTime;
            return false;
        }
    }

    protected override Vector2 GenerateMovement()
    {
        //very simple movement, just move directly to target
        if (enemy != null)
        {
            Vector2 movement = data.botPathfinding == 0 ? Vector2.zero : targetPosition - (Vector2)this.transform.position;
            if (data.botPathfinding != 0 && movement.sqrMagnitude < data.speed * data.speed * Time.deltaTime * Time.deltaTime)
            {
                FindPath();
            }
            Vector2 randommove = new Vector2((Mathf.PerlinNoise(Time.time + randomoffset, 0) - 0.5f) * 2, (Mathf.PerlinNoise(0, Time.time + randomoffset) - 0.5f) * 2) * data.botRandomness;
            Vector2 focusmove = movement.normalized * data.botPathfinding;
            Vector2 fearmove = Vector2.zero;
            foreach (GameObject go in FindObjectsOfType<GameObject>())
            {
                Hazard hazard = go.GetComponent<Hazard>();
                if (hazard != null)
                    {
                    Debug.DrawRay(this.transform.position, hazard.avoidingPath(this.transform.position), Color.red);
                    fearmove += hazard.avoidingPath(this.transform.position);
                }
            }/*
+            fearmove = fearmove / detDistance * data.botFear;
+            Debug.DrawRay(this.transform.position, focusmove, Color.green);
+            Debug.DrawRay(this.transform.position + (Vector3)focusmove, randommove, Color.yellow);
+            Debug.DrawRay(this.transform.position + (Vector3)focusmove + (Vector3)randommove, fearmove, Color.red);
+            Debug.DrawRay(this.transform.position, (focusmove + randommove + fearmove) * 5, Color.white);/**/
            Debug.DrawRay(this.transform.position, fearmove, Color.green);
            return (randommove + focusmove + fearmove).normalized;
        } 
        else
        {
            return Vector2.zero;
        }
    }

    private void FindPath()
    {
        if (enemy != null)
        {
            targetPosition = Pathfinder.FindPath(this.transform.position, enemy.transform.position);
        }
        else targetPosition = Vector2.zero;
    }

    protected override bool PlaceBomb()
    {
        if (data.botBmbFrequncy != 0 && timelastbombed < 0)
        {
            timelastbombed = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botBmbFrequncy / 2 + data.botBmbFrequncy);
            return true;
        }
        else
        {
            timelastbombed -= Time.deltaTime;
            return false;
        }
    }
}
