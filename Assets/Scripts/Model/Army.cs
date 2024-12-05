using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Army<T> : IEnumerable<T> where T : IMovable
{
    private List<T> pieces;

    public Army()
    {
        pieces = new List<T>();
    }
    public void AddPiece(T piece)
    {
        pieces.Add(piece);
    }
    public void RemovePiece(T piece)
    {
        pieces.Remove(piece);
    }

    public T Find(Func<T, bool> predicate)
    {
        return pieces.FirstOrDefault(predicate);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return pieces.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}