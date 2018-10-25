﻿using Code.Classes;
using Code.Scripts.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Code.Scripts.SceneController
{
    public class BaseSceneController : MonoBehaviour
    {
        public GameObject GameElements;
        public GameObject MainCamera;
        public List<Player> PlayersGoList;
        public GameObject RespawnPointParent;
        public GameObject SpeechBubble;
        public TMP_Text Text;
        public TextAsset TextAsset;
        public GameObject UiCanvas;
        protected int CutsceneStringCounter;
        protected List<string> CutsceneStrings;
        protected Dictionary<Character, Player> Players;
        private List<Transform> respawnPoints;

        public delegate void RespawnEventhandler();

        public static event RespawnEventhandler OnRespawn;
        public static void InvokeRespawnBoth()
        {
            OnRespawn?.Invoke();
        }

        protected void DisableFollowingCamera()
        {
            MainCamera.GetComponent<FollowingCamera>().Following = false;
        }

        protected void DisablePlayerMovement()
        {
            foreach (Player player in Players.Values)
                player.DisableMovement();
        }

        protected IEnumerator Fade(SpriteRenderer sprite, int from, int to, float duration = 5)
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

        protected void FadeSceneOut()
        {
            GameElements.GetComponentsInChildren<SpriteRenderer>().ToList()
                .ForEach(s => { StartCoroutine(Fade(s, 1, 0)); });
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

        protected void SetNextCutSceneString()
        {
            Text.text = CutsceneStrings[CutsceneStringCounter++];
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

        protected IEnumerator ShowNextTextSection(int time, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                SetNextCutSceneString();
                yield return new WaitForSeconds(time);
            }
        }

        /// <summary>
        /// Initializes the cutscene strings from the provided textfile
        /// </summary>
        protected virtual void Start()
        {
            respawnPoints = InitializeRespawnPoints();
            OnRespawn += RespawnBoth;
            InitializeCutsceneStrings();
            InitializePlayerDictionary();
        }

        private Vector3 FindClosestSpawnPoint()
        {
            Vector3 middlePoint =
                (Players[Character.Pollin].transform.position + Players[Character.Muni].transform.position) / 2;
            float minDistance = respawnPoints.Min(rp => Vector3.Distance(middlePoint, rp.position));
            Vector3 closest = respawnPoints.First(rp => Vector3.Distance(middlePoint, rp.position) == minDistance)
                .position;
            return closest;
        }

        private void InitializeCutsceneStrings()
        {
            string completeString = TextAsset.text;
            CutsceneStrings = completeString.Split('\n').ToList();
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

        private List<Transform> InitializeRespawnPoints()
        {
            return RespawnPointParent.GetComponentsInChildren<Transform>().ToList();
        }
        private void RespawnBoth()
        {
            Vector3 closest = FindClosestSpawnPoint();
            foreach (Player player in Players.Values)
            {
                player.GetComponent<Transform>().position = closest;
            }
        }
    }
}