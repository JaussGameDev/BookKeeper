using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaArmy : Army
{
    [SerializeField] private string teamName;
    [SerializeField] private int nbArmyKilled = 0;
    [Header("Army")]
    [SerializeField] private List<Minion> army;
    [SerializeField] private List<Minion> alive;
    [SerializeField] private List<Transform> MinionPosition;
    [Header("Position")]
    [SerializeField] private Vector3 direction;
    [SerializeField] private Transform followPoint;
    [SerializeField] private Army enemy = null;
    [SerializeField] private Home opponentHome = null;

    private float time = 0f;

    private void Awake()
    {
        followPoint = new GameObject("followPoint").transform;
        followPoint.parent = transform.Find("IaArmy");

        SetPoints();
        SetDirection(new Vector3 (1, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Minion mignon in army)
        {
            if (!mignon.GetAlive()) alive.Remove(mignon);
        }

        /*foreach (Minion mignon in alive)
        {
            //Attack if opponent closed enought
        }*/
    }

        private void FixedUpdate()
    {
        time += 0.5f * Time.deltaTime;
        followPoint.position = new Vector3(Mathf.Cos(time) * 10f, 0, Mathf.Sin(time) * 10f);
        followPoint.rotation = MinionPosition[0].rotation;
        ReplaceMinions();
    }
    public override List<Minion> GetAlive()
    {
        return alive;
    }

    public override Player GetPlayer()
    {
        return null;
    }
    public override string GetTeamName()
    {
        return teamName;
    }
    public override void RemoveFromArmy(Minion minion, Army killerArmy)
    {
        army.Remove(minion);
        minion.SetEnemyArmy(killerArmy);
    }

    public void AddKill()
    {
        nbArmyKilled ++;
    }

    public override void SetMinionToHome()
    {
        return;
    }
    public override void SetMinionToLeaveHome()
    {
        return;
    }

    public override void ReplaceMinions()
    {
        int i = 0;
        var count = alive.Count;
        while (i < count)
        {
            army[i].SetFuturPosition(MinionPosition[i % 5].position + Mathf.Ceil(i / 5) * direction + followPoint.position);
            army[i].SetSteering(followPoint.position - MinionPosition[0].position);
            i++;
        }
    }

    public override void Resurect(List<Minion> minions)
    {
        var mode = alive[0].GetMode();
        var enemyArmy = new List<Minion>();
        enemyArmy.AddRange(minions);
        foreach (Minion mignon in enemyArmy)
        {
            mignon.GetArmy().RemoveFromArmy(mignon, mignon.GetArmy());
            mignon.transform.parent = transform.Find("Minions");
            mignon.Revive(this, mode);
            army.Add(mignon);
            alive.Add(mignon);
        }
    }
    public override void KillAll()
    {
        foreach (Minion minion in alive)
        {
            minion.Die();
        }
    }
    public override void SetEnemy(Army enemyArmy)
    {
        enemy = enemyArmy;
    }
    public override List<Minion> GetArmy()
    {
        return army;
    }

    public override void AttackHome(Home home)
    {
        opponentHome = home;
        foreach (Minion minion in alive)
        {
            minion.opponentHome = opponentHome;
            minion.Attack(opponentHome);
            minion.SetMode("attack home");
        }
    }
    public override void SetMode(string mode)
    {
        foreach (Minion minion in alive)
        {
            minion.SetMode(mode);
        }
    }
    public void AddMinions(List<Minion> minions)
    {
        army.AddRange(minions);
        alive.AddRange(minions);
    }

    public void SetPoints()
    {
        Debug.Log("PLayer, SetPoints : transform.rotation.y =" + transform.eulerAngles.y);
        var playerRot = transform.eulerAngles;
        transform.Rotate(-playerRot);

        //Create 5 points in front of the player
        var maxLeft = new GameObject("maxLeft").transform;
        maxLeft.position = new Vector3(transform.position.x - 3f, transform.position.y, transform.position.z + 2.5f);
        maxLeft.parent = transform.Find("armyPoint");

        var maxRight = new GameObject("maxRight").transform;
        maxRight.position = new Vector3(transform.position.x + 3f, transform.position.y, transform.position.z + 2.5f);
        maxRight.parent = transform.Find("armyPoint");

        var inFront = new GameObject("inFront").transform;
        inFront.position = (maxLeft.position + maxRight.position) / 2;
        inFront.parent = transform.Find("armyPoint");

        var midRight = new GameObject("midRight").transform;
        midRight.position = inFront.position + (maxRight.position - inFront.position) / 2f;
        midRight.parent = transform.Find("armyPoint");

        var midLeft = new GameObject("midLeft").transform;
        midLeft.position = inFront.position + (maxLeft.position - inFront.position) / 2f;
        midLeft.parent = transform.Find("armyPoint");

        MinionPosition.AddRange(new Transform[] { inFront, midRight, midLeft, maxRight, maxLeft });

        transform.Rotate(playerRot);

    }

    public override bool IsPlayer()
    {
        return false;
    }
    public void SetDirection(Vector3 value)
    {
        direction = value;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position if point exist
        foreach (Transform point in MinionPosition)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(point.position + Vector3.down, new Vector3(1, 1, 1));
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(followPoint.position, 0.8f);

    }
}
