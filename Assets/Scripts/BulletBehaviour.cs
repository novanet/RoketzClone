using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float LifeTimeInSeconds = 6f;
    public int Damage = 1;

    void Start()
    {
        Destroy(gameObject, LifeTimeInSeconds);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
