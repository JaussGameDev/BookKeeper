using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;
    public LayerMask attackableMask;
    public LayerMask homeMask;

    Camera cam;
    PlayerMotor motor;
    Army army;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        army = GetComponentInParent<Army>();
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetAlive())
        {
            CheckMoveTo();
            CheckAttack();
            CheckHome();
        }
    }
    private void CheckMoveTo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            //          MOVE
            if (Physics.Raycast(ray, out hit, 100, movementMask))
            {
                motor.MoveToPoint(hit.point, true);
            }
        }
    }


    private void CheckAttack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //          ATTACK
            if (Physics.Raycast(ray, out hit, 100, attackableMask))
            {
                var opponent = hit.transform.gameObject.GetComponentInParent<Army>();
                if (opponent != army) army.SetEnemy(opponent);
                if (opponent != army && opponent.GetAlive().Count == 0)  player.Resurect(opponent.GetArmy());
                Debug.Log("PlayerController, Update->CheckAttack : We hit " +opponent.name);
                //Check if interactable
                //If it is -> focus it
            }
            else
            {
                army.SetEnemy(null);
                Debug.Log("PlayerController, Update->CheckAttack : We did not opponent.");
            }
        }
    }
    private void CheckHome()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //          ATTACK
            if (Physics.Raycast(ray, out hit, 100, homeMask))
            {
                var home = hit.transform.gameObject.GetComponentInParent<Home>();
                if (home.GetTeam() != player.GetTeam()) { army.AttackHome(home); return; }

                player.MoveToHome();
                Debug.Log("PlayerController, Update->CheckHome : We hit an home.");
            }
            else
            {
                army.SetMode("follow");
                Debug.Log("PlayerController, Update->CheckHome : We did not home .");
            }
        }
    }

}
