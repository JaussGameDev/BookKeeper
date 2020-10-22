using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public enum TeamType
    {
        Player,
        IA
    };

    [SerializeField] private TeamType type = TeamType.Player; 
    [SerializeField] private string teamName;
    [SerializeField] private int nbArmyKilled = 0;
    [SerializeField] private Army army = null;
    [SerializeField] private bool hasLost = false;
    [SerializeField] private Home home = null;

    private void Awake()
    {
        if (teamName == "Ia") type = TeamType.IA;
        else home = GetComponentInChildren<Home>();
        army = GetComponentInChildren<Army>();
    }
    public string GetTeamName()
    {
        return teamName;
    }
    public void LooseGame() 
    {
        hasLost = true;
        army.KillAll();
    }
    public void AddKill()
    {
        nbArmyKilled ++;
    }
    public void SetArmy(Army value)
    {
        army = value;
    }
    public TeamType GetType()
    {
        return type;
    }
    public Home GetHome()
    {
        return home;
    }
    public bool GetHasLost()
    {
        return hasLost;
    }
}
