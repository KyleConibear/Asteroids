using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KyleConibear
{
    using static Logger;
    public sealed class LevelManager : MonoBehaviour
    {
        #region Fields
        [Header("General")]
        [SerializeField] private LevelUI levelUI = null;
        public LevelUI LevelUI
        {
            get
            {
                if (levelUI != null)
                {
                    return this.levelUI;
                }
                else
                {
                    levelUI = GameObject.FindObjectOfType<LevelUI>();

                    if (levelUI == null)
                    {
                        Logger.Log(Type.Error, $"LevelUI is {levelUI}.\n(Link in inspector.)");
                    }
                        
                    return levelUI;
                }
            }
        }

        [SerializeField] private Player player = null;

        [Header("Audio")]
        [SerializeField] private AudioClip levelMusicClip = null;

        [Header("Spawners")]
        [SerializeField] private AsteroidSpawner blueAsteroidSpawner = null;
        [SerializeField] private EnemySpawner enemySpawner = null;
        [SerializeField] private ExplosionSpawner explosionSpawner = null;

        [Header("Colliders")]
        [SerializeField] private Collider spawnAreaCollider = null;
        [SerializeField] private Collider playAreaCollider = null;

        [Header("Level Progression")]
        [SerializeField] private float levelStartDelay = 3.0f;
        [SerializeField] private int level = 0;
        [SerializeField] private int asteroidsRemaining = 0;
        [SerializeField] private int enemiesRemaining = 0;
        [SerializeField] private int rewardedPoints = 0;
        [SerializeField] private TMP_Text pointsText = null;
        #endregion

        #region Properties        
        private float playAreaYValue = Mathf.Infinity;
        public float PlayAreaYValue
        {
            get
            {
                if (this.playAreaYValue == Mathf.Infinity)
                {
                    this.playAreaYValue = this.playAreaCollider.bounds.center.y;
                }
                return this.playAreaYValue;
            }
        }

        private Vector4 playerAreaCoordinates = Vector4.positiveInfinity;
        /// <summary>
        /// x = xMin, y = xMax, z = zMin, w = zMax
        /// play area collider coordinates
        /// </summary>
        public Vector4 PlayerAreaCoordinates
        {
            get
            {
                if (playerAreaCoordinates.x == Mathf.Infinity)
                {
                    float[] bounds = this.playAreaCollider.Bounds();

                    this.playerAreaCoordinates = new Vector4(bounds[0], bounds[1], bounds[4], bounds[5]);
                    Logger.Log(Type.Message, $"playerAreaCoordinates {this.playerAreaCoordinates}");
                }

                return this.playerAreaCoordinates;
            }
        }

        private Vector4 spawnAreaCoordinates = Vector4.positiveInfinity;
        public Vector4 SpawnAreaCoordinates
        {
            get
            {
                if (spawnAreaCoordinates.x == Mathf.Infinity)
                {
                    float[] bounds = this.spawnAreaCollider.Bounds();

                    this.spawnAreaCoordinates = new Vector4(bounds[0], bounds[1], bounds[4], bounds[5]);
                    Logger.Log(Type.Message, $"playerAreaCoordinates {this.spawnAreaCoordinates}");
                }

                return this.spawnAreaCoordinates;
            }
        }
        #endregion

        public static Action<string, LevelManager> On_LevelLoaded;

        private void Awake()
        {
            if (On_LevelLoaded != null)
            {
                On_LevelLoaded.Invoke(SceneManager.GetActiveScene().name, this);
            }
            else
            {
                Logger.Log(Type.Warning, $"On_LevelLoaded Action is null.");
            }
        }

        private void Start()
        {
            AsteroidSpawner.On_AsteroidSpawned += (() => { this.asteroidsRemaining++; });
            Asteroid.On_AsteroidDestroyed += this.AsteroidDestroyed;

            EnemySpawner.On_EnemySpawned += (() => { this.enemiesRemaining++; });
            Enemy.On_EnemyKilled += this.EnemyKilled;

            AudioManager.Instance.PlayAudio(AudioManager.AudioChannel.Music, levelMusicClip);

            if (this.player == null)
            {
                Logger.Log(Type.Message, $"Player is {this.player}.\n(Spawning player.)");
                this.SpawnPlayer();
            }

            StartCoroutine(this.StartLevelDelay(this.levelStartDelay));
        }

        private void SpawnPlayer()
        {
            if (this.player != null)
            {
                Logger.Log(Type.Warning, "CANNOT SPAWN PLAYER!\n(Player already exists.)");
                return;
            }

            GameObject playerGO = Instantiate(Resources.Load("Player")) as GameObject;
            this.player = playerGO.transform.GetComponent<Player>() as Player;
        }

        public Vector3 GetPlayerPosition()
        {
            return this.player.spaceShipPosition;
        }

        private IEnumerator StartLevelDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            this.ProceedToNextLevel();
        }

        private void ProceedToNextLevel()
        {
            this.level++;
            this.SpawnBlueAsteroid(FibonacciSequence.GetValueAtIndex(this.level));
            this.SpawnEnemies(FibonacciSequence.GetValueAtIndex(this.level));
        }

        public void SpawnBlueAsteroid(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                this.blueAsteroidSpawner.Spawn();
            }
        }

        public void SpawnEnemies(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                this.enemySpawner.Spawn();
            }
        }

        public void SpawnExplosion(ExplosionSpawner.Type explosionType, Vector3 position)
        {
            this.explosionSpawner.Spawn(explosionType, position);
        }

        public void RewardPoints(int points)
        {
            this.rewardedPoints += points;
            this.pointsText.text = $"POINTS: {this.rewardedPoints}";
        }

        private void AsteroidDestroyed(Asteroid asteroid)
        {
            if (asteroid.transform.localScale.x > Asteroid.smallestSize)
            {
                this.blueAsteroidSpawner.SpawnAsteroidDebris(asteroid);
            }

            this.asteroidsRemaining--;
            this.RewardPoints(asteroid.RewardPoints);
            if (this.IsLevelComplete())
            {
                this.ProceedToNextLevel();
            }
        }

        private void EnemyKilled(Enemy enemy)
        {
            this.RewardPoints(enemy.RewardPoints);
            this.enemiesRemaining--;
            if (this.IsLevelComplete())
            {
                this.ProceedToNextLevel();
            }
        }

        private bool IsLevelComplete()
        {
            if (this.asteroidsRemaining < 1 && this.enemiesRemaining < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTargetWithinPlayArea(Vector3 target)
        {
            if (target.x < this.PlayerAreaCoordinates.x ||
                target.x > this.PlayerAreaCoordinates.y ||
                target.z < this.PlayerAreaCoordinates.z ||
                target.z > this.PlayerAreaCoordinates.w)
            {
                return false;
            }

            return true;
        }

        public Vector3 RandomLocationWithinPlayArea()
        {
            float xPosition = UnityEngine.Random.Range(PlayerAreaCoordinates.x, PlayerAreaCoordinates.y);
            float zPosition = UnityEngine.Random.Range(PlayerAreaCoordinates.z, PlayerAreaCoordinates.w);

            return new Vector3(xPosition, this.PlayAreaYValue, zPosition);
        }

        /// <summary>
        /// Get a random spawn position out of the players view
        /// </summary>
        /// <param name="yPos">Y position is not generated, provide the decided y position</param>
        /// <param name="offsets">This accounts for the size of the object to ensure it is spawned off-screen</param>
        /// <returns></returns>
        public Vector3 RandomOffScreenSpawnPosition(Vector3 offsets)
        {
            // Choose a random side to spawn an object
            // 0 top, 1 right, 2 bottom, 3 left
            int rndSide = UnityEngine.Random.Range(0, 4);

            float xPos;
            float zPos;

            switch (rndSide)
            {
                // Top
                case 0:
                xPos = UnityEngine.Random.Range((PlayerAreaCoordinates.x + offsets.x), (PlayerAreaCoordinates.y - offsets.x));
                zPos = PlayerAreaCoordinates.w + offsets.y;
                break;

                // Right
                case 1:
                xPos = PlayerAreaCoordinates.y + offsets.x;
                zPos = UnityEngine.Random.Range((PlayerAreaCoordinates.z + offsets.z), (PlayerAreaCoordinates.w - offsets.z));
                break;

                // Left
                case 2:
                xPos = PlayerAreaCoordinates.x - offsets.x;
                zPos = UnityEngine.Random.Range((PlayerAreaCoordinates.z + offsets.z), (PlayerAreaCoordinates.w - offsets.z));
                break;

                // Bottom
                default:
                xPos = UnityEngine.Random.Range((PlayerAreaCoordinates.x + offsets.x), (PlayerAreaCoordinates.z - offsets.x));
                zPos = PlayerAreaCoordinates.z - offsets.y;
                break;
            }
            return new Vector3(xPos, this.PlayAreaYValue, zPos);
        }
    }
}