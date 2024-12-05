using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour, IEnumerable<IMovable>
{
    private const short boardSize = 8;
    private GameObject blackTilePreFabs;
    private GameObject whiteTilePreFabs;
    private GameObject blackPiecePreFabs;
    private GameObject whitePiecePreFabs;
    private GameObject blackKingPreFabs;
    private GameObject whiteKingPreFabs;
    private GameObject[,] tiles = new GameObject[boardSize, boardSize];
    private Army<IMovable> pieces = new Army<IMovable>();
    public PieceColor playerColor { get; set; }
    public PieceColor enemyColor { get; set; }

    public void SetTilesPreFabs(GameObject whiteTilePreFabs, GameObject blackTilePreFabs)
    {
        this.whiteTilePreFabs = whiteTilePreFabs;
        this.blackTilePreFabs = blackTilePreFabs;
    }

    public void SetPiecesPreFabs(GameObject whitePiecePreFabs, GameObject blackPiecePreFabs, GameObject whiteKingPreFabs, GameObject blackKingPreFabs)
    {
        this.whitePiecePreFabs = whitePiecePreFabs;
        this.blackPiecePreFabs = blackPiecePreFabs;
        this.whiteKingPreFabs = whiteKingPreFabs;
        this.blackKingPreFabs = blackKingPreFabs;
    }

    public void CreateBoard(PieceColor playerColor)
    {
        this.playerColor = playerColor;
        enemyColor = playerColor == PieceColor.WHITE ? PieceColor.BLACK : PieceColor.WHITE;

        bool isTileBlack = true;
        foreach (int row in Enumerable.Range(0, boardSize))
        {
            foreach (int col in Enumerable.Range(0, boardSize))
            {
                GameObject tile = Instantiate(isTileBlack ? blackTilePreFabs : whiteTilePreFabs, new Vector2(col, row), Quaternion.identity);
                tiles[row, col] = tile;

                isTileBlack = !isTileBlack;
            }
            isTileBlack = !isTileBlack;
        }

        PlacePieces<PlayersPiece>(0, 3, playerColor);
        PlacePieces<EnemiesPiece>(boardSize - 3, boardSize, enemyColor);
    }

    private void PlacePieces<T>(int rowFirst, int rowLast, PieceColor pieceColor) where T : IMovable
    {
        GameObject piecePrefab = pieceColor == PieceColor.WHITE ? whitePiecePreFabs : blackPiecePreFabs;

        for (int row = rowFirst; row < rowLast; row++)
        {
            int colFirst = row % 2;
            int colLast = colFirst + boardSize - 1;
            for (int col = colFirst; col < colLast; col += 2)
            {
                Vector3 position = new Vector3(col, row, -1);
                GameObject pieceObject = Instantiate(piecePrefab, position, Quaternion.identity);
                T piece = (T)Activator.CreateInstance(typeof(T), col, row, pieceColor, pieceObject);
                pieces.AddPiece(piece);
            }
        }
    }

    public bool inBoardRange(int x, int y)
    {
        return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
    }

    public IMovable GetPieceAt(int x, int y)
    {
        return pieces.Find(p => p.X == x && p.Y == y);
    }

    public IMovable GetPieceByGameObject(GameObject pieceObject)
    {
        return pieces.Find(p => p.PieceObject == pieceObject);
    }

    public List<IMovable> GetPiecesBetweenPositions(Vector2 oldPosition, Vector2 newPosition)
    {
        int fromCol = (int)oldPosition.x;
        int fromRow = (int)oldPosition.y;
        int toCol = (int)newPosition.x;
        int toRow = (int)newPosition.y;

        int directionX = Math.Sign(toCol - fromCol);
        int directionY = Math.Sign(toRow - fromRow);
        int distance = Math.Abs(toRow - fromRow);

        if (distance <= 1)
            return new List<IMovable>();

        return Enumerable.Range(1, distance - 1)
                     .Select(i => GetPieceAt(fromCol + directionX * i, fromRow + directionY * i))
                     .Where(piece => piece != null)
                     .ToList();
    }

    public bool IsPositionFree(int x, int y)
    {
        return GetPieceAt(x, y) == null;
    }

    public void RemovePiece(IMovable piece)
    {
        pieces.RemovePiece(piece);
        Destroy(piece.PieceObject);
    }

    public void DestroyBoard()
    {
        foreach (IMovable piece in pieces.ToList())
        {
            if (piece != null)
            {
                RemovePiece(piece);
            }
        }

        foreach (GameObject tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
    }

    public IEnumerator<IMovable> GetEnumerator()
    {
        return pieces.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
