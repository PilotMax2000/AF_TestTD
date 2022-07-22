using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview.Towers
{
    public class BurstTower : BaseTower
    {
        [Header("Burst Tower Settings")]
        [SerializeField] private float mainShootingCooldown;
        [SerializeField] private float burstShootingCooldown;
        [SerializeField] private int bulletsPerBurst;

        private CooldownTimer mainCooldownTimer;
        private CooldownTimer burstCooldownTimer;
        private int bulletsShotNumber;

        public override void Initialize(IReadOnlyList<Enemy> enemies)
        {
            base.Initialize(enemies);
            
            InitMainCooldownTimer();
            InitBulletCooldownTimer();
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
            burstCooldownTimer.UpdateByTime(Time.deltaTime);

            if (mainCooldownTimer.IsOver == false)
                return;
            
            if (burstCooldownTimer.IsOver == false)
                return;
            
            if (targetEnemy == null)
                return;
                    
            SpawnAndShootBullet();
            bulletsShotNumber++;
                    
            if (bulletsShotNumber >= bulletsPerBurst)
            {
                bulletsShotNumber = 0;
                mainCooldownTimer.ResetCooldown();
            }
            else
            {
                burstCooldownTimer.ResetCooldown();
            }
        }

        private void SpawnAndShootBullet()
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity)
                .GetComponent<Bullet>();
            bullet.Initialize(targetEnemy);
        }

        private void InitMainCooldownTimer()
        {
            mainCooldownTimer = new CooldownTimer(mainShootingCooldown, true);
            mainCooldownTimer.SetTimerAsActive(true);
        }
        private void InitBulletCooldownTimer()
        {
            burstCooldownTimer = new CooldownTimer(burstShootingCooldown, true);
            burstCooldownTimer.SetTimerAsActive(true);
        }
    }
}