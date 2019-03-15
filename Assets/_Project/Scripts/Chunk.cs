using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector3> newUV = new List<Vector3>();

    private Mesh newMesh;
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    private Mesh mesh;
    private MeshCollider col;

    private int faceOffset;
    private int colOffset;
    private int z = 0;

    [HideInInspector] public ChunkRenderer cRender;
    [HideInInspector] public Vector2 position;

    public bool collision = true;
    public Vector2 scale;
    public Vector2 offset;

    [ReadOnly] public bool update = false;
    [ReadOnly] public bool rendered;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
    }

    public void UpdateChunk()
    {
        rendered = true;
        UpdateMesh();
        if (collision)
        {
            UpdateCollision();
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.SetUVs(0, newUV);
        mesh.RecalculateNormals();

        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();
        faceOffset = 0;
        if (collision)
        {
            UpdateCollision();
        }
    }

    void UpdateCollision()
    {
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();

        colOffset = 0;
    }

    Vector2 vv;
    Vector3 vf = new Vector3(0, 0, 0);
    int a;
    public void AddTile(int x, int y, TileBase t)
    {
        a = 0;
        for (int i = 0; i < t.vertices.Length; i++)
        {
            vv = t.vertices[i];
            vf.x = (offset.x + x + vv.x) * scale.x;
            vf.y = (offset.y + y + vv.y) * scale.y;
            newVertices.Add(vf);
            a++;
        }
        for (int j = 0; j < t.triangles.Length; j++)
        {
            newTriangles.Add(faceOffset + t.triangles[j]);
        }
        faceOffset += a;

        for(int w = 0; w < t.uvs.Length; w++)
        {
            newUV.Add(t.uvs[w]);
        }

        //newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y + tUnit));
        //newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y + tUnit));
        //newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y));
        //newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y));
    }

    Vector3 cv;
    Vector3 gf = new Vector3(0, 0, 0);
    void AddCollider(int x, int y, TileBase t)
    {
        a = 0;
        #region Top
        int[] trTop;
        Vector3[] clVertices = t.GetCollisionTop(out trTop);
        for(int i = 0; i < clVertices.Length; i++)
        {
            cv = clVertices[i];
            gf.x = (offset.x + x + cv.x) * scale.x;
            gf.y = (offset.y + y + cv.y) * scale.y;
            gf.z = cv.z;
            colVertices.Add(gf);
            a++;
        }

        for(int j = 0; j < trTop.Length; j++)
        {
            colTriangles.Add(colOffset+trTop[j]);
        }
        colOffset += a;
        #endregion
    }
}
