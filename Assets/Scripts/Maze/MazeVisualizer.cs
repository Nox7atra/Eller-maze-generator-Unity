using Nox7atra.Mazes;
using Nox7atra.Mazes.Generators;
using Nox7atra.Mazes.PostProcess;
using Nox7atra.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Nox7atra.Mazes
{
    public class MazeVisualizer : MonoBehaviour
    {
        private const string MAZE_MESHDATA_PATH = "Assets/Resources/Gamedata/MazeTMP";
        [SerializeField]
        private LightPlacer _LightPlacer;
        [SerializeField]
        private int _MazeCellsX = 5;
        [SerializeField]
        private int _MazeCellsY = 5;
        [SerializeField]
        private AlgorithmType _CurrentAlgorithm;
        [SerializeField]
        [Tooltip("In Meters")]
        private float _Height = 1;
        [SerializeField]
        [Tooltip("In percent")]
        private float _WallThikness = 0.1f;
        [SerializeField]
        private Material _Material;
        [SerializeField]
        private bool _IsGenerateRoof;
        [SerializeField]
        private bool _IsSerializeMeshes = false;
        [SerializeField]
        private bool _IsDrawGizmos;
        private W4Maze _Maze;
        private MazeGraph _GraphMaze;
        public void RefreshMaze()
        {
            //Preprocess
            var generator = new EllerGenerator();
            _Maze = generator.Generate(_MazeCellsX, _MazeCellsY);
            _GraphMaze = new MazeGraph(_Maze, true);
            //Process
            var mazeGO = GenerateW4MazeMesh(
                _Maze,
                _Height, 
                _WallThikness,
                _Material);
            //PostProcess
            var lightsGO = _LightPlacer.SetUpLights(_Maze, _Height);
            lightsGO.transform.SetParent(mazeGO.transform);
        }
        public GameObject GenerateW4MazeMesh(
            W4Maze maze,
            float mazeHeight,
            float wallThikness,
            Material material)
        {
            var mazeGO = new GameObject();
            var mazeWidth = maze.ColumnCount;
            var mazeDepth = maze.RowCount;
            var floor = CreateFloor(
                mazeWidth, 
                mazeDepth,
                material);
            var wallsGO = CreateWalls(
                new MazeGraph(maze, true), 
                mazeHeight, 
                wallThikness,
                Mathf.Max(mazeDepth, mazeWidth),
                material);
            floor.transform.position += new Vector3(
                mazeWidth / 2f,
                0,
                mazeDepth / 2f);
            if (_IsGenerateRoof)
            {
                var roof = Instantiate(floor);
                roof.transform.position += Vector3.up * mazeHeight;
                roof.transform.up = -Vector3.up;
#if UNITY_EDITOR
                roof.transform.SetParent(mazeGO.transform);
                roof.name = "roof" + mazeWidth.ToString() + "x"
                    + mazeDepth.ToString();
#endif
            }
#if UNITY_EDITOR
            floor.transform.SetParent(mazeGO.transform);
            wallsGO.transform.SetParent(mazeGO.transform);
            mazeGO.name = "Maze";
            wallsGO.name = "Walls";
#endif
            return mazeGO;
        }
        private GameObject CreateFloor(
            float width, 
            float depth, 
            Material mat)
        {
            var name = "floor" + width.ToString() + "x" + depth.ToString();
            var floor = MeshTools.CreateMeshObj(name, mat);

            floor.sharedMesh = MeshTools.CreatePlaneMesh(
                width,
                depth,
                2,
                2,
                name,
                _IsSerializeMeshes,
                MAZE_MESHDATA_PATH,
                Mathf.Max(width, depth)
            );
            floor.transform.rotation = Quaternion.Euler(90, 0, 0);
            return floor.gameObject;
        }
        private MeshFilter CreateWall(
            float length,
            float height,
            float maxDismension,
            Material mat)
        {
            var name = "wall" + length.ToString() + "x" + height.ToString();
            var mf = MeshTools.CreateMeshObj(name, mat);

            mf.mesh = MeshTools.CreatePlaneMesh(
                length,
                height,
                2,
                2,
                name,
                _IsSerializeMeshes,
                MAZE_MESHDATA_PATH,
                maxDismension);
            return mf;
        }
        private GameObject CreateWalls(
            MazeGraph maze, 
            float height,
            float wallThikness,
            float maxDismension,
            Material mat)
        {
            var wallsGO = new GameObject();
            var edges = _GraphMaze.WallEdges;
            for(int i = 0; i < edges.Count; i++)
            {
                var begin = edges[i].Begin;
                var end = edges[i].End;
                var wall1MF = CreateWall(
                        (Vector2.Distance(begin, end) + wallThikness),
                        height,
                        maxDismension,
                        mat);
                var normal = new Vector3(edges[i].Normal.x, 0, edges[i].Normal.y);
                //first wall
                wall1MF.transform.forward = normal;
                wall1MF.transform.position = new Vector3(
                            (begin.x + end.x) / 2,
                            height / 2,
                            (begin.y + end.y) / 2)
                            - normal * wallThikness
                            - (new Vector3(1, 0, 1) - normal) 
                            * wallThikness / 2;

                //second wall
                var wall2MF = Instantiate(wall1MF);
                wall2MF.transform.forward = -normal;
                wall2MF.transform.position += normal * wallThikness;
                if (wallThikness > 0)
                {
                    if (maze.IsLeafCoords(begin))
                    {
                        var edgeWall = CreateWall(
                            wallThikness, 
                            height, 
                            maxDismension, 
                            mat);
                        var direction = (end - begin).normalized.Vec2XYToVec3XZ();
                        edgeWall.transform.position = (begin.Vec2XYToVec3XZ() 
                            - wallThikness * direction
                            - (new Vector3(1, 0, 1) - direction)
                            * wallThikness / 2) 
                            + Vector3.up * height / 2;
                        edgeWall.transform.forward = direction;
                        edgeWall.transform.Rotate(new Vector3(0, 0, 180));
                        edgeWall.transform.SetParent(wallsGO.transform);
                    }
                    if (maze.IsLeafCoords(end))
                    {
                        var edgeWall = CreateWall(
                            wallThikness,
                            height,
                            maxDismension,
                            mat);
                        var direction = (begin - end).normalized.Vec2XYToVec3XZ();
                        edgeWall.transform.position = (end.Vec2XYToVec3XZ() 
                            + Vector3.up * height /2 
                            - wallThikness * direction
                            - (new Vector3(1, 0, 1) - direction)
                            * wallThikness / 2);
                        edgeWall.transform.forward = direction;
                        edgeWall.transform.Rotate(new Vector3(0, 0, 180));
                        edgeWall.transform.SetParent(wallsGO.transform);
                    }
                }
                wall1MF.transform.Rotate(new Vector3(0, 0, 180));
                wall2MF.transform.Rotate(new Vector3(0, 0, 180));
                wall1MF.transform.SetParent(wallsGO.transform);
                wall2MF.transform.SetParent(wallsGO.transform);
            }
            return wallsGO;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_IsDrawGizmos)
            {
                if (_Maze != null)
                {
                    Gizmos.color = Color.green;
                    UnityEditor.Handles.color = Color.white;
                    var edges = _GraphMaze.PathEdges;
                    for (int i = 0; i < edges.Count; i++)
                    {
                        Gizmos.DrawLine(edges[i].Begin, edges[i].End);
                        UnityEditor.Handles.DrawSolidDisc(edges[i].Begin, -Vector3.forward, 0.05f);
                        UnityEditor.Handles.DrawSolidDisc(edges[i].End, -Vector3.forward, 0.05f);
                    }
                    Gizmos.color = Color.yellow;
                    Handles.color = Color.white;
                    edges = _GraphMaze.WallEdges;
                    for (int i = 0; i < edges.Count; i++)
                    {
                        Gizmos.DrawLine(edges[i].Begin, edges[i].End);
                        UnityEditor.Handles.DrawSolidDisc(edges[i].Begin, -Vector3.forward, 0.05f);
                        UnityEditor.Handles.DrawSolidDisc(edges[i].End, -Vector3.forward, 0.05f);
                    }
                }
            }
        }
#endif
        public enum AlgorithmType
        {
            Eller
        }
    }
}