﻿using Code.Classes.CombatController;
using Code.Classes.MovementController;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.Entity
{
    public class Spider : BaseEnemy
    {
        public bool Patrolling;

        private void Start()
        {
            WalkingSpeed = 2;
            CombatController = new EnemyCombatController(gameObject, 1);
            MovementController = new PatrollingEnemyMovementController(gameObject, WalkingSpeed);
            if (Patrolling)
                StartPatrolling();
            StartCoroutine(JumpRandomly());
        }

        private IEnumerator JumpRandomly()
        {
            while (true)
            {
                yield return new WaitForSeconds(2 + Random.value * 3);
                MovementController.Jump();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.tag.Contains("Weapon"))
                return;
            CombatController.ReceiveHit(collision);
        }

        protected override IEnumerator Patrol()
        {
            while (true)
            {
                MovementController.Move(1);
                yield return new WaitForSeconds(1);
                MovementController.Move(-1);
                yield return new WaitForSeconds(1);
            }
        }
    }
}