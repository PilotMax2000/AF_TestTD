using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview.Towers
{
    public class BurstTower : BaseTower
    {
        [Header("Burst Tower Settings")]
        [SerializeField] private float mainShootingCooldown;
        [SerializeField] private float burstShootingCooldown;

        private CooldownTimer mainCooldownTimer;

        public override void Initialize(IReadOnlyList<Enemy> enemies)
        {
            base.Initialize(enemies);
            
            InitMainCooldownTimer();
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
            mainCooldownTimer.UpdateByTime(Time.deltaTime);
            if (mainCooldownTimer.IsOver)
            {
                if (targetEnemy == null)
                    return;
                
                var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity)
                    .GetComponent<Bullet>();
                bullet.Initialize(targetEnemy.gameObject);
                
                mainCooldownTimer.ResetCooldown();
            }
        }

        private void InitMainCooldownTimer()
        {
            mainCooldownTimer = new CooldownTimer(mainShootingCooldown, true);
            mainCooldownTimer.SetTimerAsActive(true);
        }
    }
}