using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWhenTooFarDown : MonoBehaviour
{
    void Update()
    {
        if (transform.position.z < -1000f)
            Destroy(gameObject);
    }
}
