using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController
{
    private Board board;
    private GameLogic gameLogic;
    private MoveLogger moveLogger;
    private List<Move> moves;

    private IMovable selectedPiece = null;
    private bool isWhiteTurn;
    private bool captureChain;
    string gameOverText = null;

    public event Action<string> OnGameOver;

    public GameController(Board board, MoveLogger moveLogger, List<Move> moves)
    {
        this.board = board;
        this.moveLogger = moveLogger;
        this.moves = moves;
        gameLogic = new GameLogic(board);
        selectedPiece = null;
        isWhiteTurn = true;
        captureChain = false;
    }

    public void MakeAction(GameObject clickedObject)
    {
        if (clickedObject.tag == "Piece")
        {
            IMovable piece = board.GetPieceByGameObject(clickedObject);
            if (piece != null && IsCorrectTurn(piece))
            {
                if (!captureChain)
                {
                    selectedPiece = piece;
                }
            }
        }
        else if (clickedObject.tag == "Tile" && selectedPiece != null)
        {
            MovePiece(clickedObject);
        }
    }

    private bool IsCorrectTurn(IMovable piece)
    {
        return (isWhiteTurn && piece.Color == PieceColor.WHITE) || (!isWhiteTurn && piece.Color == PieceColor.BLACK);
    }

    private void MovePiece(GameObject tile)
    {
        Vector3 oldPosition = selectedPiece.GetPosition();
        Vector3 newPosition = new Vector3(
            Mathf.RoundToInt(tile.transform.position.x),
            Mathf.RoundToInt(tile.transform.position.y),
            -1
        );

        if (!gameLogic.isActionValid(oldPosition, newPosition))
            return;

        List<IMovable> piecesBetweenPositions = board.GetPiecesBetweenPositions(oldPosition, newPosition);
        PieceColor currentColor = isWhiteTurn ? PieceColor.WHITE : PieceColor.BLACK;

        if (piecesBetweenPositions.Count == 1 && piecesBetweenPositions.FirstOrDefault()?.Color != currentColor && gameLogic.IsValidCapture(selectedPiece, newPosition))
        {
            board.RemovePiece(piecesBetweenPositions.First());
            selectedPiece.SetPosition(newPosition);
            if(selectedPiece.CanCapture(board))
            {
                captureChain = true;
                SaveMoveToHistory(selectedPiece, oldPosition, newPosition);
            }
            else
            {
                EndTurn(selectedPiece, oldPosition, newPosition);
            }
        }
        else if(piecesBetweenPositions.Count == 0 && gameLogic.IsValidMove(selectedPiece, newPosition) && !gameLogic.IsCapturePossibleForColor(currentColor))
        {
            selectedPiece.SetPosition(newPosition);
            EndTurn(selectedPiece, oldPosition, newPosition);
        }

    }

    private void SaveMoveToHistory(IMovable piece, Vector2 oldPosition, Vector2 newPosition)
    {
        moves.Add(new Move(piece, oldPosition, newPosition));
        moveLogger.AddMoveToLog( $"{moves.Count}. {moves[^1].ToString()}\n");
    }

    private void EndTurn(IMovable piece, Vector2 oldPosition, Vector2 newPosition)
    {
        captureChain = false;
        selectedPiece = null;
        isWhiteTurn = !isWhiteTurn;

        SaveMoveToHistory(piece, oldPosition, newPosition);
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (isWhiteTurn)
        {
            if (!gameLogic.HasAnyValidMove(PieceColor.WHITE))
            {
                gameOverText = "Black win!";
                OnGameOver?.Invoke(gameOverText);
            }
        }
        else
        {
            if (!gameLogic.HasAnyValidMove(PieceColor.BLACK))
            {
                gameOverText = "White win!";
                OnGameOver?.Invoke(gameOverText);
            }
        }
    }
    
}
