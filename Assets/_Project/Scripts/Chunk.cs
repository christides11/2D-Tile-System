using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

public class Chunk : MonoBehaviour
{
    private CollisionData collisionData =  new CollisionData();
    private List<EdgeCollider2D> edgeColls = new List<EdgeCollider2D>();
    //private Mesh colMesh;

    private MeshData meshData = new MeshData();
    private Mesh mesh;

    private int faceOffset;
    private int colOffset;
    private int z = 0;

    //public PolygonCollider2D col;
    public Vector2 colPoints;
    public MeshRenderer mr;
    [HideInInspector] public ChunkRenderer cRender;
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public MapLayers layer;

    public bool collision = false;
    public Vector2 scale;
    public Vector2 offset;

    public bool update = true;
    public bool rendered;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshData.vertices = new List<Vector3>();
        meshData.triangles = new List<int>();
        meshData.uv = new List<Vector3>();
        collisionData.vertices = new List<Vector3>();
        collisionData.triangles = new List<int>();
        if (collision)
        {
            //colMesh = new Mesh();
        }
    }

    public void Update()
    {
        if (update)
        {
            update = false;
            UpdateChunk();
            if (collision)
            {
                UpdateCollision();
            }
        }
    }

    void UpdateChunk()
    {
        rendered = true;
        //Profiler.BeginSample("BUILD CHUNK");
        BuildChunk();
        //Profiler.EndSample();
        //Profiler.BeginSample("RENDER CHUNK");
        RenderChunk();
        //Profiler.EndSample();
        //Profiler.BeginSample("COLLISION CHUNK");
        if (collision)
        {
            //UpdateCollision();
        }
        //Profiler.EndSample();
    }

    public void DestroyChunk()
    {

    }

    ChunkDefinition cd;
    string tl;
    TileBase tb;
    void BuildChunk()
    {
        //Profiler.BeginSample("1");
        cd = cRender.mm.map.chunks[position.x, position.y];
        //Profiler.EndSample();
        //Profiler.BeginSample("2");
        for(int i = 0; i < cRender.mm.chunkWidth; i++)
        {
            for(int j = 0; j < cRender.mm.chunkHeight; j++)
            {
                //Profiler.BeginSample("2-1");
                tl = cd.GetTile(i, j, layer);
                //Profiler.EndSample();
                //Profiler.BeginSample("2-2");
                if (!ReferenceEquals(tl, null))
                {
                    tb = TileCollection.GetTile(tl);
                    AddTile(i, j, tb);
                    //AddCollider(i, j, tb);
                }
                //Profiler.EndSample();
            }
        }
        //Profiler.EndSample();
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

    List<Vector2> recalcPoints = new List<Vector2>();
    List<Vector2> ignorePoints = new List<Vector2>();
    short[,] chu;
    ChunkDefinition chd;
    void UpdateCollision()
    {
        RecycleColls();
        chd = cRender.mm.map.chunks[position.x, position.y];
        chu = chd.tileLayers[layer];
        for (int y = 0; y < cRender.mm.chunkHeight; y++)
        {
            for (int x = 0; x < cRender.mm.chunkWidth; x++)
            {
                Vector2 vt = new Vector2(x, y);
                if (!ignorePoints.Contains(vt) || recalcPoints.Contains(vt))
                {
                    if (chu[x, y] != 0)
                    {
                        if (x == 0 && y == 0)
                        {
                            GenerateIsland(x, y);
                        }else
                        {
                            if (x - 1 >= 0 && y-1 >= 0)
                            {
                                if (chu[x - 1, y] == 0 && chu[x, y-1] == 0)
                                {
                                    GenerateIsland(x, y);
                                }
                            } else if(x-1 >= 0)
                            {
                                if (chu[x - 1, y] == 0)
                                {
                                    GenerateIsland(x, y);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        //Add collision
        //colMesh.Clear();
        //colMesh.vertices = collisionData.vertices.ToArray();
        //colMesh.triangles = collisionData.triangles.ToArray();
        //colMesh.RecalculateNormals();
        //col.sharedMesh = colMesh;
        //Cleanup
        //collisionData.vertices.Clear();
        //collisionData.triangles.Clear();

        colOffset = 0;
    }

    List<Vector2> points = new List<Vector2>();
    List<Vector2> realPts = new List<Vector2>();
    Vector2Int currentpoint = new Vector2Int(0, 0);
    Vector2Int dir = new Vector2Int(1, 0);
    int cnt = 0;
    private void GenerateIsland(int x, int y)
    {
        Profiler.BeginSample("Generate Island");
        Profiler.BeginSample("1");
        points.Clear();
        realPts.Clear();
        dir = Vector2Int.right;
        //currentpoint = new Vector2Int(x, y);
        currentpoint.x = x;
        currentpoint.y = y;
        points.Add(currentpoint);
        realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y, MapLayers.FG)).GetCollisionBtm(currentpoint.x, currentpoint.y));
        currentpoint += dir;
        while (!(points[0] == currentpoint))
        {
            if(dir == Vector2Int.right)
            {
                if (currentpoint.x >= chu.GetLength(0))
                {
                    realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x - 1, currentpoint.y, MapLayers.FG)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                    dir = Vector2Int.up;
                } else if (chu[currentpoint.x, currentpoint.y] == 0)
                {
                    realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x - 1, currentpoint.y, MapLayers.FG)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                    dir = Vector2Int.up;
                }else if (currentpoint.y-1 >= 0)
                {
                    if (chu[currentpoint.x, currentpoint.y-1] != 0)
                    {
                        realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y - 1, MapLayers.FG)).GetCollisionLeft(currentpoint.x, currentpoint.y - 1));
                        dir = Vector2Int.down;
                        //if(chu[currentpoint.x+1, currentpoint.y] != 0)
                        //{
                        //ignorePoints.Add(new Vector2(currentpoint.x+1,currentpoint.y));
                        //}
                    } else
                    {
                        realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y, MapLayers.FG)).GetCollisionBtm(currentpoint.x, currentpoint.y));
                    }
                } else
                {
                    realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y, MapLayers.FG)).GetCollisionBtm(currentpoint.x, currentpoint.y));
                }
            } else if(dir == Vector2Int.up)
            {
                if (currentpoint.y >= chu.GetLength(1))
                {
                    dir = Vector2Int.left;
                } else
                {
                    if (chu[currentpoint.x - 1, currentpoint.y] == 0)
                    {
                        realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x-1, currentpoint.y - 1, MapLayers.FG)).GetCollisionTopBk(currentpoint.x-1, currentpoint.y - 1));
                        dir = Vector2Int.left;
                        if (currentpoint.x < chu.GetLength(0) && currentpoint.y < chu.GetLength(1))
                        {
                            if (chu[currentpoint.x, currentpoint.y] != 0)
                            {
                                recalcPoints.Add(currentpoint);
                            }
                        }
                    } else if (chu[currentpoint.x - 1, currentpoint.y] != 0)
                    {
                        if (currentpoint.x < chu.GetLength(0))
                        {
                            if (chu[currentpoint.x, currentpoint.y] != 0)
                            {
                                dir = Vector2Int.right;
                            } else
                            {
                                realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x - 1, currentpoint.y, MapLayers.FG)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                            }
                            ignorePoints.Add(new Vector2(currentpoint.x - 1, currentpoint.y));
                        } else
                        {
                            realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x - 1, currentpoint.y, MapLayers.FG)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                        }
                    }
                }
                //if(chu[currentpoint.x-1, currentpoint.y] != 0)
                //{
                    //ignorePoints.Add(new Vector2(currentpoint.x-1, currentpoint.y));
                //}
            }else if(dir == Vector2Int.left)
            {
                if (currentpoint.x - 1 < 0)
                {
                    dir = Vector2Int.down;
                    realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y - 1, MapLayers.FG)).GetCollisionLeft(currentpoint.x, currentpoint.y - 1));
                } //else if (currentpoint.y - 1 < 0)
                //{
                //    dir = Vector2Int.right;
                 else if (chu[currentpoint.x - 1, currentpoint.y - 1] == 0)
                {
                    realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y-1, MapLayers.FG)).GetCollisionLeft(currentpoint.x, currentpoint.y-1));
                    dir = Vector2Int.down;
                } else if (currentpoint.y < chu.GetLength(1)) { 
                    if (chu[currentpoint.x - 1, currentpoint.y] != 0)
                    {
                        dir = Vector2Int.up;
                    }
                }
            }else if(dir == Vector2Int.down)
            {
                if (currentpoint.y - 1 < 0)
                {
                    dir = Vector2Int.right;
                } else if (chu[currentpoint.x, currentpoint.y - 1] == 0)
                {
                    //realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x+1, currentpoint.y, MapLayers.FG)).GetCollisionBtm(currentpoint.x, currentpoint.y));
                    dir = Vector2Int.right;
                } else if (currentpoint.x-1 >= 0) {
                    if (chu[currentpoint.x - 1, currentpoint.y - 1] != 0 && chu[currentpoint.x - 1, currentpoint.y] == 0)
                    {
                        realPts.AddRange(TileCollection.GetTile(chd.GetTile(currentpoint.x, currentpoint.y - 1, MapLayers.FG)).GetCollisionTop(currentpoint.x, currentpoint.y - 1));
                        dir = Vector2Int.left;
                        //if (chu[currentpoint.x, currentpoint.y - 1] != 0)
                        //{
                            //ignorePoints.Add(new Vector2(currentpoint.x, currentpoint.y-1));
                        //}
                    }
                }
            }
            points.Add(currentpoint);
            currentpoint += dir;
        }
        Profiler.EndSample();
        Profiler.BeginSample("2");
        //col.pathCount += 1;
        //col.SetPath(cnt, points.ToArray());
        //col.SetPath(cnt, realPts.ToArray());
        EdgeCollider2D ec = GetColl();
        ec.points = realPts.ToArray();
        ignorePoints.AddRange(points);
        cnt += 1;
        Profiler.EndSample();
        Profiler.EndSample();
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
    int[] trTop;
    Vector3[] clVertices;
    void AddCollider(int x, int y, TileBase t)
    {
        /*
        a = 0;
        #region Top
        clVertices = t.GetCollisionTop(out trTop);
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
        */
    }

    EdgeCollider2D GetColl()
    {
        for (int i = 0; i < edgeColls.Count; i++)
        {
            if (!edgeColls[i].enabled)
            {
                edgeColls[i].enabled = true;
                return edgeColls[i];
            }
        }
        edgeColls.Add(gameObject.AddComponent<EdgeCollider2D>());
        return edgeColls[edgeColls.Count-1];
    }

    void RecycleColls()
    {
        for(int i = 0; i < edgeColls.Count; i++)
        {
            edgeColls[i].points = new Vector2[0];
            edgeColls[i].enabled = false;
        }
    }
}
