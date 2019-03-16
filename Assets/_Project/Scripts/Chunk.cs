using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private CollisionData collisionData =  new CollisionData();
    private Mesh colMesh;

    private MeshData meshData = new MeshData();
    private Mesh mesh;
    private MeshCollider col;
    private MeshRenderer mr;

    private int faceOffset;
    private int colOffset;
    private int z = 0;

    [HideInInspector] public ChunkRenderer cRender;
    [HideInInspector] public Vector2Int position;

    public bool collision = true;
    public Vector2 scale;
    public Vector2 offset;

    [ReadOnly] public bool update = true;
    [ReadOnly] public bool rendered;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        mr = GetComponent<MeshRenderer>();
        meshData.vertices = new List<Vector3>();
        meshData.triangles = new List<int>();
        meshData.uv = new List<Vector3>();
        collisionData.vertices = new List<Vector3>();
        collisionData.triangles = new List<int>();
    }

    public void Update()
    {
        if (update)
        {
            update = false;
            UpdateChunk();
        }
    }

    void UpdateChunk()
    {
        rendered = true;
        BuildChunk();
        RenderChunk();
        if (collision)
        {
            UpdateCollision();
        }
    }

    public void DestroyChunk()
    {

    }

    ChunkDefinition cd;
    string tl;
    void BuildChunk()
    {
        cd = cRender.mm.map.chunks[position.x, position.y];
        for(int i = 0; i < cd.fgTiles.GetLength(0); i++)
        {
            for(int j = 0; j < cd.fgTiles.GetLength(0); j++)
            {
                tl = cd.GetTile(i, j, true);
                if (!ReferenceEquals(tl, null))
                {
                    AddTile(i, j, TileCollection.GetTile(tl));
                }
            }
        }
    }

    void RenderChunk()
    {
        //Render chunk
        mesh.Clear();
        mesh.vertices = meshData.vertices.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.SetUVs(0, meshData.uv);
        mesh.RecalculateNormals();
        //Cleanup
        meshData.vertices.Clear();
        meshData.triangles.Clear();
        meshData.uv.Clear();
        faceOffset = 0;
    }

    void UpdateCollision()
    {
        //Add collision
        Destroy(col.sharedMesh);
        colMesh.Clear();
        colMesh.vertices = collisionData.vertices.ToArray();
        colMesh.triangles = collisionData.triangles.ToArray();
        colMesh.RecalculateNormals();
        col.sharedMesh = colMesh;
        //Cleanup
        collisionData.vertices.Clear();
        collisionData.triangles.Clear();

        colOffset = 0;
    }

    Vector2 vv;
    Vector3 vf = new Vector3(0, 0, 0);
    int a;
    void AddTile(int x, int y, TileBase t)
    {
        if(ReferenceEquals(t, null))
        {
            return;
        }
        a = 0;
        for (int i = 0; i < t.vertices.Length; i++)
        {
            vv = t.vertices[i];
            vf.x = (offset.x + x + vv.x) * scale.x;
            vf.y = (offset.y + y + vv.y) * scale.y;
            meshData.vertices.Add(vf);
            a++;
        }
        for (int j = 0; j < t.triangles.Length; j++)
        {
            meshData.triangles.Add(faceOffset + t.triangles[j]);
        }
        faceOffset += a;

        for(int w = 0; w < t.uvs.Length; w++)
        {
            meshData.uv.Add(t.uvs[w]);
        }
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
            collisionData.vertices.Add(gf);
            a++;
        }

        for(int j = 0; j < trTop.Length; j++)
        {
            collisionData.triangles.Add(colOffset+trTop[j]);
        }
        colOffset += a;
        #endregion
    }
}
