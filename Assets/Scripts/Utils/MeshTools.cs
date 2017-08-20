using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Nox7atra.Utils
{
    public static class MeshTools
    {
        public static Mesh CreatePlaneMesh(
                    float width,
                    float height,
                    int pointsCountX,
                    int pointsCountY,
                    string objectName,
                    bool isSerialize = false,
                    string path = "",
                    float maxDismension = 10)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = CreateVertices(width, height, pointsCountX, pointsCountY);
            mesh.triangles = CreatePlaneTriangles(pointsCountX, pointsCountY);
            mesh.uv = CreatePlaneUVs(mesh, maxDismension);
            mesh.RecalculateNormals();
            mesh.name = objectName;
#if UNITY_EDITOR
            if (isSerialize)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                UnityEditor.AssetDatabase.CreateAsset(mesh,
                    string.Concat(path,
                        "/",
                        objectName,
                        ".obj"));
            }
#endif
            return mesh;
        }
        public static MeshFilter CreateMeshObj(string name, Material mat)
        {
            GameObject meshObj = new GameObject();
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
            mr.material = mat;
            meshObj.name = name;
            return mf;
        }
        public static Vector2[] CreatePlaneUVs(Mesh mesh, float maxDismension)
        {
            try
            {
                Vector3[] verts = mesh.vertices;
                Vector2[] uvs = new Vector2[verts.Length];
                Vector3 minPoint = GetPointWithMinCords(verts);
                Vector3 maxPoint = GetPointWithMaxCords(verts);
                for (int i = 0; i < uvs.Length; i++)
                {
                    uvs[i] = GetPlaneUVPoint(
                        minPoint, 
                        maxPoint, 
                        verts[i],
                        maxDismension);
                }

                return uvs;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                throw e;
            }
        }
        private static Vector2 GetPlaneUVPoint(
            Vector3 min, 
            Vector3 max, 
            Vector3 vert, 
            float maxDismension)
        {
            Vector2 uv;
            if (min.x == max.x)
            {
                uv = new Vector2(
                    (vert.y - max.y) / (min.y - max.y) * (max.x - min.x) / maxDismension,
                    (vert.z - max.z) / (min.z - max.z) * (max.y - min.y) / maxDismension);
            }
            else if (min.y == max.y)
            {
                uv = new Vector2(
                    (vert.x - max.x) / (min.x - max.x) * (max.x - min.x) / maxDismension,
                    (vert.z - max.z) / (min.z - max.z) * (max.z - min.z) / maxDismension);
            }
            else
            {
                uv = new Vector2(
                    (vert.x - max.x) / (min.x - max.x) * (max.x - min.x) / maxDismension,
                    (vert.y - max.y) / (min.y - max.y) * (max.y - min.y) / maxDismension);
            }
            return uv;
        }
        private static Vector3 GetPointWithMinCords(Vector3[] points)
        {
            Vector3 minPoint = new Vector3(
                float.MaxValue,
                float.MaxValue,
                float.MaxValue);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x < minPoint.x)
                {
                    minPoint.x = points[i].x;
                }
                if (points[i].y < minPoint.y)
                {
                    minPoint.y = points[i].y;
                }
                if (points[i].z < minPoint.z)
                {
                    minPoint.z = points[i].z;
                }
            }
            return minPoint;
        }
        private static Vector3 GetPointWithMaxCords(Vector3[] points)
        {
            Vector3 maxPoint = new Vector3(
                float.MinValue,
                float.MinValue,
                float.MinValue);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x > maxPoint.x)
                {
                    maxPoint.x = points[i].x;
                }
                if (points[i].y > maxPoint.y)
                {
                    maxPoint.y = points[i].y;
                }
                if (points[i].z > maxPoint.z)
                {
                    maxPoint.z = points[i].z;
                }
            }
            return maxPoint;
        }

        private static Vector3[] CreateVertices(
            float width,
            float height,
            int pointsCountX,
            int pointsCountY)
        {
            Vector3[] vertices = new Vector3[pointsCountX * pointsCountY];
            for (int i = 0; i < pointsCountY; i++)
            {
                float yPos = -height / 2 + height * i / (pointsCountY - 1);
                for (int j = 0; j < pointsCountX; j++)
                {
                    float xPos = -width / 2 + width * j / (pointsCountX - 1);
                    int index = j + i * pointsCountX;
                    vertices[index] = new Vector3(xPos, yPos, 0f);

                }
            }
            return vertices;
        }
        private static int[] CreatePlaneTriangles(int pointsCountX, int pointsCountY)
        {
            int numFaces = (pointsCountX - 1) * (pointsCountY - 1);
            int[] triangles = new int[numFaces * 6];
            int t = 0;
            for (int face = 0; face < numFaces; face++)
            {
                int i = face % (pointsCountX - 1) + (face / (pointsCountX - 1) * pointsCountX);

                triangles[t++] = i + pointsCountX;
                triangles[t++] = i + 1;
                triangles[t++] = i;

                triangles[t++] = i + pointsCountX;
                triangles[t++] = i + pointsCountX + 1;
                triangles[t++] = i + 1;
            }
            return triangles;
        }
    }
}
