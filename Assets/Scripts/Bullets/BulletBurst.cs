using UnityEngine;

namespace AFSInterview.Bullets
{
    public class BulletBurst : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            GameObject target = collision.gameObject;
            
            if(target.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
            else if(target.GetComponent<Enemy>() != null)
            {
                Destroy(gameObject);
                Destroy(target);
            }
        }
    }
}