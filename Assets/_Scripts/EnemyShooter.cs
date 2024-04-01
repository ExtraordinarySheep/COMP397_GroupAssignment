using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] EnemyStates enemyState;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private float projectileForce = 10f;
    private bool interval = true;

    private void Update()
    {
        enemyState = gameObject.GetComponent<EnemyController>().enemyState;
        if (enemyState == EnemyStates.Chasing)
        {
            if (interval) StartCoroutine(ShootInterval());
        }
    }

    IEnumerator ShootInterval()
    {
        interval = false;
        yield return new WaitForSeconds(2);
        ShootProjectile();
        interval = true;
    }

    private void ShootProjectile()
    {
        var projectile = EnemyProjectileManager.Instance.Get();
        projectile.transform.SetPositionAndRotation(projectileSpawn.position, projectileSpawn.rotation);
        projectile.gameObject.SetActive(true);
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * projectileForce, ForceMode.Impulse);
    }
}
