using UnityEngine;

namespace KyleConibear
{
    public class AIBehaviour : MonoBehaviour
    {
        #region Fields              
        [SerializeField] private AIMovement movement = null;
        [SerializeField] private Weapon weapon = null;
        [SerializeField] private Behaviour behaviour = Behaviour.Patrol;
        [SerializeField] private float targetDiscoveryRange = 50;
        [SerializeField] private float attackRange = 25;

        /// <summary>
        /// AI Behavior follows linearly 
        /// </summary>
        public enum Behaviour
        {
            /// <summary>
            /// Move to random location within play area
            /// while searching for target.
            /// </summary>
            Patrol = 0,

            /// <summary>
            /// Within range of target,
            /// attack target.
            /// </summary>
            Attack = 1
        }

        private Vector3? target = null;
        private Vector3 Target
        {
            get
            {
                return (Vector3)this.target;
            }
        }
        #endregion

        #region MonoBehaviour Methods

        private void Update()
        {
            this.UpdateBehaviour();
            this.ExecuteBehavior();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.behaviour = Behaviour.Attack;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.target = null;
                this.behaviour = Behaviour.Patrol;
            }
        }
        #endregion

        #region Class Methods
        private void UpdateBehaviour()
        {
            float distanceToPlayer = Vector3.Distance(GameManager.Level.GetPlayerPosition(), this.movement.position);

            if (distanceToPlayer <= this.targetDiscoveryRange)
            {
                this.behaviour = Behaviour.Attack;
            }
            else
            {
                ////this.behaviour = Behaviour.Patrol;
            }                
        }

        private void ExecuteBehavior()
        {
            switch (this.behaviour)
            {
                case Behaviour.Patrol:
                this.Patrol();
                break;

                case Behaviour.Attack:
                this.Attack();
                break;
            }
        }

        private void Patrol()
        {
            if (this.target == null)
            {
                this.target = GameManager.Level.RandomLocationWithinPlayArea();
            }

            if (this.movement.IsMovingToTarget(this.Target) == false)
            {
                this.target = null;
            }
        }

        private void Attack()
        {
            this.movement.IsMovingToTarget(GameManager.Level.GetPlayerPosition(), this.attackRange);
            this.weapon.Fire();
        }
    }
    #endregion
}