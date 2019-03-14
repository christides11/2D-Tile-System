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

    public void InitChuns(int width, int height)
    {
        chunks = new Chunk[width,height];
    }

    public void AddChunk(int x, int y, int chunkWidth, int chunkHeight)
    {
        chunks[x, y] = Instantiate(chunkPrefab, transform).GetComponent<Chunk>();
        chunks[x, y].transform.localPosition = new Vector3(x*chunkWidth, y*chunkHeight, 0);
        chunks[x, y].position = new Vector2(x, y);
        chunks[x, y].cRender = this;
    }

    public void DestroyChunk(int x, int y)
    {
        if (chunks[x, y] != null)
        {
            Destroy(chunks[x, y]);
        }
    }

    string tl;
    public void RenderChunk(int x, int y, ChunkDefinition def)
    {
        Chunk c = chunks[x, y];
        for (int i = 0; i < def.fgTiles.GetLength(0); i++)
        {
            for (int j = 0; j < def.fgTiles.GetLength(1); j++)
            {
                tl = def.GetTile(i, j, true);
                if (tl != null) {
                    c.AddTile(i, j, TileCollection.GetTile(tl));
                }
            }
        }
        c.UpdateChunk();
    }

    public Chunk GetChunk(Vector2Int pos)
    {
        if(pos.x >= chunks.GetLength(0) || pos.x < 0 || pos.y >= chunks.GetLength(1) || pos.y < 0)
        {
            return null;
        }
        return chunks[pos.x, pos.y];
    }
}
