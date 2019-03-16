using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private MapManager mm;
    public float moveSpeed = 1.0f;
    public float fastSpeed = 2.0f;
    float curspeed;
    public int destroyRadius = 5;

    public string blockToPlace;

    private void Start()
    {
        mm = MapManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        curspeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * curspeed * Time.deltaTime;
        }

        Vector2 pos;
        Vector2Int blockPos;
        if (Input.GetMouseButton(0))
        {
            Profiler.BeginSample("Place Multiple");
            pos = cam.ScreenToWorldPoint(Input.mousePosition);
            blockPos = mm.WorldToBlock(pos);
            for (int i = -destroyRadius; i < destroyRadius; i++)
            {
                for (int j = -destroyRadius; j < destroyRadius; j++)
                {
                    mm.SetTile(blockPos.x + i, blockPos.y + j, blockToPlace, MapLayers.FG);
                }
            }
            Profiler.EndSample();
        }

        if (Input.GetMouseButton(1))
        {
            Profiler.BeginSample("Destroy Multiple");
            pos = cam.ScreenToWorldPoint(Input.mousePosition);
            blockPos = mm.WorldToBlock(pos);
            for (int i = -destroyRadius; i < destroyRadius; i++)
            {
                for(int j = -destroyRadius; j < destroyRadius; j++)
                {
                    mm.SetTile(blockPos.x+i, blockPos.y+j, null, MapLayers.FG);
                }
            }
            Profiler.EndSample();
        }
    }
}
