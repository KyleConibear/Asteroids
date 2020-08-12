using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyleConibear
{
    [RequireComponent(typeof(Meter))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Meter healthMeter = null;
        [SerializeField] private Collider collider;
        public Collider Collider => this.collider;
        public static Action<Enemy> On_EnemyKilled;

        [SerializeField] private int rewardPoints = 0;
        public int RewardPoints => this.rewardPoints;

        private void Start()
        {
            this.healthMeter.OnDepleted.AddListener(Killed);
        }

        public void Hit(uint damage)
        {
            this.healthMeter.Subtract(damage);
        }

        private void Killed()
        {
            On_EnemyKilled.Invoke(this);
            this.gameObject.SetActive(false);
        }
    }
}