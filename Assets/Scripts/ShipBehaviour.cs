using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBehaviour : MonoBehaviour
{
    public float TopSpeed = 40f;
    public float Acceleration = 100f;
    public float TurnRate = 3f;
    public Text DebugText;
    private Vector2 _previousDirection;

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1f)
        {
            Rotate(horizontal, vertical);
        }

        if (Input.GetKey(KeyCode.JoystickButton0))
            Boost(horizontal, vertical);
    }

    private void Boost(float horizontal, float vertical)
    {
        var direction = new Vector3(horizontal, 0, vertical);
        
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce((transform.forward + direction).normalized * Acceleration);

        var velocity = rigidBody.velocity;
        if (velocity.magnitude > TopSpeed)
        {
            velocity = velocity.normalized * TopSpeed;
            rigidBody.velocity = velocity;
        }
        
        DebugText.text = $"Speed: {velocity.magnitude}";        
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
        //DebugText.text = String.Format("diff: {0}", angle);
        _previousDirection = currentDirection;

        var euler = rotation.eulerAngles;
        var result = Quaternion.Euler(
            0,
            euler.y,
            angle
        );
        
        return result;
    }

    private Quaternion GetTurn(float horizontal, float vertical)
    {
        return Quaternion.Lerp(
           transform.rotation,
           Quaternion.LookRotation(new Vector3(horizontal, 0, vertical), transform.up),
           Time.deltaTime * TurnRate);
    }
}
