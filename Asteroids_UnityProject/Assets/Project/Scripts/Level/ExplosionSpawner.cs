using UnityEngine;

namespace KyleConibear
{
    public class ExplosionSpawner : Spawner
    {
        [SerializeField] private GameObjectPool energyPool = null;

        public enum Type
        {
            RocketExplosion,
            EnergyExplosion
        }

        public void Spawn(Type type, Vector3? position = null)
        {
            switch (type)
            {
                case Type.RocketExplosion:
                base.pool.GetObject<GameObject>(true, position);
                break;

                case Type.EnergyExplosion:
                this.energyPool.GetObject<GameObject>(true, position);
                break;
            }
        }

        public override GameObject Spawn(Vector3? position = null)
        {
            Logger.Log(Logger.Type.Error, "Use \"Spawn(Type type, Vector3 ? position = null)\" instead.");
            throw new System.NotImplementedException();
        }
    }
}