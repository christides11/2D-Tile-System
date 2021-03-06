﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KL.TileSystem
{
    [CreateAssetMenu(fileName = "Base Tile", menuName = "Tiles/Base Tile")]
    public class TileBase : ScriptableObject
    {
        //public Matrix4x4 transform;
        [NonSerialized] public int textureBaseIndex;
        public string id;
        public string tileName;
        public GameObject gameObject;
        public Texture2D[] textures;
        [SerializeField] public bool[] coll = new bool[0];
        [SerializeField] public Vector2Int collPivot;
        [HideInInspector] public int collX;
        [HideInInspector] public int collY;
        public Vector2[] vertices;
        public int[] triangles;
        public Vector3[] uvs;

        public Vector2[] colVerticesBtm;
        public Vector2[] colVerticesRight;
        public Vector2[] colVerticesTop;
        public Vector2[] colVerticesLeft;

        public virtual Vector2[] GetCollisionBtm(int x, int y, bool backwards = false)
        {
            Vector2[] cbtm = new Vector2[colVerticesBtm.Length];
            if (backwards)
            {
                for (int i = colVerticesBtm.Length-1; i >= 0; i--)
                {
                    int index = colVerticesBtm.Length - 1 - i;
                    cbtm[index].x = colVerticesBtm[i].x + x;
                    cbtm[index].y = colVerticesBtm[i].y + y;
                }
            }
            else
            {
                for (int i = 0; i < colVerticesBtm.Length; i++)
                {
                    cbtm[i].x = colVerticesBtm[i].x + x;
                    cbtm[i].y = colVerticesBtm[i].y + y;
                }
            }
            return cbtm;
        }

        public virtual Vector2[] GetCollisionRight(int x, int y, bool backwards = false)
        {
            Vector2[] cbtm = new Vector2[colVerticesRight.Length];
            if (backwards)
            {
                for (int i = colVerticesRight.Length - 1; i >= 0; i--)
                {
                    int index = colVerticesRight.Length - 1 - i;
                    cbtm[index].x = colVerticesRight[i].x + x;
                    cbtm[index].y = colVerticesRight[i].y + y;
                }
            }
            else
            {
                for (int i = 0; i < colVerticesRight.Length; i++)
                {
                    cbtm[i].x = colVerticesRight[i].x + x;
                    cbtm[i].y = colVerticesRight[i].y + y;
                }
            }
            return cbtm;
        }

        public virtual Vector2[] GetCollisionLeft(int x, int y, bool backwards = false)
        {
            Vector2[] cbtm = new Vector2[colVerticesLeft.Length];
            if (backwards)
            {
                for (int i = colVerticesLeft.Length-1; i >= 0; i--)
                {
                    int index = colVerticesLeft.Length - 1 - i;
                    cbtm[index].x = colVerticesLeft[i].x + x;
                    cbtm[index].y = colVerticesLeft[i].y + y;
                }
            }
            else
            {
                for (int i = 0; i < colVerticesLeft.Length; i++)
                {
                    cbtm[i].x = colVerticesLeft[i].x + x;
                    cbtm[i].y = colVerticesLeft[i].y + y;
                }
            }
            return cbtm;
        }

        public virtual Vector2[] GetCollisionTop(int x, int y)
        {
            Vector2[] cbtm = new Vector2[colVerticesTop.Length];
            for (int i = 0; i < colVerticesTop.Length; i++)
            {
                cbtm[i].x = colVerticesTop[i].x + x;
                cbtm[i].y = colVerticesTop[i].y + y;
            }
            return cbtm;
        }

        public virtual Vector2[] GetCollisionTopBk(int x, int y)
        {
            Vector2[] cbtm = new Vector2[colVerticesTop.Length];
            for (int i = colVerticesTop.Length - 1; i >= 0; i--)
            {
                int index = colVerticesTop.Length - 1 - i;
                cbtm[index].x = colVerticesTop[i].x + x;
                cbtm[index].y = colVerticesTop[i].y + y;
            }
            return cbtm;
        }

        public virtual void OnAddedToMap(int x, int y, MapLayers layer)
        {

        }

        public virtual void RefreshTile(Vector2Int position, MapLayers layer)
        {

        }

        public virtual int GetTextureIndex(int x, int y, MapLayers layer)
        {
            return textureBaseIndex;
        }
    }
}