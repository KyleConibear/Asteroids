using UnityEngine;


namespace KyleConibear
{
    public abstract class Spawner : MonoBehaviour
    {
        [SerializeField] protected GameObjectPool pool = null;

        public abstract GameObject Spawn(Vector3? position = null);
    }
}