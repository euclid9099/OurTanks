using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotTank : Tank
{
    private Tank enemy;
    private float timelastchanged = 0;
    private float maxtimelastchanged = 15;
    private Vector2 targetPosition = Vector2.zero;

    private float timelastshot = 0;

    public new void Start()
    {
        base.Start();
        timelastshot = (Mathf.Pow(Random.Range(0f, 1f), 2) * (Random.Range(0, 2) == 1 ? 1 : -1) * data.botProjFrequncy / 2 + data.botProjFrequncy);
    }

    public override Vector3 GetTarget()
    {
        timelastchanged += Time.deltaTime;
        if (enemy == null || Mathf.Pow(timelastchanged / maxtimelastchanged, 4) > Random.value)
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
            Debug.Log(timelastshot);
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
            if (targetPosition == Vector2.zero || (targetPosition - (Vector2)this.transform.position).sqrMagnitude < 0.25)
            {
                targetPosition = Pathfinder.FindPath(this.transform.position, enemy.transform.position);
            }
            return targetPosition - (Vector2)this.transform.position;
            /*if((enemy.transform.position - this.transform.position).sqrMagnitude > data.botPrefDistance * data.botPrefDistance)
            {
                return enemy.transform.position - this.transform.position;
            } else
            {
                return this.transform.position - enemy.transform.position;
            }/**/
        } 
        else
        {
            return Vector2.zero;
        }
    }

    protected override bool PlaceBomb()
    {
        return false;
    }
}
