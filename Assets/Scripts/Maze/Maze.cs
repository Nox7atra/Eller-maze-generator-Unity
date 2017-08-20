using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Maze<T>
{
    public readonly int ColumnCount;
    public readonly int RowCount;
    protected List<T> _Cells;
    public Maze(int columnCount, int rowCount)
    {
        ColumnCount = columnCount;
        RowCount = rowCount;
    }
    public virtual T GetCell(int x, int z)
    {

        if (x < ColumnCount && z < RowCount)
        {
            return _Cells[x + z * ColumnCount];
        }
        else
        {
            Debug.Log("Index out of range: "
                + (x < ColumnCount ?
                z.ToString() + ">=" + RowCount.ToString() :
                x.ToString() + ">=" + ColumnCount.ToString()));
            return default(T);
        }
    }
}
