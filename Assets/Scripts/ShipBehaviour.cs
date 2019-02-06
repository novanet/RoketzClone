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
    private Vector2 _previousDirection;

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
        var stickDirection = new Vector3(horizontal, 0, vertical);
        var direction = transform.position + stickDirection;
        transform.LookAt(direction);
    }

    private void Rotate(float horizontal, float vertical)
    {
        var rotation = GetTurn(horizontal, vertical);

        rotation = GetRoll(rotation);

        transform.rotation = rotation;
    }

    private Quaternion GetRoll(Quaternion rotation)
    {
        var currentDirection = new Vector2(transform.forward.x, transform.forward.z);
        var angle = Vector2.SignedAngle(_previousDirection, currentDirection) * 5;
        DebugText.text = String.Format("diff: {0}", angle);
        _previousDirection = currentDirection;

        var euler = rotation.eulerAngles;
        return Quaternion.Euler(
            euler.x,
            euler.y,
            angle
        );
        //return rotation;
    }

    private Quaternion GetTurn(float horizontal, float vertical)
    {
        return Quaternion.Lerp(
           transform.rotation,
           Quaternion.LookRotation(new Vector3(horizontal, 0, vertical), transform.up),
           Time.deltaTime * TurnRate);
    }
}
