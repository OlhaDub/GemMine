using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBombPiece : ClearablePiece
{
    public override void Clear()
    {
        base.Clear();

        // Отримуємо позицію цього елементу на ігровому полі
        int x = piece.X;
        int y = piece.Y;

        // Викликаємо метод 
        piece.GameGridRef.ClearBomb(x, y);
    }
}
