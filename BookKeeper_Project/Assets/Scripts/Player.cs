using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Rigidbody rigidbody = null;
    [Header("Visual")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private List<Material> materials;
    [SerializeField] GameObject playerVisual = null;
    [SerializeField] GameObject hat = null;
    [SerializeField] GameObject robe = null;


    [Header("Infos")]
    [SerializeField] private bool isAlive = true;
    [SerializeField] private float maxHealth = 2000f;
    [SerializeField] private float health = 2000f;
    [SerializeField] private float SPEED = 0.3f;


    [Header("Combat")]
    [SerializeField] private int deathNumber = 0;
    [SerializeField] private List<Minion> resurectList = null;

    [Header("Team")]
    [SerializeField] private Team team;
    [SerializeField] private Home home;
    [SerializeField] private string teamName;
    [SerializeField] public bool isGettingHome = false;
    [SerializeField] public bool isHome = false;

    [Header("Army")]
    [SerializeField] private List<Transform> points;
    [SerializeField] private Army army;

    // Start is called before the first frame update
    void Awake()
    {
        SetPoints();
        rigidbody = GetComponent<Rigidbody>();
        motor = GetComponent<PlayerMotor>();
        playerVisual = transform.Find("PlayerVisual").gameObject;
        hat = playerVisual.transform.Find("Hat").gameObject;
        robe = playerVisual.transform.Find("Robe").gameObject;
        animator = playerVisual.GetComponent<Animator>();

        army = GetComponentInParent<Army>();
        team = army.GetComponentInParent<Team>();
        home = team.GetComponentInChildren<Home>();

        animator.enabled = true;
    }

    private void Update()
    {
        if (isGettingHome) CheckHomeDistance();
        if (isHome) CheckHomeDistance();
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

        points.AddRange(new Transform[] { inFront, midRight, midLeft, maxRight, maxLeft });

        transform.Rotate(playerRot);

    }
    private void FixedUpdate()
    {
        animator.SetBool("isWalking", motor.GetIsMoving());
    }
    public List<Transform> GetPoints()
    {
        return points;
    }

    public void SetTeamName(string name)
    {
        teamName = name;
        SetPlayerColor();
    }
    public string GetTeamName()
    {
        return teamName;
    }
    public Team GetTeam()
    {
        return team;
    }

    public Vector3 GetSteeringDirection()
    {
        return motor.GetSteeringDirection();
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position if point exist
        foreach (Transform point in points)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(point.position + Vector3.down, new Vector3(1, 1 ,1));
        }
        
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

    //          Life related
    public bool GetAlive()
    {
        return isAlive;
    }
    public void SetAlive(bool value)
    {
        isAlive = value;
    }
    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
    }
    public float GetHealth()
    {
        return health;
    }

    private void SetHealth(float amount)
    {
        health = amount;
    }
    public void ResetHealth()
    {
        health = maxHealth;
    }

    public bool RemoveHealth(float amount)
    {
        Debug.Log($"Player, RemoveHealth : {gameObject.name} loss {amount} pv");
        if (amount < health)
        {
            SetHealth(health - amount);
            return true;
        }
        else
        {
            Die();
            return false;

        }
    }
    public bool GetHit(float damage)
    {
        return RemoveHealth(damage);
    }

    public void Die()
    {
        Debug.Log($"Player, Die : {gameObject.name}");
        deathNumber++;
        SetHealth(0);
        SetAlive(false);    
        //if (deathNumber <= 4) bodyRend.material = GetMaterialByString("Minion" + deathNumber);

        animator.SetTrigger("Die");
        SetAlive(false);
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX |
                                RigidbodyConstraints.FreezePositionY | 
                                RigidbodyConstraints.FreezePositionZ |
                                RigidbodyConstraints.FreezeRotationX |
                                RigidbodyConstraints.FreezeRotationY |
                                RigidbodyConstraints.FreezeRotationZ;
    }
    public void SetPlayerColor()
    {
        //Set player color, it depends on team name
        var hatRend = hat.GetComponent<Renderer>();
        var robeRend = robe.GetComponent<Renderer>();

        hatRend.material = GetMaterialByString(teamName + "_hat");
        robeRend.material = GetMaterialByString(teamName);
    }
    public void MoveToHome()
    {
        army.SetMinionToHome();
        motor.MoveToPoint(home.transform.position, true);
        isGettingHome = true;
    }
    private void GetInGHome()
    {
        DisableVisual();
        isHome = true;
        isGettingHome = false;
        home.AddPlayer();
    }
    private void GetOutGHome()
    {
        home.RemovePlayer();
        army.SetMinionToLeaveHome();
        army.ReplaceMinions();
        isHome = false;
        EnableVisual();
    }
    private void DisableVisual()
    {
        playerVisual.gameObject.SetActive(false);
    }
    private void EnableVisual()
    {
        if (!playerVisual.gameObject.active)
        {
            playerVisual.gameObject.SetActive(true);
        }
    }
    private void CheckHomeDistance()
    {
        if (!isHome && (transform.position - home.transform.position).magnitude <= 4f)
        {
            GetInGHome();
        }

        if (isHome && (transform.position - home.transform.position).magnitude > 5f)
        {
            GetOutGHome();
            
        }
    }

    public void Resurect(List<Minion> army)
    {
        resurectList.AddRange(army);
        animator.SetTrigger("Resurect");
        foreach (Minion minion in army)
        {
            minion.StartParticles();
        }
    }

    public void EndResurect()
    {
        foreach (Minion minion in resurectList)
        {
            minion.StopParticles();
        }
        army.Resurect(resurectList);
        resurectList.Clear();
    }
    public void GoIdle()
    {
        animator.CrossFade("Idle", 1.5f, -1, 0.0f);
    }

}



