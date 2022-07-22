using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview.Towers
{
    public class SimpleTower : BaseTower
    {
        [Header("Simple Tower Settings")]
        [SerializeField] private float firingRate;

        private float fireTimer;

        public override void Initialize(IReadOnlyList<Enemy> enemies)
        {
            base.Initialize(enemies);
            fireTimer = firingRate;
        }
        
        protected override void UpdateTargetEnemy()
        {
            base.UpdateTargetEnemy();
            if (targetEnemy == null)
                return;
            
            var lookRotation = Quaternion.LookRotation(targetEnemy.transform.position - transform.position);
            var rotation = transform.rotation;
            rotation = Quaternion.Euler(rotation.eulerAngles.x, lookRotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;
        }

        protected override void UpdateFireRate()
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                if (targetEnemy != null)
                {
                    var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity)
                        .GetComponent<Bullet>();
                    bullet.Initialize(targetEnemy);
                }

                fireTimer = firingRate;
            }
        }
    }
}
