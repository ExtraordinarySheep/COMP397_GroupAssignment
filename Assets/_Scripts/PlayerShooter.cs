using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private List<Transform> _projectileSpawns;
    [SerializeField] private float _projectileForce = 0f;
    private PlayerControl _inputs;
    private bool _projectileFired = false;

    private void Awake()
    {
       _inputs = new PlayerControl();
    }

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
    private void Update()
    {
        if (_inputs.Player.Fire.IsPressed() && !_projectileFired) // Check if fire button is pressed and no projectile has been fired
        {
            ShootPooledProjectile();
            _projectileFired = true; // Set the flag to true
        }
        else if (!_inputs.Player.Fire.IsPressed()) // Reset the flag when fire button is released
        {
            _projectileFired = false;
        }
    }

    private void ShootPooledProjectile()
    {
        var projectile = ProjectilePoolManager.Instance.Get();
        projectile.transform.SetPositionAndRotation(_projectileSpawns[0].position, _projectileSpawns[0].rotation);
        projectile.gameObject.SetActive(true);
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * _projectileForce, ForceMode.Impulse);
    }

}
