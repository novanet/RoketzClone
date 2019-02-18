using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShipBehaviour : MonoBehaviour
{
    public float TopSpeed = 40f;
    public float Acceleration = 60f;
    public float TurnRate = 3f;
    public Text DebugText;
    public int PlayerNumber;

    private PlayerInput _playerInput;

    public Transform[] Wreckage;
    [FormerlySerializedAs("ExplosionPrefab")] public GameObject ShipExplosionPrefab;
    
    private Vector2 _previousDirection;
    private Vector2 _target;
    private KeyCode boostButton;
    private bool _dead = false;

    private AudioSource _boostSound;
    private AudioSource _impactSound;

    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _particleEmission;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _playerInput = new PlayerInput(PlayerNumber);

        _rigidbody = GetComponent<Rigidbody>();
        
        var audioSources = GetComponents<AudioSource>();
        _boostSound = audioSources[0];
        _impactSound = audioSources[1];

        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleEmission = _particleSystem.emission;
        
        _target = transform.position.ToVector2() + new Vector2(0, 1); // Points upwards
    }

    void FixedUpdate()
    {
        if (_dead)
        {
            _rigidbody.AddForce(Constants.Gravity * 10);
            return;
        }
        
        _target = _playerInput.GetAnalogueStickDirection();
        
        Rotate();

        if (_playerInput.IsBoosting())
            Boost();
        else
            Drag();
            

        ConstrainToPlane();
    }
    
    private void Rotate()
    {
        var rotation = GetTurn();

        rotation = GetRoll(rotation);

        transform.rotation = rotation;
    }

    private Quaternion GetTurn()
    {
        return Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(_target.ToVector3(), transform.up),
            Time.deltaTime * TurnRate);
    }

    private Quaternion GetRoll(Quaternion rotation)
    {
        var currentDirection = transform.forward.ToVector2();
        var angle = Vector2.SignedAngle(_previousDirection, currentDirection) * 7;
        
        _previousDirection = currentDirection;

        var euler = rotation.eulerAngles;
        var result = Quaternion.Euler(
            0,
            euler.y,
            angle
        );
        
        return result;
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

        var direction = _target.ToVector3().normalized;
        
        var rigidBody = GetComponent<Rigidbody>();
        var velocity = rigidBody.velocity;
        velocity += direction * Acceleration * Time.deltaTime;

        velocity = LimitToMaxSpeed(velocity);

        rigidBody.velocity = velocity;
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

    private Vector3 LimitToMaxSpeed(Vector3 velocity)
    {
        if (velocity.magnitude > TopSpeed)
            return velocity.normalized * TopSpeed;

        return velocity;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag == "Player")
        {
            if (!_impactSound.isPlaying)
                _impactSound.Play();
            Die();
        }

        if (new[]{"Scenery", "Depot"}.Contains(collision.transform.gameObject.tag) && _dead)
        {
            Explode();
        }
    }

    private void Explode()
    {
        var explosion = Instantiate(ShipExplosionPrefab, transform.position, ShipExplosionPrefab.transform.rotation);
        var time = explosion.GetComponent<AudioSource>().clip.length;
        Destroy (explosion, time);

        gameObject.SetActive(false);
    }

    private void Die()
    {
        _dead = true;
    }

}
