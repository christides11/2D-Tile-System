using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

//http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html
public class TileRenderer : MonoBehaviour
{
    private MeshCollider col;
    private Mesh mesh;
    // This first list contains every vertex of the mesh that we are going to render
    public List<Vector3> newVertices = new List<Vector3>();
    // The triangles tell Unity how to build each section of the mesh joining
    // the vertices
    public List<int> newTriangles = new List<int>();
    // Tells Unity how the texture is aligned on each polygon
    public List<Vector2> newUV = new List<Vector2>();

    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    private int z = 0;
    public Vector2 scale;
    public Vector2 offset;

    private int off;
    private int colOff;

    Mesh newMesh;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        newMesh = new Mesh();
        col = GetComponent<MeshCollider>();
    }

    void UpdateMesh()
    {
        //Profiler.BeginSample("UPDATE MESH");
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        //mesh.uv = newUV.ToArray();
        mesh.RecalculateNormals();

        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();
        off = 0;
        //Profiler.EndSample();
        UpdateCollision();
    }

    void UpdateCollision()
    {
        //Profiler.BeginSample("UPDATE COLL");
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();

        colOff = 0;
        //Profiler.EndSample();
    }

    public void GenTerrain(TileBase t)
    {
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                if (j < 5)
                {
                    //GenCollider(i, j, t);
                    GenSquare(i, j, t);
                }
            }
        }
        UpdateMesh();
    }

    Vector2 vv;
    Vector3 vf = new Vector3(0, 0, 0);
    int a;
    void GenSquare(int x, int y, TileBase t)
    {
        //Profiler.BeginSample("A");
        a = 0;
        for(int i = 0; i < t.vertices.Length; i++)
        {
            vv = t.vertices[i];
            vf.x = (offset.x + x + vv.x) * scale.x;
            vf.y = (offset.y + y + vv.y) * scale.y;
            //newVertices.Add(new Vector3((offset.x+x+vv.x)*scale.x, (offset.y+y+vv.y)*scale.y, z));
            newVertices.Add(vf);
            a++;
        }
        //Profiler.EndSample();
        //Profiler.BeginSample("B");
        for(int j = 0; j < t.triangles.Length; j++)
        {
            newTriangles.Add(off+t.triangles[j]);
        }
        //Profiler.EndSample();
        off += a;

        //newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y + tUnit));
        //newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y + tUnit));
        //newUV.Add(new Vector2(tUnit * tStone.x + tUnit, tUnit * tStone.y));
        //newUV.Add(new Vector2(tUnit * tStone.x, tUnit * tStone.y));
    }

    Vector3 cv;
    Vector3 gf = new Vector3(0, 0, 0);
    void GenCollider(int x, int y, TileBase t)
    {
        a = 0; 
        for(int i = 0; i < t.colVertices.Length; i++)
        {
            cv = t.colVertices[i];
            gf.x = (offset.x + x + cv.x) * scale.x;
            gf.y = (offset.y + y + cv.y) * scale.y;
            gf.z = cv.z;
            colVertices.Add(gf);
        }

        for(int j = 0; j < t.colTriangles.Length; j++)
        {
            colTriangles.Add(colOff+t.colTriangles[j]);
        }
        colOff += a;
    }
}
