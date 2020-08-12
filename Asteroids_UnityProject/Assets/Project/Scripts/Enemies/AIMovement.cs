using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Physics))]
    [RequireComponent(typeof(ObstacleAvoidance))]
    public class AIMovement : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Physics physics = null;
        [SerializeField] private ObstacleAvoidance obstacleAvoidance = null;
        public Vector3 position => this.transform.position;
        private float stopDistance;
        #endregion

        private void Start()
        {
            this.stopDistance = this.obstacleAvoidance.DistanceToColliderEdge.x * 2;
        }

        #region Class Methods
        /// <summary>
        /// Move towards given target
        /// </summary>
        /// <param name="target">The position in which to move towards</param>
        /// <returns>Returns true once target is reached</returns>
        public bool IsMovingToTarget(Vector3 target, float? stopDistance = null)
        {
            if (stopDistance == null)
                stopDistance = this.stopDistance;

            this.transform.LookAt(target);

            // Check if we are out of range of target
            if (this.transform.OutOfRange(target, (float)stopDistance))
            {
                if (this.obstacleAvoidance.IsAvoidingObstacle(target) == false)
                {
                    this.physics.AddForceTowardsTarget(target);
                }                

                return true; // Have not reached target
            }

            return false; // Target reached
        }
        #endregion
    }
}