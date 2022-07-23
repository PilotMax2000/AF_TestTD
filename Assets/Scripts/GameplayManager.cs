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
        [SerializeField] private TextMeshProUGUI enemiesCountText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        private List<Enemy> enemies;
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
            SpawnEnemyAfterCooldown();

            if (Input.GetMouseButtonDown(0)) 
                SpawnTowerOnMousePosition();

            UpdateUI();
        }

        private void UpdateUI()
        {
            scoreText.text = "Score: " + score;
            enemiesCountText.text = "Enemies: " + enemies.Count;
        }

        private void SpawnTowerOnMousePosition()
        {
            var ray = Helpers.Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, LayerMask.GetMask("Ground")) == false)
                return;
            var spawnPosition = hit.point;
            spawnPosition.y = towerPrefab.transform.position.y;

            SpawnTower(spawnPosition);
        }

        private void SpawnEnemyAfterCooldown()
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