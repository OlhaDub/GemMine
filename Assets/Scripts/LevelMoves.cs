public class LevelMoves : Level
    {

        public int numMoves;

        private int _movesUsed = 0;

        private void Start()
        {
            type = LevelType.Moves;

            hud.SetScore(currentScore);
            hud.SetRemaining(numMoves);
        }

        public override void OnMove()
        {
            _movesUsed++;

            hud.SetRemaining(numMoves - _movesUsed);

            if (numMoves - _movesUsed != 0) return; 
            else { GameWin(); }
        }
    }

