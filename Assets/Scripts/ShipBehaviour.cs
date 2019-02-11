using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ShipBehaviour : MonoBehaviour
{
    public float TopSpeed = 40f;
    public float Acceleration = 100f;
    public float TurnRate = 3f;
    public Text DebugText;
    private Vector2 _previousDirection;
    private Vector2 _target;

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1f)
        {
            _target = new Vector2(horizontal, vertical);    
        }

        Rotate();
        
        if (Input.GetKey(KeyCode.JoystickButton0))
            Boost();

        ApplyGravity();
        ConstrainToPlane();
    }

    private void ConstrainToPlane()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void ApplyGravity()
    {
        GetComponent<Rigidbody>().AddForce(Constants.Gravity);
    }

    private void Boost()
    {
        var direction = new Vector3(_target.x, 0, _target.y);
        
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

    private void Rotate()
    {
        var rotation = GetTurn();

        rotation = GetRoll(rotation);

        transform.rotation = rotation;
    }

    private Quaternion GetRoll(Quaternion rotation)
    {
        var currentDirection = new Vector2(transform.forward.x, transform.forward.z);
        var angle = Vector2.SignedAngle(_previousDirection, currentDirection) * 7;
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

    private Quaternion GetTurn()
    {
        return Quaternion.Lerp(
           transform.rotation,
           Quaternion.LookRotation(new Vector3(_target.x, 0, _target.y), transform.up),
           Time.deltaTime * TurnRate);
    }
}
