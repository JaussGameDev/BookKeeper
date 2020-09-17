using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject iaArmy;
    [SerializeField] private GameObject minion;
    [SerializeField] private int minionNb = 10;
    // Start is called before the first frame update
    void Start()
    {
        var army = Instantiate(iaArmy);
        var i = 0;
        var list = new List<Minion>();
        while (i < minionNb)
        {
            var unit = Instantiate(minion);
            list.Add(unit.GetComponent<Minion>());
            unit.transform.parent = army.transform.Find("minions");
            i++;
        }
        army.transform.parent = GetComponentInChildren<Team>().transform;
        foreach (Minion minion in list)
        {
        }
        GetComponentInChildren<Team>().SetArmy(army.GetComponent<IaArmy>());
        army.GetComponent<IaArmy>().AddMinions(list);


    }

}
