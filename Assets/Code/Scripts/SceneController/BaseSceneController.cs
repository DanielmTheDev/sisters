﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Classes;
using Code.Scripts.Entity;
using TMPro;
using UnityEngine;

namespace Code.Scripts.SceneController
{
    public class BaseSceneController : MonoBehaviour
    {
        public TextAsset TextAsset;
        public TMP_Text Text;
        public GameObject UiCanvas;
        public GameObject GameElements;
        public GameObject SpeechBubble;
        public GameObject MainCamera;
        public List<Player> PlayersGoList;

        protected List<string> CutsceneStrings;
        protected int CutsceneStringCounter;
        protected Dictionary<Character, Player> Players;

        /// <summary>
        /// Initializes the cutscene strings from the provided textfile
        /// </summary>
        protected virtual void Start()
        {
            InitializeCutsceneStrings();
            InitializePlayerDictionary();
        }

        protected void DisablePlayerMovement()
        {
            foreach (Player player in Players.Values)
                player.DisableMovement();
        }

        protected void DisableFollowingCamera()
        {
            MainCamera.GetComponent<FollowingCamera>().Following = false;
        }

        private void InitializePlayerDictionary()
        {
            Players = new Dictionary<Character, Player>();
            if (PlayersGoList == null || PlayersGoList.Count == 0)
                return;
            Player muni = PlayersGoList.First(p => p.gameObject.name.Contains("Muni"));
            Players.Add(Character.Muni, muni);
            Player pollin = PlayersGoList.First(p => p.gameObject.name.Contains("Pollin"));
            Players.Add(Character.Pollin, pollin);
        }

        protected void SetNextCutSceneString()
        {
            Text.text = CutsceneStrings[CutsceneStringCounter++];
        }

        private void InitializeCutsceneStrings()
        {
            string completeString = TextAsset.text;
            CutsceneStrings = completeString.Split('\n').ToList();
        }

        protected IEnumerator ShowNextTextSection(int time, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                SetNextCutSceneString();
                yield return new WaitForSeconds(time);
            }
        }

        protected IEnumerator MoveCamera(Vector3 targetPosition)
        {
            Transform cameraTransform = MainCamera.transform;
            const float smoothTime = 4;
            Vector3 velocity = Vector3.zero;
            while (cameraTransform.position != targetPosition)
            {
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                    ref velocity, smoothTime);
                yield return null;
            }
        }

        protected IEnumerator FillSpeechbubble()
        {
            Text.text = string.Empty;
            string bubbleText = CutsceneStrings[CutsceneStringCounter++];
            char[] charArray = bubbleText.ToCharArray();
            foreach (char c in charArray)
            {
                Text.text += c;
                yield return new WaitForSeconds(0.08f);
            }
        }

        protected void SetUpSpeechBubble()
        {
            if (SpeechBubble == null)
                return;
            SpeechBubble.GetComponent<SpriteRenderer>().enabled = true;
            Text = SpeechBubble.GetComponentInChildren<TMP_Text>();
        }

        protected IEnumerator ShowNextBubbleText(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                yield return FillSpeechbubble();
                yield return new WaitForSeconds(2);
            }
        }

        protected void FadeSceneOut()
        {
            GameElements.GetComponentsInChildren<SpriteRenderer>().ToList()
                .ForEach(s => { StartCoroutine(Fade(s, 1, 0)); });
        }

        protected IEnumerator Fade(SpriteRenderer sprite,int from, int to, float duration = 5)
        {
            float t = 0;
            float startTime = Time.time;
            while (t < 1)
            {
                t = (Time.time - startTime) / duration;
                sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(from, to, t));
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(3);
        }
    }
}