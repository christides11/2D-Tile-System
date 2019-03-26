using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject minimap;
    private MapManager mm;
    private MapLayers ml;
    public float moveSpeed = 1.0f;
    public float fastSpeed = 2.0f;
    float curspeed;
    public int destroyRadius = 5;
    private Vector2 moveVelo;

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
        if (Input.GetKeyDown(KeyCode.M))
        {
            minimap.SetActive(!minimap.activeSelf);
        }
        curspeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;
        Vector2 moveVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveVelo = moveVec * curspeed;

        Vector2 pos;
        Vector2Int blockPos;
        if (Input.GetMouseButton(0))
        {
            pos = cam.ScreenToWorldPoint(Input.mousePosition);
            blockPos = mm.WorldToBlock(pos);
            for (int i = -destroyRadius; i < destroyRadius; i++)
            {
                for (int j = -destroyRadius; j < destroyRadius; j++)
                {
                    mm.SetTile(blockPos.x + i, blockPos.y + j, blockToPlace, ml);
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            pos = cam.ScreenToWorldPoint(Input.mousePosition);
            blockPos = mm.WorldToBlock(pos);
            for (int i = -destroyRadius; i < destroyRadius; i++)
            {
                for(int j = -destroyRadius; j < destroyRadius; j++)
                {
                    mm.SetTile(blockPos.x+i, blockPos.y+j, null, ml);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelo * Time.fixedDeltaTime);
    }
}
