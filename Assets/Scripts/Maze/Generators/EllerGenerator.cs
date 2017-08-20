using UnityEngine;

namespace Nox7atra.Mazes.Generators
{
    public class EllerGenerator : MazeGenerator
    {
        public override W4Maze Generate(int width, int height)
        {
            var eulerMaze = new W4Maze(width, height);
            for (int i = 0; i < eulerMaze.RowCount - 1; i++)
            {
                CreateRow(eulerMaze, i);
                CreateVerticalConnections(eulerMaze, i);
            }
            CreateLastRow(eulerMaze);
            return eulerMaze;
        }
        private void CreateRow(W4Maze maze, int rowNum)
        {
            for(int i = 0; i < maze.ColumnCount - 1; i++)
            {
                var cell = maze.GetCell(i, rowNum);
                var nextCell = maze.GetCell(i + 1, rowNum);
                if (cell.Set != nextCell.Set)
                {
                    if (UnityEngine.Random.Range(0, 2) > 0)
                    {
                        RemoveHorizonWallBetweenCells(
                            maze,
                            cell,
                            nextCell,
                            rowNum);
                    }
                }
            }
        }
        private void CreateVerticalConnections(
            W4Maze maze,
            int rowNum)
        {
            bool removeVertical = false;
            bool isAddedVertical = false;
            for (int i = 0; i < maze.ColumnCount - 1; i++)
            {
                W4Cell cell = maze.GetCell(i, rowNum);
                W4Cell nextCell = maze.GetCell(i + 1, rowNum);
                W4Cell topCell = maze.GetCell(i, rowNum + 1);
                if (cell.Set != nextCell.Set)
                {
                    if (!isAddedVertical)
                    {
                        RemoveVerticalWall(cell, topCell);
                    }
                    isAddedVertical = false;
                }
                else
                {
                    removeVertical = Random.Range(0, 2) > 0;
                    if (removeVertical)
                    {
                        RemoveVerticalWall(cell, topCell);
                        isAddedVertical = true;
                    }
                }
            }
            CheckLastVertical(maze, rowNum, isAddedVertical);
        }
        private void CheckLastVertical(W4Maze maze, int rowNum, bool isAddedVertical)
        {
            W4Cell lastCell = maze.GetCell(maze.ColumnCount - 1, rowNum);
            W4Cell preLastCell = maze.GetCell(maze.ColumnCount - 2, rowNum);
            W4Cell topCell = maze.GetCell(maze.ColumnCount - 1, rowNum + 1);
            if (lastCell.Set != preLastCell.Set)
            {
                RemoveVerticalWall(lastCell, topCell);
            }
            else
            {
                if (isAddedVertical ? Random.Range(0, 2) > 0 : true)
                {
                    RemoveVerticalWall(lastCell, topCell);
                }
            }
        }
        private void RemoveVerticalWall(W4Cell bot, W4Cell top)
        {
            bot.TopWall = false;
            top.BotWall = false;
            top.Set = bot.Set;
        }
        private void CreateLastRow(W4Maze maze)
        { 
            int y = maze.RowCount - 1;
            for(int i = 0; i < maze.ColumnCount - 1; i++)
            {
                var cell = maze.GetCell(i, y);
                var nextCell = maze.GetCell(i + 1, y);
                if(cell.Set != nextCell.Set)
                {
                    RemoveHorizonWallBetweenCells(
                        maze,
                        cell,
                        nextCell,
                        y);
                }
            }
        }
        private void RemoveHorizonWallBetweenCells(
            W4Maze maze,
            W4Cell leftCell, 
            W4Cell rightCell,
            int rowNum)
        {
            leftCell.RightWall = false;
            rightCell.LeftWall = false;
            if (leftCell.Set < rightCell.Set)
            {
                maze.ReplaceSetInRow(rightCell.Set, leftCell.Set, rowNum);
            }
            else
            {
                maze.ReplaceSetInRow(leftCell.Set, rightCell.Set, rowNum);
            }
        }
    }
}