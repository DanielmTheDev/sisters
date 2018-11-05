﻿using Code.Classes;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.SceneController
{
    public class Level1SceneController : BaseSceneController
    {
        public Transform CameraTarget;
        public Transform JumpTarget;
        public Transform WalkTarget;

        public override void SceneTriggerEntered()
        {
            StartCoroutine(PlayEndingCutscene());
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(PlayOpeningCutscene(5, 4));
        }

        private IEnumerator GoToDiaperChanger()
        {
            Players[Character.Pollin].GoTo(WalkTarget.position, 1.2f);
            yield return new WaitForSeconds(2);
        }

        private IEnumerator JumpUpToDiaperChanger()
        {
            Transform pollin = Players[Character.Pollin].transform;
            pollin.GetComponent<Rigidbody2D>().isKinematic = true;
            while (pollin.position != JumpTarget.position)
            {
                float step = 3f * Time.deltaTime;
                pollin.position = Vector3.MoveTowards(pollin.position, JumpTarget.position, step);
                yield return null;
            }

            pollin.transform.Rotate(0, 0, 90);
        }

        private IEnumerator PlayEndingCutscene()
        {
            DisablePlayerMovement();
            DisablePlayerMovement();
            DisableFollowingCamera();
            Vector3 targetPosition = new Vector3(CameraTarget.position.x, CameraTarget.position.y,
                MainCamera.transform.position.z);
            StartCoroutine(MoveCamera(targetPosition));
            yield return new WaitForSeconds(6);
            ShowSpeechBubble(ActiveSpeechBubble);
            yield return ShowNextBubbleText(2);
            yield return GoToDiaperChanger();
            ActiveSpeechBubble.SetActive(false);
            yield return JumpUpToDiaperChanger();
            FadeSceneOut();
            yield return new WaitForSeconds(10);
            EnableNextScene();
        }
    }
}