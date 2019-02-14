using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ShipBehaviour : MonoBehaviour
{
    public float TopSpeed = 40f;
    public float Acceleration = 60f;
    public float TurnRate = 3f;
    public Text DebugText;

    public Transform[] Wreckage;
    
    private Vector2 _previousDirection;
    private Vector2 _target;
    private KeyCode boostButton;
    private bool _dead = false;

    private AudioSource _boostSound;
    private AudioSource _impactSound;
    private AudioSource _explosionSound;

    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _particleEmission;

    private void Start()
    {
        var audioSources = GetComponents<AudioSource>();
        _boostSound = audioSources[0];
        _impactSound = audioSources[1];
        _explosionSound = audioSources[2];

        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleEmission = _particleSystem.emission;
        
        _target = new Vector2(0, 1); // Points upwards
    }

    void FixedUpdate()
    {
        if (_dead)
        {
            GetComponent<Rigidbody>().AddForce(Constants.Gravity * 10);
            return;
        }
        
        SetDirectionByControllerInput();

        Rotate();

        if (BoostButtonPressed())
            Boost();
        else
            Drag();
            

        ConstrainToPlane();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag == "Player")
        {
            if (!_impactSound.isPlaying)
                _impactSound.Play();
            Die();
        }

        if (collision.transform.gameObject.tag == "Scenery" && _dead)
        {
            Explode();
        }
    }

    private void Explode()
    {
        var position = transform.position;
        
        _explosionSound.Play();
        _boostSound.Stop();
        DisableEverything();
        
        for (var i = 0; i < 30; i++)
        {
            var index = UnityEngine.Random.Range(0, 2);
            var piece = Instantiate(Wreckage[index], position + new Vector3(0, 0, 2f), Quaternion.Euler(0, 0, 0));
            piece.transform.localScale = new Vector3(
                UnityEngine.Random.Range(0.3f, 2.0f),
                UnityEngine.Random.Range(0.3f, 2.0f),
                UnityEngine.Random.Range(0.3f, 2.0f)
                );
            
            var rigidbody = piece.GetComponent<Rigidbody>();

            var direction = Vector3.forward + new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                0f,
                1f
                ).normalized;

            rigidbody.AddForce(direction * UnityEngine.Random.Range(0.1f, 0.5f));

            rigidbody.rotation = Quaternion.Euler(
                UnityEngine.Random.Range(0.1f, 0.9f),
                UnityEngine.Random.Range(0.1f, 0.9f),
                UnityEngine.Random.Range(0.1f, 0.9f)
            );
        }
    }

    private void DisableEverything()
    {
        Destroy(GetComponent<Gravity>());
        Destroy(GetComponent<Rigidbody>());
        
        transform.position = new Vector3(transform.position.x, -200, transform.position.z);
    }

    private void Die()
    {
        _dead = true;
    }

    private bool BoostButtonPressed()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
    }

    private void SetDirectionByControllerInput()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1f)
        {
            _target = new Vector2(horizontal, vertical);
        }

        if (Input.GetKey(KeyCode.A))
            _target = VectorExtensions.Rotate(_target, TurnRate);
        else if (Input.GetKey(KeyCode.D))
            _target = VectorExtensions.Rotate(_target, -TurnRate);   
    }

    // We'll brake any horizontal and upwards movement while keeping gravity intact.
    private void Drag()
    {
        if (_boostSound.isPlaying)
            _boostSound.Pause();

        _particleEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
        
        var dragMultiplier = 0.99f;
        
        var rigidBody = GetComponent<Rigidbody>();
        var velocity = rigidBody.velocity;
        velocity.x = velocity.x * dragMultiplier;
        if (velocity.z > 0f)
            velocity.z = velocity.z * dragMultiplier;
        rigidBody.velocity = velocity;
    }

    private void ConstrainToPlane()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void Boost()
    {
        if (!_boostSound.isPlaying)
            _boostSound.Play();

        _particleEmission.rateOverTime = new ParticleSystem.MinMaxCurve(300);
        
        var direction = new Vector3(_target.x, 0, _target.y).normalized;
        
        var rigidBody = GetComponent<Rigidbody>();
        var velocity = rigidBody.velocity;
        velocity += direction * Acceleration * Time.deltaTime;

        velocity = LimitToMaxSpeed(velocity);

        rigidBody.velocity = velocity;
        
        DebugText.text = $"Speed: {velocity.magnitude}";        
    }

    private Vector3 LimitToMaxSpeed(Vector3 velocity)
    {
        if (velocity.magnitude > TopSpeed)
            return velocity.normalized * TopSpeed;

        return velocity;
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
