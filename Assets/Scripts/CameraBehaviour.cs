using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform Player;

    private Vector2 _lastPosition;
    
    void Start()
    {
        Follow();
        SetLastPositionToCurrent();
    }

    void Update()
    {
        var offset = new Vector2();
        Follow(offset);
        SetLastPositionToCurrent();
    }

    private void Follow(Vector2 offset = new Vector2())
    {
        transform.position = new Vector3(
            Player.position.x,
            transform.position.y,
            Player.position.z
        );
    }

    private void SetLastPositionToCurrent()
    {
        _lastPosition = new Vector2(
            transform.position.x, 
            transform.position.z);
    }
}
