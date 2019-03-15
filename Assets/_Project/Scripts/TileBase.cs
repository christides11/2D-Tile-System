using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Base Tile", menuName ="Tiles/Base Tile")]
public class TileBase : ScriptableObject
{
    public Matrix4x4 transform;
    public string id;
    public string tileName;
    public Sprite sprite;
    [SerializeField] public bool[] coll = new bool[0];
    [SerializeField] public Vector2Int collPivot;
    [HideInInspector] public int collX;
    [HideInInspector] public int collY;
    public Vector2[] vertices;
    public int[] triangles;
    public Vector3[] uvs;

    public Vector3[] colVerticesTop;
    public int[] colTrianglesTop;
    public Vector3[] colVerticesBtm;
    public int[] colTrianglesBtm;
    public Vector3[] colVerticesLeft;
    public int[] colTrianglesLeft;
    public Vector3[] colVerticesRight;
    public int[] colTrianglesRight;

    public virtual Vector3[] GetCollisionTop(out int[] trianglesTop)
    {
        trianglesTop = colTrianglesTop;
        return colVerticesTop;
    }

    public virtual void OnAddedToMap(int x, int y)
    {

    }
}
