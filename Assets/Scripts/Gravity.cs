using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Constants.Gravity);
    }
}
