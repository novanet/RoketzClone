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
    private bool _joystickDetected;
    private KeyCode boostButton;

    private void Start()
    {
        _target = new Vector2(0, 1); // Points upwards
        _joystickDetected = Input.GetJoystickNames().Length > 0;
        InitializeBoostButton();
        Debug.Log("Joystick attached = " + _joystickDetected);
    }

    private void InitializeBoostButton()
    {
        if (_joystickDetected)
            boostButton = KeyCode.Joystick1Button0;
        else
            boostButton = KeyCode.Space;
    }

    void FixedUpdate()
    {
        SetDirectionByControllerInput();

        Rotate();

        if (Input.GetKey(boostButton))
            Boost();

        ApplyGravity();
        ConstrainToPlane();
    }

    private void SetDirectionByControllerInput()
    {
        if (_joystickDetected)
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1f)
            {
                _target = new Vector2(horizontal, vertical);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
                _target = VectorExtensions.Rotate(_target, 6f);
            else if (Input.GetKey(KeyCode.D))
                _target = VectorExtensions.Rotate(_target, -6f);
        }
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

public static class VectorExtensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;

        //float radians = degrees * Mathf.Deg2Rad;
        //float sin = Mathf.Sin(radians);
        //float cos = Mathf.Cos(radians);

        //float tx = v.x;
        //float ty = v.y;

        //return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}
