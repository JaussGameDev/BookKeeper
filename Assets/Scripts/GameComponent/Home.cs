using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Home : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Team team = null;
    [SerializeField] private Player player = null;
    [SerializeField] private CameraController camera = null;
    [Header("Visual")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private List<Material> materials;
    [Header("Infos")]
    [SerializeField] private float health = 20000f;
    [SerializeField] private bool isBroken = false;
    [SerializeField] private float damage = 20f;
    [SerializeField] private bool playerIn = false;
    [SerializeField] private List<Minion> minionsIn;
    [SerializeField] private float PlayerMaxHealth = 2000f;
    [SerializeField] private float MinionMaxHealth = 200f;
    [Header("Combat")]
    [SerializeField] private Minion aimedMinion = null;
    [SerializeField] private bool attack = false;
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private List<Minion> opponents;
    [SerializeField] private int attackTimer = 5;
    [SerializeField] private float range = 15f;

    void Awake()
    {
        team = GetComponentInParent<Team>();
        player = team.GetComponentInChildren<Player>();
        var visual = transform.Find("HomeVisual").gameObject;
        Debug.Log("HomeVisual = " + visual);
        var roofRend = visual.transform.Find("Roofs").GetComponent<Renderer>();
        var flagRend = visual.transform.Find("Flag").GetComponent<Renderer>();
        roofRend.material = GetMaterialByString(team.GetTeamName());
        flagRend.material = GetMaterialByString(team.GetTeamName() + "Flag");
    }
    private void FixedUpdate()
    {
        if (health <= 0) return;
        CheckAddOpponent();
        CheckOpponent();
        CheckAttack();
        if (attackTimer < 5) attackTimer++;
    }
    public Team GetTeam()
    {
        return team;
    }

    private void CheckAddOpponent()
    {
        var hits = Physics.OverlapSphere(transform.position + Vector3.down, range, attackMask);
        foreach (Collider hit in hits)
        {
            if (hit.tag == "Minion"
                && hit.gameObject.GetComponent<Minion>().GetTeamName() != team.GetTeamName()
                && !FindOpponent(hit.gameObject.GetComponent<Minion>()))
            {
                AddOpponent(hit.gameObject.GetComponent<Minion>());
            }
        }
    }
    private void CheckOpponent()
    {
        var list = new List<Minion>();
        list.AddRange(opponents);
        foreach (Minion minion in list)
        {
            if ((!minion.GetAlive()
                || Vector3.Distance(minion.transform.position, transform.position) > range
                && FindOpponent(minion)))
            {
                RemoveOpponent(minion);
            }
        }
    }
    private void CheckAttack()
    {
        if (aimedMinion != null) Attack(aimedMinion);
        if (opponents.Count != 0) SetAim();
    }

    public void Attack(Minion minion)
    {
        var distance = Vector3.Distance(transform.position, minion.transform.position);
        if (distance <= range && minion.GetHealth() > 0)
        {
            attack = true;
            if (attackTimer == 5) 
            { 
                minion.GetHit(damage); 
                attackTimer = 0;
                Debug.Log($"Home, Attack : {gameObject.name} hit {minion.gameObject.name}");
            }
        }
        else { aimedMinion = null; }
    }

    private void SetAim()
    {
        var distances = new List<float>();
        foreach (Minion minion in opponents)
        {
            var distance = Vector3.Distance(transform.position, minion.transform.position);
            if (distance <= range && minion.GetHealth() > 0)
            {
                distances.Add(distance);
            }
        }
        if (distances.Count == 0) return;

        var nb = distances.Min();
        int index = distances.IndexOf(nb);
        aimedMinion = opponents[index];
        Debug.Log($"Home, SetAim : {gameObject.name} hit {aimedMinion}");
    }

    public void AddMinion(Minion minion)
    {
        Debug.Log("Home, AddMinion : " + minion);
        minionsIn.Add(minion);
        minion.SetMaxHealth(MinionMaxHealth);
        minion.ResetHealth();
    }

    public void AddOpponent(Minion minion)
    {
        Debug.Log("Home, AddOpponent : " + minion);
        opponents.Add(minion);
    }
    public void RemoveOpponent(Minion minion)
    {
        Debug.Log("Home, RemoveOpponent : " + minion);
        opponents.Remove(minion);
    }
    public void RemoveMinion(Minion minion)
    {
        minionsIn.Remove(minion);
    }

    public void AddPlayer()
    {
        Debug.Log("Home, AddPlayer : " + player);
        playerIn = true;
        player.SetMaxHealth(PlayerMaxHealth);
        player.ResetHealth();
        camera.SetTarget(this.transform);
    }
    public void RemovePlayer()
    {
        playerIn = false;
        camera.SetTarget(player.transform);
    }
    public float GetHealth()
    {
        Debug.Log("Home, GetHealth : " + health);
        return health;
    }
    public bool RemoveHealth(float damage)
    {
        Debug.Log("Home, RemoveHealth : " + damage);
        if (health > damage) { health -= damage; return true; }

        team.LooseGame();
        isBroken = true;
        health = 0; 
        return false;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position + Vector3.down, range);
    }

    private bool FindOpponent(Minion minion)
    {
        foreach (Minion opponent in opponents)
        {
            if (minion == opponent) return true;
        }
        return false;
    }
    public bool GetBroken()
    {
        return isBroken;
    }
}
