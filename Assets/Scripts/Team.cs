using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] private string teamName;
    [SerializeField] private int nbArmyKilled = 0;
    [SerializeField] private Army army = null;
    [SerializeField] private bool hasLost = false;

    private void Awake()
    {
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
}
