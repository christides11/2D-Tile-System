using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

//http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html
public class ChunkRenderer : MonoBehaviour
{
    [SerializeField]private GameObject chunkPrefab;
    public Chunk[,] chunks;
    public MapManager mm;
    public MapLayers layer;
    public Color color = Color.white;

    public void Awake()
    {
        mm.OnPlaceTile += SetTile;
    }

    public void InitChuns(int width, int height)
    {
        chunks = new Chunk[width,height];
    }

    public void AddChunk(int x, int y, int chunkWidth, int chunkHeight)
    {
        chunks[x, y] = Instantiate(chunkPrefab, transform).GetComponent<Chunk>();
        chunks[x, y].transform.localPosition = new Vector3(x*chunkWidth*mm.scale.x, y*chunkHeight*mm.scale.y, 0);
        chunks[x, y].position = new Vector2Int(x, y);
        chunks[x, y].scale = mm.scale;
        chunks[x, y].cRender = this;
        chunks[x, y].update = true;
        chunks[x, y].layer = layer;
        chunks[x, y].mr.material.SetColor("_Color", color);
    }

    public void DestroyChunk(int x, int y)
    {
        if (chunks[x, y] != null)
        {
            Destroy(chunks[x, y].gameObject);
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
        if (chunks[chunk.x, chunk.y])
        {
            chunks[chunk.x, chunk.y].update = true;
        }
    }
}
