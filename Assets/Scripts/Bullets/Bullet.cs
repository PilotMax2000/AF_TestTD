using UnityEngine;

namespace AFSInterview.Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Enemy targetObject;

        public void Initialize(Enemy target)
        {
            targetObject = target;
            targetObject.OnEnemyDied += OnEnemyDied;
        }

        private void Update()
        {
            var direction = (targetObject.transform.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;

            if ((transform.position - targetObject.transform.position).magnitude <= 0.2f)
            {
                Destroy(gameObject);
                Destroy(targetObject.gameObject);
            }
        }

        private void OnDisable()
        {
            if(targetObject != null) 
                targetObject.OnEnemyDied -= OnEnemyDied;
        }

        private void OnEnemyDied(Enemy obj) => 
            Destroy(this.gameObject);
    }
}