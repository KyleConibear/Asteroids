using UnityEngine;

namespace KyleConibear
{
    using static Logger;

    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObjectPool projectilePool = null;
        [SerializeField] private Transform[] projectileSpawnPosition = new Transform[0];
        private uint nextSpawnIndex = 0;

        [Range(0.1f, 10)]
        [SerializeField] private float rateOfFire = 0.25f;
        private float nextFire = -1.0f;

        public void Fire()
        {
            if (Time.time < this.nextFire)
                return;

            this.nextFire = Time.time + this.rateOfFire;
            Projectile projectile = this.projectilePool.GetObject<Projectile>(true).GetComponent<Projectile>() as Projectile;
            Transform transform = this.GetSpawnTransform();
            projectile.transform.position = transform.position;
            // Logger.Log(Type.Message, $"projectile.transform.position={projectile.transform.position}");
            projectile.transform.rotation = transform.rotation;
            // Logger.Log(Type.Message, $"projectile.transform.rotation={projectile.transform.rotation.eulerAngles}");
            projectile.Fire();
        }

        private Transform GetSpawnTransform()
        {
            Transform transform;
            if (this.projectileSpawnPosition.Length > 0)
                transform = this.projectileSpawnPosition[this.NextSpawnPositionIndex()];
            else
                transform = this.transform;
            return transform;
        }

        private uint NextSpawnPositionIndex()
        {
            uint nextIndex = this.nextSpawnIndex;

            this.nextSpawnIndex = (this.nextSpawnIndex + 1) % (uint)projectileSpawnPosition.Length;

            return nextIndex;
        }
    }
}