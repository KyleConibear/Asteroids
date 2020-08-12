using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Rigidbody))]
    public class Physics : MonoBehaviour
    {
        #region Fields
        [Header("Components")]
        [SerializeField] private new Rigidbody rigidbody = null;

        [Header("Position")]
        [Range(0.01f, 1000)]
        [SerializeField] private float force = 10.0f;
        public float Force
        {
            set
            {
                this.force = value;
            }
        }

        [Range(1, 1000)]
        [SerializeField] private float maxVelocity = 50.0f;

        [Header("Rotation")]
        [Range(0.01f, 1000)]
        [SerializeField] private float torque = 10.0f;

        [Range(1, 1000)]
        [SerializeField] private float maxAngularVelocity = 50.0f;
        #endregion

        #region MonoBehaviour Methods
        private void OnDisable()
        {
            this.ResetVelocity();
        }
        #endregion

        public void IsKinematic(bool enable)
        {
            this.rigidbody.isKinematic = enable;
        }

        #region Velocity
        public void SetVelocity(Vector3 direction)
        {
            Vector3 velocity = direction * this.force;
            this.rigidbody.velocity = velocity;
            this.RestrictVelocity();
        }

        public Vector3 GetVelocity()
        {
            return this.rigidbody.velocity;
        }

        public void ResetVelocity()
        {
            this.rigidbody.velocity = Vector3.zero;
        }

        public void AddRandomRotation()
        {
            int rndAxis = Random.Range(0, 3);
            Vector3 axis = Vector3.zero;

            switch (rndAxis)
            {
                case 0:
                axis.x = Random.value;
                break;
                case 1:
                axis.y = Random.value;
                break;
                case 2:
                axis.z = Random.value;
                break;
            }

            this.rigidbody.angularVelocity = axis * this.torque;
        }
        #endregion

        #region Force
        /// <summary>
        /// Move an object along x and z axis by applying force in the given direction
        /// </summary>
        /// <param name="direction">"direction.y" will be converted to the z axis</param>
        public void AddForce(Vector3 direction, ForceMode mode = ForceMode.Force)
        {
            Vector3 force = direction * this.force;
            this.rigidbody.AddForce(force, mode);
            this.RestrictVelocity();
        }
        
        public void AddRelativeForce(Vector3 direction, ForceMode mode = ForceMode.Force)
        {
            Vector3 force = direction * this.force;
            this.rigidbody.AddRelativeForce(force, mode);
            this.RestrictVelocity();
        }

        public void AddForceAtAngle(float angle, ForceMode mode = ForceMode.Force)
        {           
            float xValue = Mathf.Cos(angle * Mathf.PI / 180);
            float zValue = Mathf.Sin(angle * Mathf.PI / 180);
            Vector3 direction = new Vector3(xValue, 0, zValue);
            direction *= this.force;
            this.AddForce(direction, mode);
        }

        public void AddForceTowardsTarget(Vector3 target, ForceMode mode = ForceMode.Force)
        {
            //float angle = Mathf.Atan2(target.y - this.transform.position.y, target.x - this.transform.position.x) * 180 / Mathf.PI;
            //this.AddForceAtAngle(angle, mode);

            Vector3 direction = target - this.transform.position;
            this.AddForce(direction.normalized, mode);

        }

        private void RestrictVelocity()
        {
            float xClamp = Mathf.Clamp(this.rigidbody.velocity.x, -this.maxVelocity, this.maxVelocity);
            float yClamp = Mathf.Clamp(this.rigidbody.velocity.y, -this.maxVelocity, this.maxVelocity);
            float zClamp = Mathf.Clamp(this.rigidbody.velocity.z, -this.maxVelocity, this.maxVelocity);
            Vector3 restrictedVelocity = new Vector3(xClamp, yClamp, zClamp);
            this.rigidbody.velocity = restrictedVelocity;
        }
        #endregion

        #region Torque
        /// <summary>
        /// Rotate an object on the y axis by applying a positive or negative torque.
        /// </summary>
        /// <param name="direction">A positive value will rotate clockwise and a negative value counter-clockwise</param>
        public void AddTorque(Vector3 axis)
        {
            Vector3 torque = axis * this.torque;
            this.rigidbody.AddTorque(torque);
            this.rigidbody.maxAngularVelocity = this.maxAngularVelocity;
        }

        public void TurnTowardsTarget(Vector3 target)
        {
            Vector3 relativeVector = this.transform.InverseTransformPoint(target);
            relativeVector /= relativeVector.magnitude;
            this.AddTorque(new Vector3(0, relativeVector.x, 0));
        }

        public void ResetAngularVelocity()
        {
            this.rigidbody.angularVelocity = Vector3.zero;
            print($"ResetAngularVelocity {this.rigidbody.angularVelocity}");
        }
        #endregion
    }
}