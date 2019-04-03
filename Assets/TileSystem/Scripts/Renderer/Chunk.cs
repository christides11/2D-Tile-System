using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;

namespace KL.TileSystem.Renderer
{
    public class Chunk : MonoBehaviour
    {
        private CollisionData collisionData = new CollisionData();
        private List<EdgeCollider2D> edgeColls = new List<EdgeCollider2D>();

        private MeshData meshData = new MeshData();
        private Mesh mesh;

        private int faceOffset;

        public MeshRenderer mr;
        [HideInInspector] public ChunkRenderer cRender;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public MapLayers layer;
        [HideInInspector] public GameObject[,] gameObjects;

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
            mr.material.SetTexture("_MainTex", MapManager.instance.tCollections.textures);
            gameObjects = new GameObject[MapManager.instance.chunkWidth, MapManager.instance.chunkHeight];
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
            BuildChunk();
            RenderChunk();
        }

        public void DestroyChunk()
        {

        }

        ChunkDefinition cd;
        void BuildChunk()
        {
            cd = cRender.mm.map.chunks[position.x, position.y];
            int hei = cRender.mm.chunkHeight;
            MapManager mm = MapManager.instance;
            for (int i = 0; i < cRender.mm.chunkWidth; i++)
            {
                for (int j = 0; j < hei; j++)
                {
                    TileString tl = cd.GetTile(i, j, layer);
                    if (!ReferenceEquals(tl, null))
                    {
                        TileBase tb = mm.GetTileFromCollection(tl);
                        AddTile(i, j, tb);
                        if (tb.gameObject) {
                            if (gameObjects[i, j] == null)
                            {
                                gameObjects[i, j] = GameObject.Instantiate(tb.gameObject, transform, false);
                                gameObjects[i, j].transform.localPosition = new Vector3(i, j) * scale;
                            }
                        }
                    } else
                    {
                        if (gameObjects[i, j] != null)
                        {
                            Destroy(gameObjects[i,j]);
                        }
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

        List<Vector2> recalcPoints = new List<Vector2>();
        List<Vector2> ignorePoints = new List<Vector2>();
        TileDefinition[,] chu;
        ChunkDefinition chd;
        void UpdateCollision()
        {
            RecycleColls();
            chd = cRender.mm.map.chunks[position.x, position.y];
            chu = chd.tileLayers[layer];
            ignorePoints.Clear();
            recalcPoints.Clear();
            for (int y = 0; y < cRender.mm.chunkHeight; y++)
            {
                for (int x = 0; x < cRender.mm.chunkWidth; x++)
                {
                    Vector2 vt = new Vector2(x, y);
                    if (!ignorePoints.Contains(vt) || recalcPoints.Contains(vt))
                    {
                        if (chu[x, y].tile != 0)
                        {
                            if (x == 0)
                            {
                                GenerateOutline(x, y);
                            } else
                            {
                                if (x - 1 >= 0 && y - 1 >= 0)
                                {
                                    if (chu[x - 1, y].tile == 0 && chu[x, y - 1].tile == 0)
                                    {
                                        GenerateOutline(x, y);
                                    } else if (chu[x - 1, y].tile == 0 && chu[x, y - 1].tile != 0)
                                    {
                                        //GenerateInline(x, y);
                                    }
                                } else if (x - 1 >= 0)
                                {
                                    if (chu[x - 1, y].tile == 0)
                                    {
                                        GenerateOutline(x, y);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        List<Vector2> points = new List<Vector2>();
        List<Vector2> realPts = new List<Vector2>();
        Vector2Int currentpoint = new Vector2Int(0, 0);
        Vector2Int dir = new Vector2Int(1, 0);
        private void GenerateOutline(int x, int y)
        {
            points.Clear();
            realPts.Clear();
            dir = Vector2Int.right;
            currentpoint.x = x;
            currentpoint.y = y;
            points.Add(currentpoint);
            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionBtm(currentpoint.x, currentpoint.y));
            currentpoint += dir;
            while (!(points[0] == currentpoint))
            {
                if (dir == Vector2Int.right)
                {
                    if (currentpoint.x >= chu.GetLength(0))
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y, layer)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                        dir = Vector2Int.up;
                    } else if (chu[currentpoint.x, currentpoint.y].tile == 0)
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y, layer)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                        dir = Vector2Int.up;
                    } else if (currentpoint.y - 1 >= 0)
                    {
                        if (chu[currentpoint.x, currentpoint.y - 1].tile != 0)
                        {
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y - 1, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y - 1));
                            dir = Vector2Int.down;
                        } else
                        {
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionBtm(currentpoint.x, currentpoint.y));
                        }
                    } else
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionBtm(currentpoint.x, currentpoint.y));
                    }
                } else if (dir == Vector2Int.up)
                {
                    if (currentpoint.y >= chu.GetLength(1))
                    {
                        dir = Vector2Int.left;
                    } else
                    {
                        if (chu[currentpoint.x - 1, currentpoint.y].tile == 0)
                        {
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y - 1, layer)).GetCollisionTopBk(currentpoint.x - 1, currentpoint.y - 1));
                            dir = Vector2Int.left;
                            if (currentpoint.x < chu.GetLength(0) && currentpoint.y < chu.GetLength(1))
                            {
                                if (chu[currentpoint.x, currentpoint.y].tile != 0)
                                {
                                    recalcPoints.Add(currentpoint);
                                }
                            }
                        } else if (chu[currentpoint.x - 1, currentpoint.y].tile != 0)
                        {
                            if (currentpoint.x < chu.GetLength(0))
                            {
                                if (chu[currentpoint.x, currentpoint.y].tile != 0)
                                {
                                    dir = Vector2Int.right;
                                } else
                                {
                                    realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y, layer)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                                }
                                ignorePoints.Add(new Vector2(currentpoint.x - 1, currentpoint.y));
                            } else
                            {
                                realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y, layer)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                            }
                        }
                    }
                } else if (dir == Vector2Int.left)
                {
                    if (currentpoint.x - 1 < 0)
                    {
                        dir = Vector2Int.down;
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y - 1, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y - 1));
                    } else if (chu[currentpoint.x - 1, currentpoint.y - 1].tile == 0)
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y - 1, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y - 1));
                        dir = Vector2Int.down;
                    } else if (currentpoint.y < chu.GetLength(1))
                    {
                        if (chu[currentpoint.x - 1, currentpoint.y].tile != 0)
                        {
                            dir = Vector2Int.up;
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x - 1, currentpoint.y, layer)).GetCollisionRight(currentpoint.x - 1, currentpoint.y));
                        }
                    } else
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y - 1, layer)).GetCollisionTopBk(currentpoint.x, currentpoint.y - 1));
                    }
                } else if (dir == Vector2Int.down)
                {
                    if (currentpoint.y - 1 < 0)
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y));
                        dir = Vector2Int.right;
                    } else if (chu[currentpoint.x, currentpoint.y - 1].tile == 0)
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y));
                        dir = Vector2Int.right;
                    } else if (currentpoint.x - 1 >= 0)
                    {
                        if (chu[currentpoint.x - 1, currentpoint.y - 1].tile != 0 && chu[currentpoint.x - 1, currentpoint.y].tile == 0)
                        {
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y - 1, layer)).GetCollisionTop(currentpoint.x, currentpoint.y - 1));
                            dir = Vector2Int.left;
                        } else
                        {
                            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y));
                        }
                    } else
                    {
                        realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y));
                    }
                }
                points.Add(currentpoint);
                currentpoint += dir;
            }
            realPts.Distinct();
            realPts.Add(realPts[0]);
            for (int o = 0; o < realPts.Count; o++)
            {
                realPts[o] *= scale;
            }
            EdgeCollider2D ec = GetColl();
            ec.points = realPts.ToArray();
            ignorePoints.AddRange(points);
        }

        private void GenerateInline(int x, int y)
        {
            points.Clear();
            realPts.Clear();
            dir = Vector2Int.up;
            currentpoint.x = x;
            currentpoint.y = y;
            points.Add(currentpoint);
            realPts.AddRange(MapManager.instance.GetTileFromCollection(chd.GetTile(currentpoint.x, currentpoint.y, layer)).GetCollisionLeft(currentpoint.x, currentpoint.y));
            currentpoint += dir;
            while (!(points[0] == currentpoint))
            {
                if (dir == Vector2Int.up)
                {

                } else if (dir == Vector2Int.left)
                {

                } else if (dir == Vector2Int.down)
                {

                } else if (dir == Vector2Int.right)
                {

                }
            }
            realPts.Distinct();
            realPts.Add(realPts[0]);
            EdgeCollider2D ec = GetColl();
            ec.points = realPts.ToArray();
            ignorePoints.AddRange(points);
        }

        Vector2 vv;
        Vector3 vf = new Vector3(0, 0, 0);
        void AddTile(int x, int y, TileBase t)
        {
            for (int i = 0; i < t.vertices.Length; i++)
            {
                vv = t.vertices[i];
                vf.x = (offset.x + x + vv.x) * scale.x;
                vf.y = (offset.y + y + vv.y) * scale.y;
                meshData.vertices.Add(vf);
            }
            for (int j = 0; j < t.triangles.Length; j++)
            {
                meshData.triangles.Add(faceOffset + t.triangles[j]);
            }
            faceOffset += t.vertices.Length;

            for (int w = 0; w < t.uvs.Length; w++)
            {
                t.uvs[w].z = t.GetTextureIndex(x+(position.x*MapManager.instance.chunkWidth), 
                    y+(position.y*MapManager.instance.chunkHeight), layer);
                meshData.uv.Add(t.uvs[w]);
            }
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
            return edgeColls[edgeColls.Count - 1];
        }

        void RecycleColls()
        {
            for (int i = 0; i < edgeColls.Count; i++)
            {
                edgeColls[i].points = new Vector2[0];
                edgeColls[i].enabled = false;
            }
        }
    }
}