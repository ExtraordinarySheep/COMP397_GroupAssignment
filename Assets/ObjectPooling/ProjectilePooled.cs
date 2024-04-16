using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooled : MonoBehaviour
{
    [SerializeField] private float _lifeTime, _maxLifeTime = 5f;

    private void OnEnable()
    {
        _lifeTime = 0; 
    }

    private void FixedUpdate()
    {
        _lifeTime += Time.fixedDeltaTime;
        if ( _lifeTime > _maxLifeTime)
        {
            ProjectilePoolManager.Instance.ReturnToPool(this);
        }
    }

}
