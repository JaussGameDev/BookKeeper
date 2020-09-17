using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Transform trans = null;
    public float speed = 20f;

    // Update is called once per frame
    void FixedUpdate()
    {
        trans.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}
