﻿using System.Collections;
using Code.Classes;
using UnityEngine;

namespace Code.Scripts.SceneController
{
    public class Level4SceneController : BaseSceneController
    {
        [SerializeField] private GameObject clown;
        private Animator clownAnimator;
        [SerializeField] private Transform clownCameraTarget;
        [SerializeField] private AudioClip baepsaeClip;
        [SerializeField] private AudioSource mainSource;

        protected override void HandleTrigger()
        {
            IgnoreTrigger = true;
            DisableCameraAndMovement();
            StartCoroutine(ClownCutscene());
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(PlayOpeningCutscene(1, 3));
            clownAnimator = clown.GetComponent<Animator>();
        }

        private IEnumerator ClownCutscene()
        {
            yield return Talk();
            Vector3 targetPosition = new Vector3(clownCameraTarget.position.x, clownCameraTarget.position.y,
                MainCamera.transform.position.z);
            ChangeMusic();
            yield return MoveCameraSmoothly(targetPosition);
            yield return Dance();
            StartCoroutine(StartPunching());
        }

        private IEnumerator StartPunching()
        {
            while (true)
            {
                yield return new WaitForSeconds(7);
                clownAnimator.SetTrigger("Punch");
            }
        }

        private void ChangeMusic()
        {
            mainSource.clip = baepsaeClip;
            mainSource.Play();
        }

        private IEnumerator Dance()
        {
            clownAnimator.SetBool("Dance", true);
            yield return new WaitForSeconds(7);
            clownAnimator.SetBool("Dance", false);
            EnableCameraAndMovement();
        }

        private IEnumerator Talk()
        {
            yield return TextController.ShowCharactersNextBubbleText(Character.Pollin, 2);
            yield return TextController.ShowCharactersNextBubbleText(Character.Muni, 2);
        }
    }
}