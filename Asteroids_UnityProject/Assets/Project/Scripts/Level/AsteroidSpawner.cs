﻿using System;
using UnityEngine;

namespace KyleConibear
{
    public class AsteroidSpawner : Spawner
    {
        public static Action On_AsteroidSpawned;

        public override GameObject Spawn(Vector3? position = null)
        {
            Asteroid asteroid = this.pool.GetObject<Asteroid>(true, position).GetComponent<Asteroid>() as Asteroid;
            asteroid.SetScale(Asteroid.OrginalScale);
            asteroid.transform.position = GameManager.Level.RandomOffScreenSpawnPosition(asteroid.Collider.GetBoundsExtents(true));

            On_AsteroidSpawned.Invoke();

            return asteroid.gameObject;
        }

        /// <summary>
        /// Spawn four asteroids which are half the size of the "parentAsteroid" unless that size is smaller than the smallest allowed asteroid size
        /// </summary>
        /// <param name="parentAsteroid">The asteroid which is calling this method.</param>
        public void SpawnAsteroidDebris(Asteroid parentAsteroid)
        {
            for (int i = 1; i < 5; i++)
            {
                Asteroid asteroid = base.pool.GetObject<Asteroid>(false).GetComponent<Asteroid>() as Asteroid;

                asteroid.SetScale((int)parentAsteroid.transform.localScale.x / 2);

                Vector3 spawnPos;
                float forceAngle;

                asteroid.gameObject.SetActive(true);
                this.DebrisSpawnPositin(parentAsteroid, (Asteroid.DebrisPosition)i, asteroid.Collider.GetBoundsExtents(false), out spawnPos, out forceAngle);
                asteroid.transform.position = spawnPos;
                asteroid.AddForceToDebris(forceAngle);
                On_AsteroidSpawned.Invoke();
            }
        }

        private void DebrisSpawnPositin(Asteroid parentAsteroid, Asteroid.DebrisPosition type, Vector3 edgeOffset, out Vector3 spawnPos, out float forceAngle)
        {
            spawnPos = parentAsteroid.transform.position;

            // Add force so that debris flies outward from the corn they spawned in
            forceAngle = -45 + (90 * (int)type);

            float xOffset = parentAsteroid.Collider.GetBoundsExtents(false).x - edgeOffset.x;
            float zOffset = parentAsteroid.Collider.GetBoundsExtents(false).z - edgeOffset.z;

            switch (type)
            {
                case Asteroid.DebrisPosition.TopLeft:
                spawnPos.x -= xOffset;
                spawnPos.z += zOffset;
                break;

                case Asteroid.DebrisPosition.TopRight:
                spawnPos.x += xOffset;
                spawnPos.z += zOffset;
                break;

                case Asteroid.DebrisPosition.BottomLeft:
                spawnPos.x -= xOffset;
                spawnPos.z -= zOffset;
                break;

                default:
                spawnPos.x += xOffset;
                spawnPos.z -= zOffset;
                break;
            }
        }
    }
}