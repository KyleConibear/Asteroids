using System;
using System.Collections;
using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Physics))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Meter))]
    [RequireComponent(typeof(ScreenWarp))]
    public class Asteroid : MonoBehaviour
    {
        #region Fields
        public static Action<Asteroid> On_AsteroidDestroyed;
        [SerializeField] private Physics physics = null;
        [SerializeField] private Meter healthMeter = null;
        [SerializeField] private new Collider collider = null;
        public Collider Collider => this.collider;

        [Range(10, 50)]
        [SerializeField] private float spawnForce = 15;

        [Range(10, 50)]
        [SerializeField] private float debrisForce = 30;

        [SerializeField] private int rewardPoints = 0;
        public int RewardPoints => this.rewardPoints;

        public enum DebrisPosition
        {
            TopRight = 1,
            TopLeft = 2,
            BottomLeft = 3,
            BottomRight = 4
        }

        public static readonly int OrginalScale = 10;
        public static readonly int smallestSize = 5;
        #endregion

        #region MonoBehaviour Methods
        private void Start()
        {
            this.healthMeter.OnDepleted.AddListener(this.Destroyed);

            if (this.transform.localScale.x == OrginalScale)
            {
                this.physics.Force = this.spawnForce;
                this.physics.AddForceTowardsTarget(GameManager.Level.RandomLocationWithinPlayArea(), ForceMode.Impulse);
            }
        }

        private void OnEnable()
        {
            this.healthMeter.Fill();
            this.physics.IsKinematic(false);
            this.physics.AddRandomRotation();
        }
        #endregion

        #region Class Methods
        // Public
        public void SetScale(int size)
        {
            this.transform.localScale = new Vector3(size, size, size); ;
        }

        /// <summary>
        /// Waits until next frame before applying force
        /// </summary>
        /// <param name="angle">Apply force in the direction of the angle</param>
        public void AddForceToDebris(float angle)
        {
            this.physics.Force = this.debrisForce;
            this.physics.AddForceAtAngle(angle, ForceMode.Impulse);
        }

        public void Hit(uint damage)
        {
            this.healthMeter.Subtract(damage);
        }

        // Private
        private void Destroyed()
        {
            this.physics.ResetVelocity();
            this.physics.ResetAngularVelocity();
            this.physics.IsKinematic(true);

            On_AsteroidDestroyed.Invoke(this);
            this.gameObject.SetActive(false);
        }
        #endregion
    }
}