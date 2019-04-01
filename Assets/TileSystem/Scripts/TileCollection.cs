using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName ="TileCollection")]
public class TileCollection : ScriptableObject
{
    public Dictionary<string, TileBase> tls = new Dictionary<string, TileBase>();
    public string prefix;
    public TileBase[] tiles = null;

    public void BuildDictionary()
    {
        tls.Clear();
        foreach(TileBase tb in tiles)
        {
            tls.Add(tb.id, tb);
        }
    }

    public TileBase GetTile(string id)
    {
        return tls[id];
    }
}
