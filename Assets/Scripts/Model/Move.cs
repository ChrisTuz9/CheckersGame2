using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class Move
{
    private IMovable piece;
    private Vector2 oldPosition;
    private Vector2 newPosition;

    public Move(IMovable piece, Vector2 oldPosition, Vector2 newPosition)
    {
        this.piece = piece;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
    }
    public void Deconstruct(out IMovable piece, out Vector2 oldPosition, out Vector2 newPosition)
    {
        piece = this.piece;
        oldPosition = this.oldPosition;
        newPosition = this.newPosition;
    }

    public override string ToString()
    {
        return $"{piece.Color}: {ConvertPositionToString(oldPosition)} -> {ConvertPositionToString(newPosition)}";
    }

    private string ConvertPositionToString(Vector2 position)
    {
        return $"{(char)('A' + position.x)}{(int)position.y + 1}";
    }

}
