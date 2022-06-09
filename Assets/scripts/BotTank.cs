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

    public new void Start()
    {
        base.Start();
        timelastshot = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botProjFrequncy / 2 + data.botProjFrequncy);
        timelastchanged = Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botPositionFocus / 2 + data.botPositionFocus;
        if(data.speed != 0 && data.botAggression != 0) InvokeRepeating("FindPath", 0f, 0.5f);
    }

    public override Vector3 GetTarget()
    {
        timelastchanged -= Time.deltaTime;
        if (enemy == null || timelastchanged < 0)
        {
            timelastchanged = 0;
            string[] opposingTeams = GameManager.Instance.teams.Keys.Where(e => !e.Equals(team)).ToArray();

            GameObject[] opposingTeam = GameManager.Instance.teams[opposingTeams[Random.Range(0, opposingTeams.Length)]].ToArray();

            enemy = opposingTeam[Random.Range(0, opposingTeam.Length)].GetComponent<Tank>();
        }
        return enemy.transform.position;
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
            Vector2 movement = data.botAggression == 0 ? Vector2.zero : targetPosition - (Vector2)this.transform.position;
            if (data.botAggression != 0 && movement.sqrMagnitude < data.speed * data.speed * Time.deltaTime * Time.deltaTime)
            {
                FindPath();
            }
            return (new Vector2((Mathf.PerlinNoise(Time.time, 0) - 0.5f) * 2 * data.speed, (Mathf.PerlinNoise(0, Time.time) - 0.5f) * 2 * data.speed)) * (Mathf.Abs(data.botAggression) - 1) + movement * data.speed * data.botAggression;
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
        return false;
    }
}
