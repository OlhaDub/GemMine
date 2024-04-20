using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hud : MonoBehaviour
    {
        public Level level;
        public GameOver gameOver;

        public TMP_Text remainingText;
        public TMP_Text scoreText;


        private void Start ()
        {
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }


        public void SetRemaining(int remaining) => remainingText.text = remaining.ToString();



        public void OnGameWin(int score)
        {
            gameOver.ShowWin(score);
        }

    }

