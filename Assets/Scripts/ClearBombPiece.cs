using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBombPiece : ClearablePiece
{
    public override void Clear()
    {
        base.Clear();

        // �������� ������� ����� �������� �� �������� ���
        int x = piece.X;
        int y = piece.Y;

        // ��������� ����� 
        piece.GameGridRef.ClearBomb(x, y);
    }
}
