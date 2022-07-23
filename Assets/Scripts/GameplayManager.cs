using System.Linq;
using AFSInterview.Towers;
using UnityEngine.UI;

namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private BaseTower towerPrefab;
        [SerializeField] private BaseTower burstTowerPrefab;

        [Header("Settings")] 
        [SerializeField] private Vector2 boundsMin;
        [SerializeField] private Vector2 boundsMax;
        [SerializeField] private float enemySpawnRate;

        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI enemiesCountText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private ToggleGroup towerTypeToggleGroup;
        [SerializeField] private Toggle burstTowerToggle;
        
        private List<Enemy> enemies;
        private int score;
        private CooldownTimer enemySpawnCooldownTimer;

        private void Awake()
        {
            enemies = new List<Enemy>();
            
            enemySpawnCooldownTimer = new CooldownTimer(enemySpawnRate, true);
            enemySpawnCooldownTimer.SetTimerAsActive(true);
            
            UpdateUI();
        }

        private void Update()
        {
            enemySpawnCooldownTimer.UpdateByTime(Time.deltaTime);
            SpawnEnemyAfterCooldown();

            if (Input.GetMouseButtonDown(0)) 
                SpawnTowerOnMousePosition();
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
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.OnEnemyDied += Enemy_OnEnemyDied;
            enemy.Initialize(boundsMin, boundsMax);

            enemies.Add(enemy);
            UpdateUI();
        }

        private void Enemy_OnEnemyDied(Enemy enemy)
        {
            enemies.Remove(enemy);
            score++;
            UpdateUI();
        }

        private void SpawnTower(Vector3 position)
        {
            var activeToggles = towerTypeToggleGroup.ActiveToggles();
            if (!activeToggles.Any())
                return;

            BaseTower tower = Instantiate(activeToggles.First() == burstTowerToggle 
                ? burstTowerPrefab 
                : towerPrefab, position, Quaternion.identity);

            tower.Initialize(enemies);
        }
    }
}