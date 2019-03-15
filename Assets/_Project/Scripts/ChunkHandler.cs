using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles deciding which chunks we should be rendering.
/// </summary>
public class ChunkHandler : MonoBehaviour
{
    static Vector2Int[] chunkPos = {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    [SerializeField] private MapManager mm;
    [SerializeField] private Transform player;
    public ChunkRenderer fg;
    public ChunkRenderer bg;
    [HideInInspector] public List<Vector2Int> loadedChunks = new List<Vector2Int>();
    int unloadTimer = 0;

    public string tempTile;

    private void Awake()
    {
        fg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
        bg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
    }

    void Update()
    {
        if(mm.mapGenerated)
        {
            UnloadChunks();
            FindChunksToRender();
        }
    }

    Vector2Int pChunk = new Vector2Int();
    Vector2Int cPos = new Vector2Int();
    Chunk nChunk;
    void FindChunksToRender()
    {
        pChunk.x = Mathf.FloorToInt(player.position.x / (mm.chunkWidth*mm.scale.x));
        pChunk.y = Mathf.FloorToInt(player.position.y / (mm.chunkHeight*mm.scale.y));

        for (int i = 0; i < chunkPos.Length; i++)
        {
            cPos.x = chunkPos[i].x + pChunk.x;
            cPos.y = chunkPos[i].y + pChunk.y;

            if (cPos.x < 0 || cPos.x >= mm.chunksX || cPos.y < 0 || cPos.y >= mm.chunksY)
            {
                continue;
            }

            nChunk = fg.GetChunk(cPos);

            if (nChunk != null && nChunk.rendered)
                continue;

            GenChunk(cPos.x, cPos.y);
            loadedChunks.Add(cPos);
        }
    }

    List<Vector2Int> chunksToDelete = new List<Vector2Int>();
    void UnloadChunks()
    {
        if (unloadTimer >= 10)
        {
            pChunk.x = Mathf.FloorToInt(player.position.x / (mm.chunkWidth*mm.scale.x));
            pChunk.y = Mathf.FloorToInt(player.position.y / (mm.chunkHeight*mm.scale.y));
            foreach(var ck in loadedChunks)
            {
                Vector2Int dist = ck - pChunk;
                if(dist.x > 1 || dist.x < -1 || dist.y > 1 || dist.y < -1)
                {
                    chunksToDelete.Add(ck);
                }
            }
            foreach(var ck in chunksToDelete)
            {
                UnloadChunk(ck.x, ck.y);
            }
            chunksToDelete.Clear();
            unloadTimer = 0;
        }
        unloadTimer++;
    }

    void GenChunk(int x, int y)
    {
        fg.AddChunk(x, y, mm.chunkWidth, mm.chunkHeight);
        fg.RenderChunk(x, y, mm.map.chunks[x,y]);
    }

    void UnloadChunk(int x, int y)
    {
        fg.DestroyChunk(x, y);
        loadedChunks.Remove(new Vector2Int(x,y));
        //chunksToDelete.Remove(new Vector2Int(x,y));
    }
}
