using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(Meter))]
    public class Player : MonoBehaviour
    {
        #region Fields
        [SerializeField] private InputHandler inputHandler = null;
        [SerializeField] private SpaceShip spaceShip = null;
        [SerializeField] private bool isAlive = true;
        #endregion

        #region Properties
        public Vector3 spaceShipPosition => this.spaceShip.transform.position;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            this.inputHandler.fireEvent.AddListener(this.spaceShip.Fire);
        }

        private void FixedUpdate()
        {
            if (isAlive == false)
                return;

            this.spaceShip.Move(this.inputHandler.MoveDirection.y * Vector3.forward);
            this.spaceShip.Look(this.inputHandler.MoveDirection.x);
        }
        #endregion

        public void Death()
        {
            this.isAlive = false;
        }
    }
}