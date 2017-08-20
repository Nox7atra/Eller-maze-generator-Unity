using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Nox7atra.Utils;
namespace Nox7atra.Mazes
{
    public class W4Maze : Maze<W4Cell>
    {
        public W4Maze(int width, int height) : base(width, height)
        {
            _Cells = new List<W4Cell>(width * height);
            CreateCells();
        }
        private void CreateCells()
        {
            for(int i = 0; i < _Cells.Capacity; i++)
            {
                _Cells.Add( new W4Cell
                {
                    Set = i
                });
            }
        }
      
        public int MinSetInRow(int rowNum)
        {
            int min = int.MaxValue;
            for(int i = 0; i < ColumnCount; i++)
            {
                var label = GetCell(i, rowNum).Set;
                if(label < min)
                {
                    min = label;
                }
            }
            return min;
        }
        public void ReplaceSetInRow(int oldSet, int newSet, int rowNum)
        {
            for(int i = 0; i < ColumnCount; i++)
            {
                var cell = GetCell(i, rowNum);
                if(cell.Set == oldSet)
                {
                    cell.Set = newSet;
                }
            }
        }
    }
    public class W4Cell
    {
        //Left first 
        public bool LeftWall;
        public bool TopWall;
        public bool RightWall;
        public bool BotWall;
        public int Set;
        public W4Cell()
        {
            LeftWall = true;
            TopWall = true;
            RightWall = true;
            BotWall = true;
            Set = 0;
        }
        public W4Cell(int type)
        {
            var binStr = MathTools.To4Bin(type);
            LeftWall  = binStr[3] == '1';
            TopWall   = binStr[2] == '1';
            RightWall = binStr[1] == '1';
            BotWall   = binStr[0] == '1';
        }
        public int ToInt()
        {
            int sum = 0;
            sum = LeftWall ? 1 : 0;
            sum += TopWall ? 2 : 0;
            sum += RightWall ? 4 : 0;
            sum += BotWall ? 8 : 0;
            return sum;
        }
    }
}