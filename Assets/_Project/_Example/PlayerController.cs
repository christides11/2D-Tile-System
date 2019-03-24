using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Camera cam;
    private MapManager mm;
    private MapLayers ml;
    public float moveSpeed = 1.0f;
    public float fastSpeed = 2.0f;
    float curspeed;
    public int destroyRadius = 5;

    public string blockToPlace;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mm = MapManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ml = MapLayers.FG;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ml = MapLayers.BG;
        }
        curspeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;
        Vector2 vl = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            vl.y = curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vl.y = -curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vl.x = -curspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vl.x = curspeed * Time.deltaTime;
        }
        rb.velocity = vl;

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
                    mm.SetTile(blockPos.x + i, blockPos.y + j, blockToPlace, ml);
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
                    mm.SetTile(blockPos.x+i, blockPos.y+j, null, ml);
                }
            }
            Profiler.EndSample();
        }
    }
}
