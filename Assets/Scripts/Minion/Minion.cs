using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Minion : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Army army;
    [SerializeField] private MinionMotor motor = null;
    [SerializeField] private CapsuleCollider collider = null;
    [Header("Visual")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private List<Material> materials;
    [SerializeField] private GameObject visual;
    [SerializeField] private Renderer bodyRend;
    [SerializeField] private Renderer shirtRend;
    [SerializeField] public ParticleSystem resurectParticles;
    [Header("Team")]
    [SerializeField] private string teamName;
    [SerializeField] private Army enemyArmy;
    [SerializeField] private Player enemyPlayer;
    [SerializeField] private Team team;
    [SerializeField] private Home home;
    [SerializeField] private bool isHome = false;
    [Header("Infos")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private string mode = "follow";
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float health = 200f;
    [SerializeField] private float SPEED = 0.3f;
    [Header("Combat")]
    [SerializeField] private bool attack = false;
    [SerializeField] private int killNumber = 0;
    [SerializeField] private int deathNumber = 0;
    [SerializeField] private float damage = 1f;
    [SerializeField] private List<Vector3> enemyPositions;
    [SerializeField] private List<Minion> enemy;
    [SerializeField] private Minion opponent;
    [SerializeField] public Home opponentHome = null;
    [Header("Position")]
    [SerializeField] private Vector3 futurPosition;

    private void Awake()
    {
        resurectParticles = GetComponentInChildren<ParticleSystem>();
        resurectParticles.Stop();
        motor = GetComponent<MinionMotor>();
        visual = transform.Find("MinionVisual").gameObject;
        bodyRend = visual.transform.Find("Body").GetComponent<Renderer>();
        shirtRend = visual.transform.Find("Shirt").GetComponent<Renderer>();
        bodyRend.material = GetMaterialByString("Minion" + 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        army = GetComponentInParent<Army>();
        team = army.GetComponentInParent<Team>();
        home = team.GetComponentInChildren<Home>();
        collider = GetComponent<CapsuleCollider>();
        animator = visual.GetComponent<Animator>();
        animator.enabled = true;
    }
    void FixedUpdate()
    {
        if (mode == "Dead") return;
        if (!attack)
        {
            var value = motor.agent.velocity.magnitude > 1f;
            animator.SetBool("IsMoving", value);
        }
        else animator.SetBool("IsMoving", false);

        if (mode == "follow")
        {
            attack = false;
            motor.MoveToPoint(futurPosition);
            animator.SetBool("isAttacking", false);
        }
        if (mode == "attack")
        {
            var distance = Vector3.Distance(transform.position, opponent.transform.position);
            if (distance > 2f) motor.MoveToPoint(opponent.transform.position);
            if (distance <= 2f) Stop();
            Attack();
        }
        if (mode == "attack player" && enemyPlayer != null && enemyArmy != null)
        {
            enemyPlayer = enemyArmy.GetPlayer();
            Debug.Log($"Minion, FixedUpdate : Minion : {gameObject.name}, enemyArmy : {enemyPlayer}");
            var distance = Vector3.Distance(transform.position, enemyPlayer.transform.position);
            if (distance > 2f) motor.MoveToPoint(enemyPlayer.transform.position);
            if (distance <= 2f) Stop();
            Attack(enemyPlayer);
        }
        if (mode == "attack home")
        {
            Debug.Log($"Minion, FixedUpdate : Minion : {gameObject.name}, enemyMode : {enemyArmy}");
            var distance = Vector3.Distance(transform.position, opponentHome.transform.position);
            if (distance > 4f) motor.MoveToPoint(opponentHome.transform.position);
            if (distance <= 4f) Stop();
            Attack(opponentHome);
        }
        if (mode == "home")
        {
            if ((transform.position - home.transform.position).magnitude > 4f) motor.MoveToPoint(futurPosition);
            CheckHomeDistance();
        }
        if (mode == "leavingHome")
        {
            if ((transform.position - home.transform.position).magnitude > 4f)
            {
                SetMode("follow");
                GetOutGHome();
            }
        }

    }
    private void Stop()
    {
        SetFuturPosition(transform.position);
        motor.MoveToPoint(futurPosition);
    }

    private void CheckHomeDistance()
    {
        if (!isHome && ( transform.position - home.transform.position).magnitude < 4f) GetInGHome();
    }
    private void GetInGHome()
    {
        DisableVisual();
        //transform.RotateAround(home.transform.position, Vector3.up, 180f);
        transform.position = home.transform.position;
        Stop();
        isHome = true;
        home.AddMinion(this);
        motor.SetAgentRadius(0.00000000000001f);
        collider.radius = 0.00000001f;
        collider.height = 0.00000001f;
    }
    private void GetOutGHome()
    {
        EnableVisual();
        isHome = false;
        home.RemoveMinion(this);
        motor.SetAgentRadius(0.5f);
        collider.radius = 0.5f;
        collider.height = 2f;
    }

    private void DisableVisual()
    {
        visual.gameObject.SetActive(false);
    }
    private void EnableVisual()
    {
        visual.gameObject.SetActive(true);
    }

    public bool GetAlive()
    {
        return isAlive;
    }
    public void SetAlive(bool alive)
    {
        isAlive = alive;
    }
    public void SetArmy(Army value)
    {
        army = value;
    }
    public void SetTeam(Team value)
    {
        team = value;
    }
    public void SetHome(Home value)
    {
        home = value;
    }

    public void SetFuturPosition(Vector3 pos)
    {
        if (!isAlive) futurPosition = transform.position;
        if (isAlive) futurPosition = pos;
    }
    public void SetSteering(Vector3 dir)
    {
        if (!isAlive) motor.SetSteering(transform.forward);
        if (isAlive) motor.SetSteering(dir);
    }

    public void SetTeamName(string name)
    {
        teamName = name;
        //change minion color
        shirtRend.material = GetMaterialByString(teamName);
    }

    public string GetTeamName()
    {
        return teamName;
    }
    public bool GetHit(float damage)
    {
        Debug.Log($"Minion, GetHit : {gameObject.name} get hit {damage}");

        if (damage < health)
        {
            RemoveHealth(damage);
            return true;
        }
        else
        {
            RemoveHealth(health);
            Die();
            return false;
        }
    }
    public void Die()
    {
        SetMode("Dead");
        SetAlive(false);
        Debug.Log($"Minion, Die : {gameObject.name}");
        deathNumber++;
        if (deathNumber <= 4) bodyRend.material = GetMaterialByString("Minion" + deathNumber);

        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAlive", false);
        animator.CrossFade("Die", .5f, -1, 0.0f);
        collider.radius = 2f;
        collider.height = 3f;
    }
    public void SetMode(string str)
    {
        mode = str;
    }
    public string GetMode()
    {
        return mode;
    }

    public void Attack()
    {
        SetSteering((opponent.transform.position - transform.position).normalized);
        motor.SteerAt(opponent.transform);
        var distance = Vector3.Distance(transform.position, opponent.transform.position);
        if (distance <= 2f && opponent.GetHealth() > 0)
        {
            animator.SetBool("isAttacking", true);
            attack = true;
            Debug.Log($"Minion, Attack : {gameObject.name} hit {opponent.gameObject.name}");
            if (!opponent.GetHit(damage)) killNumber++;
        }
        else
        {
            attack = false;
            animator.SetBool("isAttacking", false);
        }
    }
    public void Attack(Player player)
    {
        SetSteering((player.transform.position - transform.position).normalized);
        motor.SteerAt(player.transform);
        Debug.Log($"Minion, Attack : {gameObject.name} attack {player.gameObject.name}");
        var distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= 2f && player.GetHealth() > 0)
        {
            animator.SetBool("isAttacking", true);
            attack = true;
            Debug.Log($"Minion, Attack : {gameObject.name} hit {player.gameObject.name}");
            if (!player.GetHit(damage)) killNumber++;
        }
        else
        {
            attack = false;
            animator.SetBool("isAttacking", false);
        }
    }
    public void Attack(Home home)
    {
        SetSteering((home.transform.position - transform.position).normalized);
        motor.SteerAt(home.transform);
        Debug.Log($"Minion, Attack : {gameObject.name} attack {home.gameObject.name}");
        var distance = Vector3.Distance(transform.position, home.transform.position);
        if (distance <= 4f && home.GetHealth() > 0)
        {
            animator.SetBool("isAttacking", true);
            attack = true;
            Debug.Log($"Minion, Attack : {gameObject.name} hit {home.gameObject.name}");
            home.RemoveHealth(damage);
            if (!home.RemoveHealth(damage)) team.AddKill();
        }
        else
        {
            attack = false;
            animator.SetBool("isAttacking", false);
        }
    }
    public void SetEnemyArmy(Army enemies)
    {
            enemyArmy = enemies;
        if (enemyArmy != null && enemyArmy.GetAlive().Count > 0)
        {
            Debug.Log("Minion, SetEnemyArmy :  alive count = " + enemyArmy.GetAlive().Count);
            SetEnemies(enemyArmy.GetAlive());
        } else if (enemyArmy != null) Debug.Log("___________Quelque chose ici"); 
    }
    public void SetEnemies(List<Minion> mignons)
    {
        enemy = mignons;
        //enemy.Add(enemyArmy.GetPlayer());
        var positions = new List<Vector3>();
        foreach (Minion mignon in enemy)
        {
            positions.Add(mignon.transform.position);
        }
        SetEnemyPositions(positions);
    }
    public void SetEnemyPositions(List<Vector3> positions)
    {
        enemyPositions = positions;
        //enemyPositions.Add(enemyArmy.GetPlayer().transform.position);
        FindOpponent(enemyPositions);
    }
    public void FindOpponent(List<Vector3> enemies)
    {
        if (enemies == null) { opponent = null; return; }
        var distances = new List<float>();
        foreach (Vector3 pos in enemies)
        {
            distances.Add(Vector3.Distance(pos, transform.position));
        }
        var nb = distances.Min();
        int index = distances.IndexOf(nb);
        opponent = enemy[index];
    }
    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
    }
    public float GetHealth()
    {
        return health;
    }
    public void ResetHealth()
    {
        health = maxHealth;
    }

    private void SetHealth(float amount)
    {
        health = amount;
    }
    public bool RemoveHealth(float amount)
    {
        Debug.Log($"Minion, RemoveHealth : {gameObject.name} loss {amount} pv");
        if (amount > GetHealth())
        {
            SetHealth(GetHealth() - amount);
            return true;
        }
        else
        {
            SetHealth(GetHealth() - amount);
            return false;

        }

    }
    public bool GetIsHome()
    {
        return isHome;
    }
    public Army GetArmy()
    {
        return army;
    }

    public void Revive(Army army, string mode)
    {
        Debug.Log($"Minion, Revive : {gameObject.name} revive");

        animator.SetBool("IsAlive", true);
        collider.radius = 0.5f;
        collider.height = 2f;
        SetTeamName(army.GetTeamName());
        SetAlive(true);
        SetArmy(army);
        SetTeam(army.GetComponentInParent<Team>());
        SetHome(team.GetComponentInChildren<Home>());
        shirtRend.material = GetMaterialByString(army.GetTeamName());
        SetHealth(maxHealth);
        animator.CrossFade("Revive", 1.5f, -1, 0.0f);
        animator.CrossFade("Idle", 1f, -1, 1.5f);
        SetMode(mode);

    }

    private Material GetMaterialByString(string name)
    {
        Debug.Log("Player, GetMaterialByString : search = " + name);
        foreach (Material material in materials)
        {
            Debug.Log("Player, GetMaterialByString : material name = " + material.name);
            if (material.name == name) return material;
        }
        return defaultMaterial;
    }

    public void StartParticles()
    {
        resurectParticles.Play();
    }

    public void StopParticles()
    {
        //resurectParticles.Stop();
    }


}
