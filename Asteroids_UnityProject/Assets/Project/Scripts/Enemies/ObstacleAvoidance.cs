using UnityEngine;

namespace KyleConibear
{
    using static Logger;

    [RequireComponent(typeof(Physics))]
    [RequireComponent(typeof(Collider))]
    public class ObstacleAvoidance : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Physics physics = null;
        [SerializeField] private new Collider collider = null;

        [Header("Sensors")]
        // Ignore layer 12 (boundary) & 13 (enemy projectile)
        private LayerMask ignoreLayer = ~(1 << 12 & 1 << 13);

        [Range(1, 100)]
        [SerializeField] private float detectionRange = 10.0f;

        [Range(0, 30)]
        [SerializeField] private float sensorAngle = 15.0f;

        [Header("Offset")]
        [Tooltip("The amount to offset the current direction of travel to avoid an obstacle")]
        [Range(1, 5)]
        [SerializeField] private float correctionAmount = 2.0f;

        [Range(0.01f, 1)]
        [Tooltip("If a potential collision is detected, add force in the opposite direction")]
        [SerializeField] private float collisionAvoidanceForce = 0.1f;

        private int previousDirectionDecision = 0;

        // Track whether the previous iteration was avoiding an obstacle
        private bool previouslyDetected = false;

        private float distanceFromObstacle = Mathf.Infinity;
        #endregion

        #region Properties
        /// <summary>
        /// The distance from the center of the object to end of the collider
        /// </summary>
        private Vector3 distanceToColliderEdge = Vector3.zero;
        public Vector3 DistanceToColliderEdge
        {
            get
            {
                if (this.distanceToColliderEdge == Vector3.zero)
                {
                    float xVal = this.collider.bounds.extents.x;
                    float yVal = this.collider.bounds.extents.y;
                    float zVal = this.collider.bounds.extents.z;

                    this.distanceToColliderEdge = new Vector3(xVal, yVal, zVal);
                }
                return this.distanceToColliderEdge;
            }
        }
        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            this.detectionRange += this.DistanceToColliderEdge.z;
        }
        #endregion

        #region Class Methods

        #region Avoidance

        /// <summary>
        /// The logic for steering around obstacles
        /// </summary>
        /// <param name="target">The target location being traveled towards</param>
        /// <returns>Whether an obstacle has been detected</returns>
        public bool IsAvoidingObstacle(Vector3 target)
        {
            bool isObstacleLeft = this.IsLeftSideObstacle();
            bool isObstacleCenter = this.IsSensorObstructed(this.transform.position, (this.detectionRange * 1.3f), Color.blue);
            bool isObstacleRight = this.IsRightSideObstacle();

            // Obstacle on both left, center and right side
            if ((isObstacleLeft && isObstacleCenter && isObstacleRight) || isObstacleCenter) // Obstacle only in center
            {
                this.AvoidCenterObstacle(target);
            }
            // Obstacle on left but not right
            else if (isObstacleLeft && isObstacleRight == false)
            {
                this.AvoidLeftObstacle();
                Logger.Log(Type.Message, "Going right", true);
            }
            // Obstacle on right but not left
            else if (isObstacleRight && isObstacleLeft == false)
            {
                this.AvoidRightObstacle();
                Logger.Log(Type.Message, "Going Left", true);
            }

            if (isObstacleLeft || isObstacleCenter || isObstacleRight)
            {
                this.previouslyDetected = true;

                return true;
            }
            else // No obstacles
            {
                // First iteration after an obstacle avoidance
                if (this.previouslyDetected)
                {
                    this.ObstacleAvoided();
                }

                return false;
            }
        }

        private void CollisionAvoidanceForce(Vector3 collisionPosition)
        {
            // Calculate Angle Between the collision point
            Vector3 direction = collisionPosition - this.transform.position;

            // We then get the opposite (-Vector3) and normalize it
            direction = -direction.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            this.physics.AddForce(direction * this.collisionAvoidanceForce);
        }

        private void AvoidCenterObstacle(Vector3 target)
        {
            Vector3 torque = Vector3.zero;

            // Turn away from object if the object is angled
            if (this.CenterSensorRedirection() > 0.1f || this.CenterSensorRedirection() < -0.1f)
            {
                torque.y += this.CenterSensorRedirection();
            }
            // If it is a flat surface turn towards target
            else
            {
                float targetDirectionAngle = Math.BinaryPositionFromRelativeAngle(target);

                // The target is directly in front. Move in random direction
                if (targetDirectionAngle == 0 && this.previouslyDetected == false)
                {
                    Logger.Log(Type.Message, "Forced to choose random direction around obstacle", true);
                    int[] direction = new int[2] { -1, 1 };
                    this.previousDirectionDecision = direction[Random.Range(0, 2)];
                }

                Logger.Log(Type.Message, $"Center obstacle target direction {targetDirectionAngle}", true);
                torque.y = targetDirectionAngle;
                torque.y += this.previousDirectionDecision;
            }
            torque *= this.correctionAmount;
            this.physics.AddTorque(torque);
        }

        private float CenterSensorRedirection()
        {
            RaycastHit hit;
            float angle = this.transform.rotation.eulerAngles.y;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * Vector3.forward;

            if (UnityEngine.Physics.Raycast(this.transform.position, direction, out hit, Mathf.Infinity))
            {
                Debug.DrawLine(this.transform.position, hit.point, Color.blue);
                return hit.normal.x;
            }
            return (0);
        }

        private void AvoidLeftObstacle()
        {
            Vector3 torque = new Vector3(0, this.correctionAmount, 0);
            this.physics.AddTorque(torque);
        }

        private void AvoidRightObstacle()
        {
            Vector3 torque = new Vector3(0, -this.correctionAmount, 0);
            this.physics.AddTorque(torque);
        }

        /// <summary>
        /// Can obstacle was successfully avoid. Reset global temporary cache variable
        /// </summary>
        private void ObstacleAvoided()
        {
            this.previousDirectionDecision = 0;
            this.previouslyDetected = false;
            this.distanceFromObstacle = Mathf.Infinity;
            this.physics.ResetAngularVelocity();
        }
        #endregion

        #region Detection
        private bool IsLeftSideObstacle()
        {
            Vector3 leftSensorPosition;
            if (this.IsFrontLeftSensorObstructed(out leftSensorPosition) ||
                // Left right angle sensor, rotate "FrontLeftSensorDetection" on y axis
                this.IsSensorObstructed(leftSensorPosition, this.detectionRange, Color.green, -this.sensorAngle))
            {
                return true;
            }
            return false;
        }

        private bool IsRightSideObstacle()
        {
            Vector3 rightSensorPosition;
            if (this.IsFrontRightSensorObstructed(out rightSensorPosition) ||
            // Front right angle sensor, rotate "FrontRightSensorDetection" on y axis
            this.IsSensorObstructed(rightSensorPosition, this.detectionRange, Color.red, this.sensorAngle))
            {
                return true;
            }
            return false;
        }

        private bool IsFrontRightSensorObstructed(out Vector3 startingPos)
        {
            startingPos = this.transform.position;
            startingPos += this.transform.forward;
            startingPos += this.transform.right * this.DistanceToColliderEdge.x;

            return (this.IsSensorObstructed(startingPos, this.detectionRange, Color.red));
        }

        private bool IsFrontLeftSensorObstructed(out Vector3 startingPos)
        {
            startingPos = this.transform.position;
            startingPos += this.transform.forward;
            startingPos -= this.transform.right * this.DistanceToColliderEdge.x;

            return (this.IsSensorObstructed(startingPos, this.detectionRange, Color.green));
        }

        private bool IsSensorObstructed(Vector3 startingPos, float range, Color color, float angle = 0)
        {
            RaycastHit hit;
            angle += this.transform.rotation.eulerAngles.y;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * Vector3.forward;

            if (UnityEngine.Physics.Raycast(startingPos, direction, out hit, range, ~ignoreLayer))
            {
                this.CheckDistanceFromObstacle(hit.point);

                this.CollisionAvoidanceForce(hit.point);

                Debug.DrawLine(startingPos, hit.point, color, 1);
                return true;
            }

            return false;
        }

        private void CheckDistanceFromObstacle(Vector3 obstaclePosition)
        {
            float distance = Vector3.Distance(this.transform.position, obstaclePosition);

            if (distance < this.distanceFromObstacle)
            {
                this.distanceFromObstacle = distance - this.DistanceToColliderEdge.z; ;
            }
        }
        #endregion
        #endregion
    }
}