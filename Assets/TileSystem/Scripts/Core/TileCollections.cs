using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KL.TileSystem
{
    [CreateAssetMenu(fileName = "TileCollections")]
    public class TileCollections : ScriptableObject
    {
        public Dictionary<string, TileCollection> colls = new Dictionary<string, TileCollection>();
        public List<TileCollection> collections = new List<TileCollection>();
        [HideInInspector] public Texture2DArray textures;

        public int textureWidth = 32;
        public int textureHeight = 32;
        public TextureFormat textureFormat;

        public void BuildDictionary()
        {
            int depth = 0;
            colls.Clear();
            foreach (TileCollection tb in collections)
            {
                colls.Add(tb.prefix, tb);
                tb.BuildDictionary();
                foreach (TileBase t in tb.tiles)
                {
                    depth += t.textures.Length;
                }
            }
            textures = new Texture2DArray(textureWidth, textureHeight, depth, textureFormat, false);
            int c = 0;
            foreach (TileCollection tb in collections)
            {
                foreach (TileBase t in tb.tiles)
                {
                    t.textureBaseIndex = c;
                    for (int i = 0; i < t.textures.Length; i++)
                    {
                        textures.SetPixels(t.textures[i].GetPixels(), c);
                        c++;
                    }
                }
            }
            textures.Apply();
        }

        public TileBase GetTile(TileString id)
        {
            return colls[id.nspace].tls[id.tile];
        }
    }
}