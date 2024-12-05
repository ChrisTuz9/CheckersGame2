using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    private Board board;

    public GameLogic(Board board)
    {
        this.board = board;
    }

    public bool isActionValid(Vector2 oldPosition, Vector2 newPosition)
    {
        int toCol = (int)newPosition.x;
        int toRow = (int)newPosition.y;
        if (!board.IsPositionFree(toCol, toRow)) { return false; }

        int fromCol = (int)oldPosition.x;
        int fromRow = (int)oldPosition.y;
        int vectorX = toCol - fromCol;
        int vectorY = toRow - fromRow;
        if(Mathf.Abs(vectorY) != Mathf.Abs(vectorX)) { return false; }

        return true;
    }

    public bool IsValidMove(IMovable selectedPiece, Vector2 newPosition)
    {
        int fromCol = selectedPiece.X;
        int fromRow = selectedPiece.Y;
        int toCol = (int)newPosition.x;
        int toRow = (int)newPosition.y;
        int vectorX = toCol - fromCol;
        int vectorY = toRow - fromRow;

        return selectedPiece.HasValidMoveTo(vectorX, vectorY);
    }

    public bool IsValidCapture(IMovable selectedPiece, Vector2 newPosition)
    {
        int fromCol = selectedPiece.X;
        int fromRow = selectedPiece.Y;
        int toCol = (int)newPosition.x;
        int toRow = (int)newPosition.y;
        int vectorX = toCol - fromCol;
        int vectorY = toRow - fromRow;
        Debug.Log($"{vectorX} {vectorY}");
        return selectedPiece.HasValidCaptureBeforeTile(vectorX, vectorY);
    }

    public bool CheckAllTiles(PieceColor color, Func<IMovable, bool> function)
    {
        foreach (var piece in board)
        {
            if (piece.Color == color && function(piece))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCapturePossibleForColor(PieceColor color)
    {
        return CheckAllTiles(color, piece => piece.CanCapture(board));
    }

    public bool IsMovePossibleForColor(PieceColor color)
    {
        return CheckAllTiles(color, piece => piece.CanMove(board));
    }

    public bool HasAnyValidMove(PieceColor color)
    {
        return IsMovePossibleForColor(color) || IsCapturePossibleForColor(color);
    }
}