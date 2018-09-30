﻿using Code.Scripts.Entity;
using UnityEngine;

namespace Code.Scripts
{
    public class Projectile : MonoBehaviour
    {
        private const float ShootForce = 50;
        private const float RotationForce = 10;
        private Rigidbody2D rigidBody;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Shoot(Vector3 target)
        {
            Vector2 force = (target - gameObject.transform.position) * ShootForce;
            rigidBody.AddForce(force);
            rigidBody.AddTorque(RotationForce, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}