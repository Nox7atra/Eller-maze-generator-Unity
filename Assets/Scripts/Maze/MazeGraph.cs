using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Nox7atra.Mazes
{
    public class MazeGraph
    {

        private List<MazeGraphCell> _Cells;
        private List<Edge> _PathEdges;
        private List<Edge> _WallEdges;
        public List<Edge> PathEdges
        {
            get
            {
                return _PathEdges;
            }
        }
        public List<Edge> WallEdges
        {
            get
            {
                return _WallEdges;
            }
        }
        public MazeGraph(W4Maze maze, bool isSimplify)
        {
            _Cells = new List<MazeGraphCell>(maze.RowCount * maze.ColumnCount);
            _PathEdges = new List<Edge>();
            _WallEdges = new List<Edge>();
            for (int i = 0; i < _Cells.Capacity; i++)
            {
                _Cells.Add(new MazeGraphCell());
            }
            for (int j = 0; j < maze.RowCount; j++)
            {
                for (int i = 0; i < maze.ColumnCount; i++)
                {
                    var cell = maze.GetCell(i, j);
                    int index = i + j * maze.ColumnCount;

                    MazeGraphCellFromW4Cell(
                        _Cells[index],
                        cell,
                        i,
                        j,
                        maze.ColumnCount
                    );

                    _Cells[index].Position = new Vector2(
                        i + 0.5f,
                        j + 0.5f);
                    _Cells[index].CanBeSimplifiied
                        = (!cell.BotWall && !cell.TopWall
                        && cell.RightWall && cell.LeftWall)
                        || (cell.BotWall && cell.TopWall
                        && !cell.RightWall && !cell.LeftWall);

                    if (cell.BotWall)
                    {
                        var edge = new Edge();
                        edge.Begin = new Vector2(
                            _Cells[index].Position.x - 0.5f,
                            _Cells[index].Position.y - 0.5f);
                        edge.End = new Vector2(
                            _Cells[index].Position.x + 0.5f,
                            _Cells[index].Position.y - 0.5f);
                        if (_WallEdges.TrueForAll(x => x != edge))
                            _WallEdges.Add(edge);
                    }
                    if (cell.TopWall)
                    {
                        var edge = new Edge();
                        edge.Begin = new Vector2(
                            _Cells[index].Position.x - 0.5f,
                            _Cells[index].Position.y + 0.5f);
                        edge.End = new Vector2(
                            _Cells[index].Position.x + 0.5f,
                            _Cells[index].Position.y + 0.5f);
                        if (_WallEdges.TrueForAll(x => x != edge))
                            _WallEdges.Add(edge);
                    }
                    if (cell.LeftWall)
                    {
                        var edge = new Edge();
                        edge.Begin = new Vector2(
                            _Cells[index].Position.x - 0.5f,
                            _Cells[index].Position.y - 0.5f);
                        edge.End = new Vector2(
                            _Cells[index].Position.x - 0.5f,
                            _Cells[index].Position.y + 0.5f);
                        if (_WallEdges.TrueForAll(x => x != edge))
                            _WallEdges.Add(edge);
                    }
                    if (cell.RightWall)
                    {
                        var edge = new Edge();
                        edge.Begin = new Vector2(
                            _Cells[index].Position.x + 0.5f,
                            _Cells[index].Position.y + 0.5f);
                        edge.End = new Vector2(
                            _Cells[index].Position.x + 0.5f,
                            _Cells[index].Position.y - 0.5f);
                        if (_WallEdges.TrueForAll(x => x != edge))
                            _WallEdges.Add(edge);
                    }
                }
            }
            if (isSimplify)
            {
                for(int i = 0; i < _Cells.Count; i++)
                {
                    if (_Cells[i].CanBeSimplifiied)
                    {
                        SimplifyCell(_Cells[i]);
                        i--;
                    }
                }
            }
            CalculatePathEdges();
            ProcessWallEdges();
        }
        private void CalculatePathEdges()
        {
            for (int i = 0; i < _Cells.Count; i++)
            {
                var cell = _Cells[i];
                for (int j = 0; j < cell.Neighbours.Count; j++)
                {
                    var edge = new Edge();
                    edge.Begin = cell.Position;
                    edge.End = cell.Neighbours[j].Position;
                    if (_PathEdges.TrueForAll(x => x != edge))
                        _PathEdges.Add(edge);
                }
            }

        }
        private void ProcessWallEdges()
        {
            for(int i = 0; i < _WallEdges.Count; i++)
            {
                for(int j = i + 1; j < _WallEdges.Count; j++)
                {
                    var edge1 = _WallEdges[i];
                    var edge2 = _WallEdges[j];
                    if (Mathf.Abs(edge1.Normal.x)
                        == Mathf.Abs(edge2.Normal.x)
                        && Mathf.Abs(edge1.Normal.y)
                        == Mathf.Abs(edge2.Normal.y))
                    {
                        if (Edge.IsCanMerge(edge1, edge2))
                        {
                            var edge = Edge.Merge(edge1, edge2);
                            _WallEdges.Remove(edge1);
                            _WallEdges.Remove(edge2);
                            _WallEdges.Remove(edge);
                            _WallEdges.Insert(i, edge);
                            j = i;
                        }
                    }
                }
            }
        }
        //Works only on Cells with 2 neighbours
        private void SimplifyCell(
            MazeGraphCell toSimplify)
        {
            var neighbourCell0 = toSimplify.Neighbours[0];
            var neighbourCell1 = toSimplify.Neighbours[1];

            var index = neighbourCell0.Neighbours.IndexOf(toSimplify);
            neighbourCell0.Neighbours[index] = neighbourCell1;

            index = neighbourCell1.Neighbours.IndexOf(toSimplify);
            neighbourCell1.Neighbours[index] = neighbourCell0;
            _Cells.Remove(toSimplify);
        }
        private void MazeGraphCellFromW4Cell(
            MazeGraphCell mgCell,
            W4Cell w4Cell,
            int columNum,
            int rowNum,
            int columnCount
           )
        {
            mgCell.Neighbours = new List<MazeGraphCell>();
            if (!w4Cell.LeftWall)
            {
                mgCell.Neighbours.Add(
                    _Cells[(columNum - 1) + rowNum * columnCount]);
            }
            if (!w4Cell.TopWall)
            {
                mgCell.Neighbours.Add(
                    _Cells[columNum + (rowNum + 1) * columnCount]);
            }
            if (!w4Cell.RightWall)
            {
                mgCell.Neighbours.Add(
                    _Cells[(columNum + 1) + rowNum * columnCount]);
            }
            if (!w4Cell.BotWall)
            {
                mgCell.Neighbours.Add(
                    _Cells[columNum + (rowNum - 1) * columnCount]);
            }
        }
        public bool IsLeafCoords(Vector2 point)
        {
            int count = 0;
            for(int i = 0; i < _WallEdges.Count; i++)
            {
                var BEdistance = Vector2.Distance(_WallEdges[i].Begin, 
                    _WallEdges[i].End);
                var BPdistance = Vector2.Distance(_WallEdges[i].Begin,
                    point);
                var EPdistance = Vector2.Distance(_WallEdges[i].End,
                    point);
                if (BPdistance + EPdistance <= BEdistance + Edge.EPSILON)
                {
                    count++;
                }
            }
            return count == 1;
        }
    }
    public class MazeGraphCell
    {
        public List<MazeGraphCell> Neighbours;
        public Vector2 Position;
        public bool CanBeSimplifiied;
        public bool isVisited;
    }
    public struct Edge
    {
        public const float EPSILON = 0.005f;
        public Vector2 Begin;
        public Vector2 End;

        public Edge(Vector2 begin, Vector2 end)
        {
            Begin = begin;
            End = end;
        }
        public static bool operator==(Edge left, Edge right)
        {
            return 
                (Vector2.Distance(left.Begin, right.Begin) < EPSILON
                && Vector2.Distance(left.End, right.End) < EPSILON)
                || (Vector2.Distance(left.Begin, right.End) < EPSILON
                && Vector2.Distance(left.End, right.Begin) < EPSILON);
        }
        public static bool operator !=(Edge left, Edge right)
        {
            return !((Vector2.Distance(left.Begin, right.Begin) < EPSILON
                && Vector2.Distance(left.End, right.End) < EPSILON)
                || (Vector2.Distance(left.Begin, right.End) < EPSILON
                && Vector2.Distance(left.End, right.Begin) < EPSILON));
        }
        public Vector2 Normal
        {
            get
            {
                var normal = new Vector2(Begin.y - End.y, End.x - Begin.x);
                normal.Normalize();
                return normal;
            }
        }
        public float Magnitude
        {
            get
            {
                return (End - Begin).magnitude;
            }
        }
        public static bool IsCanMerge(Edge first, Edge second)
        {
            return Vector2.Distance(
                        first.Begin,
                        second.Begin)
                        < Edge.EPSILON
                        || Vector2.Distance(
                        first.Begin,
                        second.End)
                        < Edge.EPSILON
                        || Vector2.Distance(
                        first.End,
                        second.Begin)
                        < Edge.EPSILON
                        || Vector2.Distance(
                        first.End,
                        second.End)
                        < Edge.EPSILON;
        }
        public static Edge Merge(Edge first, Edge second)
        {
            var bb = new Edge(first.Begin, second.Begin);
            var be = new Edge(first.Begin, second.End);
            var eb = new Edge(first.End, second.Begin);
            var ee = new Edge(first.End, second.End);
            Edge[] arr = new Edge[]{ bb, be, eb, ee };
            return arr.First(x => x.Magnitude == arr.Max(y => y.Magnitude));
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "Begin " + Begin.ToString() +
                "End " + End.ToString();
        }
    }
}