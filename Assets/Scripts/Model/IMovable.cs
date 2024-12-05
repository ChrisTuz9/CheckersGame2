using System;
using UnityEngine;

public interface IMovable
{
    public int X { get; set; }
    public int Y { get; set; }
    public PieceColor Color { get; }
    public GameObject PieceObject { get; set; }

    public void SetPosition(Vector3 position);
    public Vector3 GetPosition();
    public bool HasValidMoveTo(int vectorX, int vectorY);
    public bool HasValidCaptureBeforeTile(int vectorX, int vectorY);
    public bool CanMove(Board board);
    public bool CanCapture(Board board);
}