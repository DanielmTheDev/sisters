﻿using Code.Classes.MovementController;
using UnityEngine;

namespace Code.Scripts.Character
{
    public class Player : BaseCharacter
    {
        public Animator Animator;
        public AudioSource Swing;
        public PolygonCollider2D SwordCollider;
        public Transform GroundCheck;

        private void Start()
        {
            MovementController = new PlayerMovementController(gameObject, GroundCheck);
        }

        private void Update()
        {
            CheckJump();
            CheckFire();
        }

        private void FixedUpdate()
        {
            CheckMove();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.tag.Contains("Enemy"))
                return;
            Die();
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void CheckFire()
        {
            if (!Input.GetButtonDown("Fire1"))
                return;
            Animator.SetTrigger("OnAttackDown");
            Swing.Play();
        }

        private void CheckMove()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            MovementController.Move(horizontal);
        }

        private void CheckJump()
        {
            if (Input.GetButtonDown("Jump"))
                MovementController.Jump();
        }
    }
}