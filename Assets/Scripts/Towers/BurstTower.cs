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
        [SerializeField] private int shootingMaxForce = 2000;

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
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

            var enemyTargetCurrentPosition = targetEnemy.transform.position;
            var enemyTargetPositionAfterTimeBuffer = enemyTargetCurrentPosition + targetEnemy.Direction * targetEnemy.Speed * 0.3f;
            //var enemyTargetPosition = targetEnemy.transform.position;
            float distanceToEnemy = Vector3.Distance(transform.position, enemyTargetPositionAfterTimeBuffer);
            float resForceDueToDistance = distanceToEnemy / firingRange * shootingMaxForce;
            Vector3 directionToEnemy = enemyTargetPositionAfterTimeBuffer - bullet.transform.position;
            bullet.GetComponent<Rigidbody>().AddForce(
                directionToEnemy.x * resForceDueToDistance,
                directionToEnemy.y * resForceDueToDistance,
                directionToEnemy.z * resForceDueToDistance);
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