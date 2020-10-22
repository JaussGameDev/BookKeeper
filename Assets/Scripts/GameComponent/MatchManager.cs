using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] List<Team> teams = new List<Team>();
    [SerializeField] bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        var locTeams = FindObjectsOfType<Team>();
        foreach(Team tms in locTeams)
        {
            if (tms.GetType() == Team.TeamType.Player) teams.Add(tms);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var locTeam = new List<Team>();
        locTeam.AddRange(teams);
        foreach(Team tms in locTeam)
        {
            if (tms.GetHasLost())
            {
                teams.Remove(tms);
                if (teams.Count <= 1) gameOver = true;
            }
        }
    }
}
