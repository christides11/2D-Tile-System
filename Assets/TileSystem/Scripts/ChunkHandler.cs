using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles deciding which chunks we should be rendering.
/// </summary>
public class ChunkHandler : MonoBehaviour
{
    //Cached chunk offsets.
    Vector2Int[] chunkPos = {
    };

    [SerializeField] private MapManager mm = null;
    [SerializeField] private Transform player = null;
    public ChunkRenderer fg;
    public ChunkRenderer bg;
    public List<Vector2Int> loadedChunks = new List<Vector2Int>();
    [SerializeField] private int unloadChunksAfterTicks = 10;
    [SerializeField] private int renderRadius = 1;
    int unloadTimer = 0;

    private void Awake()
    {
        fg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
        bg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
        int width = (renderRadius + renderRadius) + 1;
        chunkPos = new Vector2Int[width*width];
        int x = 0;
        for(int i = -renderRadius; i <= renderRadius; i++)
        {
            for(int j = -renderRadius; j <= renderRadius; j++)
            {
                chunkPos[x] = new Vector2Int(i, j);
                x++;
            }
        }
        MapManager.OnTick += delegate { unloadTimer += 1; };
    }

    void Update()
    {
        
        if(mm.mapGenerated)
        {
            UnloadChunks();
            FindChunksToRender();
        }
    }

    #region Load/Unload Chunks
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

            if (loadedChunks.Contains(cPos))
            {
                continue;
            }

            GenChunk(cPos.x, cPos.y);
            loadedChunks.Add(cPos);
        }
    }

    List<Vector2Int> chunksToDelete = new List<Vector2Int>();
    Vector2Int dist;
    void UnloadChunks()
    {
        if (unloadTimer >= unloadChunksAfterTicks)
        {
            pChunk.x = Mathf.FloorToInt(player.position.x / (mm.chunkWidth*mm.scale.x));
            pChunk.y = Mathf.FloorToInt(player.position.y / (mm.chunkHeight*mm.scale.y));
            foreach(var ck in loadedChunks)
            {
                dist = ck - pChunk;
                if(dist.x > renderRadius || dist.x < -renderRadius || dist.y > renderRadius || dist.y < -renderRadius)
                {
                    chunksToDelete.Add(ck);
                }
            }
            foreach(var ck in chunksToDelete)
            {
                UnloadChunk(ck);
            }
            chunksToDelete.Clear();
            unloadTimer = 0;
        }
    }
    #endregion

    void GenChunk(int x, int y)
    {
        fg.AddChunk(x, y, mm.chunkWidth, mm.chunkHeight);
        bg.AddChunk(x, y, mm.chunkWidth, mm.chunkHeight);
    }

    void UnloadChunk(Vector2Int pos)
    {
        fg.DestroyChunk(pos.x, pos.y);
        bg.DestroyChunk(pos.x, pos.y);
        loadedChunks.Remove(pos);
    }
}
