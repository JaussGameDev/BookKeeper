using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAnimation : MonoBehaviour
{
    private Player player = null;
    // Start is called before the first frame update
    void Awake()
    {
        player = GetComponentInParent<Player>();
    }


    private void EndResurect()
    {
        player.EndResurect();
    }
    private void GoIdle()
    {
        player.GoIdle();
    }
}
