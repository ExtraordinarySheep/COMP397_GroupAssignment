using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : PersistentSingleton<ProjectilePoolManager>
{
    [SerializeField] private ProjectilePooled _projectilePoolPrefab; 
    private Queue<ProjectilePooled> _projectilePool = new Queue<ProjectilePooled>();

    public ProjectilePooled Get()
    {
        if ( _projectilePool.Count == 0)
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
    public void ReturnToPool(ProjectilePooled projectile)
    {
        projectile.gameObject.SetActive(false);
        _projectilePool.Enqueue(projectile);
    }
}

