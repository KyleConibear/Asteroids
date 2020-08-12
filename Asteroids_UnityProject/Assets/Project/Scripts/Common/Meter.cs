using UnityEngine;
using UnityEngine.Events;

namespace KyleConibear{
    public class Meter : MonoBehaviour
    {
        [SerializeField] private uint max = 1;
        [SerializeField] private uint current = 1;

        public UnityEvent OnDepleted = new UnityEvent();
        public UnityEvent OnFull = new UnityEvent();

        private void Awake()
        {
            this.OnDepleted.AddListener(this.Depleted);
            this.OnFull.AddListener(this.Full);
        }

        public void Subtract(uint amount)
        {
            this.current -= amount;

            if (this.current <= 0)
            {
                this.OnDepleted.Invoke();
            }
        }

        public void Add(uint amount)
        {
            this.current += amount;

            if (this.current >= this.max)
            {
                this.OnFull.Invoke();
            }
        }

        /// <summary>
        /// Set the current amount to the max
        /// </summary>
        public void Fill()
        {
            this.current = this.max;
        }

        public bool IsGreaterThanZero()
        {
            return this.current > 0;
        }

        public float GetPercentageRemaining()
        {
            return (float)this.current / (float)this.max;
        }

        private void Depleted()
        {
            this.current = 0;
        }

        private void Full()
        {
            this.current = this.max;
        }
    }
}