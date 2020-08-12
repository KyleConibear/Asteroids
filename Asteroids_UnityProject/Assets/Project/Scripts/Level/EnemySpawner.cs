using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyleConibear
{
    public class EnemySpawner : Spawner
    {
        public static Action On_EnemySpawned;

        public override GameObject Spawn(Vector3? position = null)
        {
            Enemy enemy;
            if (position != null)
            {
                enemy = this.pool.GetObject<Enemy>(true, (Vector3)position).GetComponent<Enemy>() as Enemy;
            }
            else
            {
                enemy = this.pool.GetObject<Enemy>(true).GetComponent<Enemy>() as Enemy;
            }

            enemy.transform.position = GameManager.Level.RandomOffScreenSpawnPosition(enemy.Collider.GetBoundsExtents(false));
            On_EnemySpawned.Invoke();
            return enemy.gameObject;
        }
    }
}