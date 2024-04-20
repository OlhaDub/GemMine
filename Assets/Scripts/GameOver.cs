using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
    {
        public GameObject screenParent;
        public GameObject scoreParent;
        public GameObject menuButton;

         public TMP_Text remainingText;
        public TMP_Text remainingSubText;
        public TMP_Text loseText;
        public TMP_Text scoreText;

        public GameObject user;
        public GameObject submitButton;

        private void Start ()
        {
            menuButton.SetActive(false);
            screenParent.SetActive(false);
            loseText.enabled = false;
            scoreText.enabled = false;

            submitButton.SetActive(false);
        user.SetActive(false);
    }


    public void SetRemaining(int remaining) => remainingText.text = remaining.ToString();

    public void SetRemaining(string remaining) => remainingText.text = remaining;

        public void ShowWin(int score)
        {
            menuButton.SetActive(true);
            screenParent.SetActive(true);
            loseText.enabled = true;

            scoreText.text = score.ToString();
            scoreText.enabled = true;

        submitButton.SetActive(true);
        user.SetActive(true);
    }



        public void OnDoneClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
        }

    }

