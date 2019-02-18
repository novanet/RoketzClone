using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipExplosion : MonoBehaviour
{
    public Transform[] WreckagePrefab;

    void Start()
    {
        var position = transform.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        
        for (var i = 0; i < 15; i++)
        {
            var index = UnityEngine.Random.Range(0, WreckagePrefab.Length);
            var piece = Instantiate(WreckagePrefab[index], position + new Vector3(0, 0, 2f), Quaternion.Euler(0, 0, 0));
            piece.transform.localScale = new Vector3(
                UnityEngine.Random.Range(1f, 3.0f),
                UnityEngine.Random.Range(1f, 3.0f),
                UnityEngine.Random.Range(1f, 3.0f)
                );
            
            var rigidbody = piece.GetComponent<Rigidbody>();

            var direction = Vector3.forward + new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                0f,
                1f
                ).normalized;

            rigidbody.AddForce(direction * UnityEngine.Random.Range(0.5f, 1.5f));

            rigidbody.AddTorque(
                UnityEngine.Random.Range(0.1f, 0.9f),
                UnityEngine.Random.Range(0.1f, 0.9f),
                UnityEngine.Random.Range(0.1f, 0.9f)
            );
        }
    }
}
