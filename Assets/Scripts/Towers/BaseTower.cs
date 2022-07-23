using System;
using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview.Towers
{
    public abstract class BaseTower : MonoBehaviour
    {
        [Header("Base Tower Settings")]
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected Transform bulletSpawnPoint;
        [SerializeField] protected float firingRange;

        protected Enemy targetEnemy;
        protected bool targetEnemyWasFound;

        private IReadOnlyList<Enemy> enemies;

        public virtual void Initialize(IReadOnlyList<Enemy> enemies)
        {
            this.enemies = enemies;
        }

        private void Update() => 
            TowerUpdater();

        private void TowerUpdater()
        {
            UpdateTargetEnemy();
            UpdateFireRate();
        }

        protected virtual void UpdateTargetEnemy()
        {
            targetEnemy = FindClosestEnemy();
            targetEnemyWasFound = targetEnemy != null;
        }

        protected virtual void UpdateFireRate() { }

        private Enemy FindClosestEnemy()
        {
            Enemy closestEnemy = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var distance = (enemy.transform.position - transform.position).magnitude;
                if (distance <= firingRange && distance <= closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }
            return closestEnemy;
        }
        
        
    }
}