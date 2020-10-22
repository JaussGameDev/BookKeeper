using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmy : Army
{
    [Header("Team")]
    [SerializeField] private Team team;
    [SerializeField] public string teamName;
    [Header("Army")]
    [SerializeField]  private List<Minion> army;
    [SerializeField] private List<Minion> alive;
    [SerializeField]  private List<Transform> MinionPosition;
    [SerializeField]  private Player player;
    [SerializeField]  private float count;
    [SerializeField] private bool toHome = false;

    [SerializeField]  private Vector3 direction;

    [SerializeField] private Army enemy = null;
    [SerializeField] private Home opponentHome = null;


    private 

    // Start is called before the first frame update
    void Awake()
    {
        player = GetComponentInChildren<Player>();
        army.AddRange(GetComponentsInChildren<Minion>());
        alive.AddRange(army);
        team = GetComponentInParent<Team>();
        teamName = team.GetTeamName();
    }

    void Start()
    {
        //add all minions positions in the army
        player.SetTeamName(teamName);
        foreach (Minion mignon in army)
        {
            mignon.SetTeamName(teamName);
        }
        MinionPosition.AddRange(player.GetPoints());
        direction = player.transform.position - MinionPosition[0].position;
        direction.Normalize();
        PlaceMinions();

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Minion mignon in army)
        {
            if (!mignon.GetAlive()) alive.Remove(mignon);
        }

        direction = player.GetSteeringDirection();

        if (enemy != null && enemy.GetAlive().Count != 0)
        {
            Debug.Log("Army, Update : first if");
            foreach (Minion mignon in army)
            {
                if (mignon.GetMode() != "Dead")
                {
                    mignon.SetMode("attack");
                    mignon.SetEnemyArmy(enemy);
                }
            }
        }
        else if (enemy != null && enemy.GetAlive().Count == 0 && enemy.IsPlayer() && enemy.GetPlayer().GetAlive())
        {
            Debug.Log("Army, Update : third if");
            foreach (Minion mignon in army)
            {
                Debug.Log("Army, Update : third if : mignon.Attack(player : = " + enemy.GetPlayer().name);
                mignon.SetMode("attack player");
                mignon.SetFuturPosition(enemy.GetPlayer().transform.position);
                mignon.Attack(enemy.GetPlayer());
                
            }

        }
        else if ( enemy != null && (!enemy.IsPlayer() || !enemy.GetPlayer().GetAlive()) )
        {
            Debug.Log("Army, Update : fourth if");
            foreach (Minion mignon in army)
            {
                mignon.SetMode("follow");
                mignon.SetEnemyArmy(null);
                mignon.FindOpponent(null);
            }
            PlaceMinions();
            SetEnemy(null);
        }
        else
        {
            Debug.Log("Army, Update : else");
            /*foreach (Minion mignon in army)
            {
                //mignon.SetMode("follow");
                mignon.SetEnemyArmy(null);
                mignon.FindOpponent(null);
            }*/
            PlaceMinions();

        }

    }
    public override string GetTeamName()
    {
        return teamName;
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


    void PlaceMinions()
    { 
        int i = 0;
        count = alive.Count;
        while (i < count)
        {
            alive[i].SetFuturPosition(MinionPosition[i%5].position + Mathf.Ceil( i / 5 ) * direction);
            alive[i].SetSteering(direction);
            i++;
        }
    }

    public override void ReplaceMinions()
    {
        int i = 0;
        count = alive.Count;
        while (i < count)
        {
            alive[i].SetFuturPosition(MinionPosition[i % 5].position + Mathf.Ceil(i / 5) * direction);
            alive[i].SetSteering(direction);
            alive[i].transform.position = MinionPosition[i % 5].position + Mathf.Ceil(i / 5) * direction;
            i++;
        }
    }

    public List<Vector3> GetMinionPositions()
    {
        var mignonPositions = new List<Vector3>();
        foreach(Minion mignon in army)
        {
            mignonPositions.Add(mignon.transform.position);
        }
        return mignonPositions;
    }

    public override void SetEnemy(Army enemyArmy)
    {
        enemy = enemyArmy;
    }
    public override List<Minion> GetArmy()
    {
        return army;
    }
    public override List<Minion> GetAlive()
    {
        return alive;
    }

    public override Player GetPlayer()
    {
        return player;
    }
    public void DeleteArmy(Army killerArmy)
    {
        Debug.Log("Army, DeleteArmy : Army = " + teamName);
        foreach (Minion minion in army)
        {
            army.Remove(minion);
            minion.SetEnemyArmy(killerArmy);
            minion.Attack(player);
        }
    }

    public override void RemoveFromArmy(Minion minion, Army killerArmy)
    {
        army.Remove(minion);
        minion.SetEnemyArmy(killerArmy);
        minion.Attack(player);
    }

    public override void SetMinionToHome()
    {
        toHome = true;
        foreach (Minion minion in alive)
        {
            minion.SetMode("home");
        }
    }
    public override void SetMinionToLeaveHome()
    {
        toHome = false;
        foreach (Minion minion in alive)
        {
            minion.SetMode("leavingHome");
        }
    }
    public override void KillAll()
    {
        player.Die();
        foreach(Minion minion in alive)
        {
            minion.Die();
        }
    }

    public override void AttackHome(Home home)
    {
        opponentHome = home;
        foreach(Minion minion in alive)
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
    public override bool IsPlayer()
    {
        return true;
    }
}
