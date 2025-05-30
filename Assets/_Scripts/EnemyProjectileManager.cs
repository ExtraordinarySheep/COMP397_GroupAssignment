using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileManager : PersistentSingleton<EnemyProjectileManager>
{
    [SerializeField] private EnemyProjectile _projectilePoolPrefab;
    private Queue<EnemyProjectile> _projectilePool = new Queue<EnemyProjectile>();

    public EnemyProjectile Get()
    {
        if (_projectilePool.Count == 0)
        {
            AddProjectile(1);
        }
        return _projectilePool.Dequeue();
    }
    private void AddProjectile(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var projectile = Instantiate(_projectilePoolPrefab);
            projectile.gameObject.SetActive(false);
            _projectilePool.Enqueue(projectile);
        }
    }
    public void ReturnToPool(EnemyProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _projectilePool.Enqueue(projectile);
    }
}
