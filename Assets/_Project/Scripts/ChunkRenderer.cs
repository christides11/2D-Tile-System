using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

//http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html
public class ChunkRenderer : MonoBehaviour
{
    private List<Chunk> chunkPool = new List<Chunk>();
    [SerializeField]private GameObject chunkPrefab;
    public Chunk[,] chunks;
    public MapManager mm;
    public MapLayers layer;
    public Color color = Color.white;
    public bool collision;

    public void Awake()
    {
        MapManager.OnPlaceTile += SetTile;
    }

    public void InitChuns(int width, int height)
    {
        chunks = new Chunk[width,height];
    }

    public void AddChunk(int x, int y, int chunkWidth, int chunkHeight)
    {
        chunks[x, y] = GetChunkFromPool();
        Chunk c = chunks[x, y];
        c.transform.localPosition = new Vector3(x*chunkWidth*mm.scale.x, y*chunkHeight*mm.scale.y, 0);
        c.position = new Vector2Int(x, y);
        c.scale = mm.scale;
        c.cRender = this;
        c.collision = collision;
        c.update = true;
        c.layer = layer;
        c.mr.material.SetColor("_Color", color);
    }

    public void DestroyChunk(int x, int y)
    {
        if (chunks[x, y] != null)
        {
            chunks[x, y].gameObject.SetActive(false);
            chunks[x, y] = null;
        }
    }

    public Chunk GetChunk(Vector2Int pos)
    {
        if(pos.x >= chunks.GetLength(0) || pos.x < 0 || pos.y >= chunks.GetLength(1) || pos.y < 0)
        {
            return null;
        }
        return chunks[pos.x, pos.y];
    }

    void SetTile(Vector2Int chunk, Vector2Int pos, TileBase tb, MapLayers layer)
    {
        if (this.layer == layer)
        {
            if (chunks[chunk.x, chunk.y])
            {

                chunks[chunk.x, chunk.y].update = true;
            }
        }
    }

    Chunk GetChunkFromPool()
    {
        for(int i = 0; i < chunkPool.Count; i++)
        {
            if (!chunkPool[i].gameObject.activeSelf)
            {
                chunkPool[i].gameObject.SetActive(true);
                return chunkPool[i];
            }
        }
        chunkPool.Add(Instantiate(chunkPrefab, transform).GetComponent<Chunk>());
        return chunkPool[chunkPool.Count-1];
    }
}
