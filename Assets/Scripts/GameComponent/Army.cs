using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Army : MonoBehaviour
{
    public abstract List<Minion> GetAlive();
    public abstract Player GetPlayer();
    public abstract void RemoveFromArmy(Minion minion, Army killerArmy);
    public abstract void SetMinionToHome();
    public abstract void SetMinionToLeaveHome();
    public abstract void ReplaceMinions();
    public abstract void Resurect(List<Minion> resurectList);
    public abstract void KillAll();
    public abstract void SetEnemy(Army enemy);
    public abstract List<Minion> GetArmy();
    public abstract void AttackHome(Home home);
    public abstract void SetMode(string mode);
    public abstract string GetTeamName();
    public abstract bool IsPlayer();
}
