using UnityEngine; 

namespace KyleConibear
{
    using static Logger;

    [RequireComponent(typeof(Physics))]
    [RequireComponent(typeof(Weapon))]
    public class SpaceShip : MonoBehaviour
    {
        [SerializeField] private Physics physics = null;
        [Range(0, 1)]
        [SerializeField] private float horizontalMovementAllowance = 0.3f;
        [Range(0, 1)]
        [SerializeField] private float backwardsMovementAllowance = 0.1f;

        [SerializeField] private Meter _healthMeter = null;
        private Meter healthMeter
        {
            get
            {
                if(this._healthMeter != null)
                {
                    return this._healthMeter;
                }
                else
                {
                    this._healthMeter = this.GetComponent<Meter>();
                    if (this._healthMeter == null)
                    {
                        Logger.Log(Type.Error, $"{this.name} healthMeter is NULL.");
                    }
                    return this._healthMeter;
                }                
            }
        }

        [SerializeField] private Weapon weapons = null;

        #region MonoBehaviour
        private void OnCollisionEnter(Collision collision)
        {
            if (this.healthMeter.IsGreaterThanZero() == false)
                return;

            Projectile projectile = collision.transform.GetComponent<Projectile>();
            if (projectile != null)
            {              
                if(projectile.type == Projectile.Type.Enemy)
                {
                    this.TakeDamage(projectile.damage);
                }                
            }
        }
        #endregion

        #region Class Methods
        // Methods used by the Player class
        public void Move(Vector3 direction)
        {
            // Reduce horizontal movement
            float xValue = direction.x * this.horizontalMovementAllowance;
            float zValue = 0;

            // Prevent flying backwards
            if (direction.z < 0)
            {
                zValue = direction.z * this.backwardsMovementAllowance;
            }
            else
            {
                zValue = direction.z;
            }

            direction = new Vector3(xValue, 0, zValue);

            this.physics.AddRelativeForce(direction, ForceMode.Force);
        }
        public void Look(float yInput)
        {
            Vector3 direction = new Vector3(0, yInput, 0);
            this.physics.AddTorque(direction);
        }
        public void Fire()
        {
            this.weapons.Fire();
        }
        private void TakeDamage(uint damage)
        {
            this.healthMeter.Subtract(damage);

            GameManager.Level.LevelUI.UpdatePlayerHealth(this.healthMeter.GetPercentageRemaining());

            if(this.healthMeter.IsGreaterThanZero() == false)
            {
                // Spaceship has been destroyed
                this.physics.IsKinematic(true);
            }
        }
        #endregion
    }
}