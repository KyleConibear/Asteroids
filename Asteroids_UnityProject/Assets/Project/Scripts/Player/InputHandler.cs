using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KyleConibear
{
    public class InputHandler : MonoBehaviour
    {
        #region Fields
        private Vector2 moveDirection = Vector2.zero;
        public Vector2 MoveDirection => this.moveDirection;


        public UnityEvent fireEvent = new UnityEvent();
        #endregion

        #region Methods       
        /// <summary>
        /// Player Input Callback Event capturing the left joystick & 'W','A','S','D' keys
        /// </summary>
        /// <param name="context">Data captured from the input device</param>
        public void Move(InputAction.CallbackContext context)
        {
            this.moveDirection = context.ReadValue<Vector2>();
        }

        public void Fire(InputAction.CallbackContext context)
        {
            this.fireEvent.Invoke();
        }
        #endregion
    }
}