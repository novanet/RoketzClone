using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BasicWeapon : MonoBehaviour
{
    public float ShotsPerSecond = 3f;
    public float BulletSpeed = 50f;
    public GameObject BulletPrefab;

    private float _timeOfLastShot;
    private float _timeBetweenShots;
    private AudioSource _audioSource;
    private PlayerInput _input;
    private Rigidbody _rigidbody;

    void Awake()
    {
        var playerNumber = GetComponentInParent<ShipBehaviour>().PlayerNumber;
        _input = new PlayerInput(playerNumber);
        _timeBetweenShots = 1 / ShotsPerSecond;
        _audioSource = GetComponent<AudioSource>();

        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        if (_input.IsShooting() && CanShoot())
        {
            Shoot();
        }
    }

    private bool CanShoot()
    {
        if (_timeOfLastShot == 0)
            return true;

        var timeSinceLastShot = Time.time - _timeOfLastShot;
        return timeSinceLastShot >= _timeBetweenShots;
    }

    private void Shoot()
    {
        _audioSource.Play();
        var bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = _rigidbody.velocity + (transform.forward * BulletSpeed);
        bullet.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.1f);

        _timeOfLastShot = Time.time;
    }
}

