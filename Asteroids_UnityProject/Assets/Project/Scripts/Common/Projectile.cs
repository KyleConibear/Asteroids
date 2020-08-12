using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Physics))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ExplosionSpawner.Type explosionType;

        [SerializeField] private Physics physics = null;

        [Range(1, 100)]
        [SerializeField] private float attackRange = 10.0f;

        [Range(1, 15)]
        [SerializeField] private float lifeSpawn = 12.0f;

        [Range(0, 100)]
        [SerializeField] private uint _damage = 1;
        public uint damage => this._damage;

        private float startTime = -1.0f;

        private bool canMove = false;
        private Vector3 startPosition = Vector3.zero;

        [SerializeField] private Type _type;
        public Type type => this._type;

        public enum Type
        {
            Player,
            Enemy
        }

        public void Fire()
        {
            // Rocket has already been fired wait for reset.
            if (this.canMove == true)
            {
                return;
            }
            else
            {
                this.startPosition = this.transform.position;
                this.canMove = true;
                this.startTime = Time.time;
                this.gameObject.SetActive(true);
            }
        }

        private void SpawnExplosion()
        {
            GameManager.Level.SpawnExplosion(explosionType, this.transform.position);
        }

        private void Reset()
        {
            this.SpawnExplosion();
            this.canMove = false;
            this.startTime = -1.0f;
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (this.lifeSpawn < 0)
                this.lifeSpawn = Mathf.Infinity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<Asteroid>() != null)
            {
                collision.transform.GetComponent<Asteroid>().Hit(_damage);
                this.gameObject.SetActive(false);
            }
            else if (this._type != Type.Enemy && collision.transform.GetComponent<Enemy>() != null)
            {
                collision.transform.GetComponent<Enemy>().Hit(_damage);
                this.gameObject.SetActive(false);
            }

            this.SpawnExplosion();

            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            bool isOutOfRange = this.transform.OutOfRange(this.startPosition, this.attackRange);
            bool isWithinPlayArea = GameManager.Level.IsTargetWithinPlayArea(this.transform.position);
            bool isLifeSpanExceed = Time.time > (this.startTime + this.lifeSpawn);

            if (isOutOfRange || !isWithinPlayArea || isLifeSpanExceed)
            {
                this.Reset();
                return;
            }
            else if (this.canMove)
            {
                this.physics.AddRelativeForce(Vector3.forward, ForceMode.VelocityChange); //AddRelativeForce(Vector2.up, ForceMode.Impulse);
            }
        }
    }
}