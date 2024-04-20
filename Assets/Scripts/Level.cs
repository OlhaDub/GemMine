using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
    {
        public GameGrid gameGrid;
        public Hud hud;

        protected LevelType type;

        protected int currentScore;

        

        private void Start()
        {
            hud.SetScore(currentScore);
        }

        public LevelType Type => type;

        protected virtual void GameWin()
        {
            gameGrid.GameOver();
            StartCoroutine(WaitForGridFill());
        }

    
        public virtual void OnMove()
        {

        }

        public void OnPieceCleared(GamePiece piece)
        {
            
            currentScore += piece.score;
            hud.SetScore(currentScore);
        }

        protected virtual IEnumerator WaitForGridFill()
        {
            while (gameGrid.IsFilling)
            {
                yield return null;
            }

        hud.OnGameWin(currentScore);
        }
    }

