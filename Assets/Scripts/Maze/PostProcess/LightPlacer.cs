using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nox7atra.Mazes.PostProcess
{
    public class LightPlacer : MonoBehaviour
    {
        [SerializeField]
        private LightLevel _LightLevel = LightLevel.Medium;
        [SerializeField]
        private float _LightRange;
        [SerializeField]
        private Color _LightColor;
        [SerializeField]
        private bool _IsRandomColor;
        public GameObject SetUpLights(
            W4Maze maze,
            float height)
        {
            GameObject lights = new GameObject();
#if UNITY_EDITOR
            lights.name = "Lights";
#endif
            for(int i = 0; i < maze.ColumnCount; i++)
            {
                for(int j = 0; j < maze.RowCount; j++)
                {
                    var cell = maze.GetCell(i, j);
                    if(cell.ToInt() < (int) _LightLevel)
                    {
                        var lightGo = CreatePointLight();
                        lightGo.transform.position = new Vector3(
                            i ,
                            height * 0.9f,
                            j);
#if UNITY_EDITOR
                        lightGo.name = "light";
                        lightGo.transform.SetParent(lights.transform);
#endif
                    }
                }
            }
            return lights;
        }
        private GameObject CreatePointLight()
        {
            GameObject lightGO = new GameObject();
            var lightComp = lightGO.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.range = _LightRange;
            lightComp.color = _IsRandomColor
                ? new Color(
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f))
                : _LightColor;
            return lightGO;
        }
        public enum LightLevel : byte
        {
            Low = 3,
            Medium = 5,
            High = 10,
            VeryHigh = 16
        }
    }
}
