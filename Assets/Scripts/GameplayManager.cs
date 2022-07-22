using AFSInterview.Towers;

namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject towerPrefab;
        [SerializeField] private GameObject burstTowerPrefab;

        [Header("Settings")] 
        [SerializeField] private Vector2 boundsMin;
        [SerializeField] private Vector2 boundsMax;
        [SerializeField] private float enemySpawnRate;

        [Header("UI")] 
        [SerializeField] private GameObject enemiesCountText;
        [SerializeField] private GameObject scoreText;
        
        private List<Enemy> enemies;
        //private float enemySpawnTimer;
        private int score;
        private CooldownTimer enemySpawnCooldownTimer;

        private void Awake()
        {
            enemies = new List<Enemy>();
            
            enemySpawnCooldownTimer = new CooldownTimer(enemySpawnRate, true);
            enemySpawnCooldownTimer.SetTimerAsActive(true);
        }

        private void Update()
        {
            enemySpawnCooldownTimer.UpdateByTime(Time.deltaTime);
            SpawnEnemyAferCooldown();

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, LayerMask.GetMask("Ground")))
                {
                    var spawnPosition = hit.point;
                    spawnPosition.y = towerPrefab.transform.position.y;

                    SpawnTower(spawnPosition);
                }
            }

            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
            enemiesCountText.GetComponent<TextMeshProUGUI>().text = "Enemies: " + enemies.Count;
        }

        private void SpawnEnemyAferCooldown()
        {
            if (enemySpawnCooldownTimer.IsOver == false) 
                return;
            SpawnEnemy();
            enemySpawnCooldownTimer.ResetCooldown();
        }

        private void SpawnEnemy()
        {
            var position = new Vector3(Random.Range(boundsMin.x, boundsMax.x), enemyPrefab.transform.position.y, Random.Range(boundsMin.y, boundsMax.y));
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
            enemy.OnEnemyDied += Enemy_OnEnemyDied;
            enemy.Initialize(boundsMin, boundsMax);

            enemies.Add(enemy);
        }

        private void Enemy_OnEnemyDied(Enemy enemy)
        {
            enemies.Remove(enemy);
            score++;
        }

        private void SpawnTower(Vector3 position)
        {
            //var tower = Instantiate(towerPrefab, position, Quaternion.identity).GetComponent<SimpleTower>();
            var tower = Instantiate(burstTowerPrefab, position, Quaternion.identity).GetComponent<BurstTower>();
            tower.Initialize(enemies);
        }
    }
}