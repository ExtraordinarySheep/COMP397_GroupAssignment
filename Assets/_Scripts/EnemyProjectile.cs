using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
        [SerializeField] private float lifetime, maxLifetime = 5f;

        private void OnEnable()
        {
            lifetime = 0;
        }

        private void FixedUpdate()
        {
            lifetime += Time.fixedDeltaTime;
            if (lifetime > maxLifetime)
            {
                EnemyProjectileManager.Instance.ReturnToPool(this);
            }
        }
}
