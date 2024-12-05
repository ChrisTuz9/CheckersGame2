using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesPiece : IMovable
{
    public int X { get; set; }
    public int Y { get; set; }
    public PieceColor Color { get; }
    public GameObject PieceObject { get; set; }

    public EnemiesPiece(int x, int y, PieceColor color, GameObject pieceObject)
    {
        X = x;
        Y = y;
        Color = color;
        PieceObject = pieceObject;
    }

    public void SetPosition(Vector3 position)
    {
        PieceObject.transform.position = position;
        X = (int)position.x;
        Y = (int)position.y;
    }

    public Vector3 GetPosition()
    {
        return PieceObject.transform.position;
    }

    public bool HasValidMoveTo(int vectorX, int vectorY)
    {
        return Math.Abs(vectorX) == 1 && vectorY == -1;
    }

    public bool HasValidCaptureBeforeTile(int vectorX, int vectorY)
    {
        return Math.Abs(vectorX) == 2 && Math.Abs(vectorY) == 2;
    }

    public bool CanMove(Board board)
    {
        int[] vectorX = { -1, 1 };
        int vectorY = -1;

        foreach (int dx in vectorX)
        {
            int checkX = X + dx;
            int checkY = Y + vectorY;

            if (!board.inBoardRange(checkX, checkY))
                continue;
            if (board.IsPositionFree(checkX, checkY))
                return true;
        }

        return false;
    }

    public bool CanCapture(Board board)
    {
        int[] deltaX = { -2, 2, -2, 2 };
        int[] deltaY = { 2, 2, -2, -2 };

        for (int i = 0; i < 4; i++)
        {
            int checkRow = Y + deltaY[i];
            int checkCol = X + deltaX[i];
            if (!board.inBoardRange(checkCol, checkRow))
                continue;
            if (!board.IsPositionFree(checkCol, checkRow))
                continue;

            List<IMovable> piecesBetweenPositions = board.GetPiecesBetweenPositions(new Vector2(X, Y), new Vector2(checkCol, checkRow));
            if (piecesBetweenPositions.Count == 1 && piecesBetweenPositions.FirstOrDefault()?.Color != Color)
            {
                return true;
            }
        }

        return false;
    }
}
