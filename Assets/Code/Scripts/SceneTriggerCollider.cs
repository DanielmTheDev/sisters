﻿using Code.Scripts.SceneController;
using UnityEngine;

namespace Code.Scripts
{
    public class SceneTriggerCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            GetComponentInParent<BaseSceneController>().SceneTriggerEntered();
        }
    }
}