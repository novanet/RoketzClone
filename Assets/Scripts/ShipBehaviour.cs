using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBehaviour : MonoBehaviour
{
    public float Speed = 5f;
    public float TurnRate = 3f;
    public Text DebugText;
    private Vector2 _previousRotation;

    void Update()
    {
        if (Input.GetKey(KeyCode.JoystickButton0))
            transform.position += transform.forward * Speed * Time.deltaTime;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1f)
            Rotate(horizontal, vertical);
    }

    private void LookAt(float horizontal, float vertical)
    {
        var stickDirection = new Vector3(horizontal, vertical, 0);
        var direction = transform.position + stickDirection;
        transform.LookAt(direction);     
    }

    private void Rotate(float horizontal, float vertical)
    {
        var position = transform.position;

        var quaternion = Quaternion.Lerp(
            Quaternion.LookRotation(transform.forward, transform.up),
            Quaternion.LookRotation(position + new Vector3(horizontal, vertical, 0), transform.up),
            Time.deltaTime * TurnRate);

        transform.rotation = quaternion;
        
        //debug
        var euler = transform.rotation.eulerAngles;
        var current = new Vector2(euler.x, euler.y);
        var diff = Vector2.Angle(_previousRotation, current);
        DebugText.text = String.Format("diff: {0}", diff);
        _previousRotation = current;
    }
}
