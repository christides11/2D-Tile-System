using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName ="TileCollection")]
public class TileCollection : ScriptableObject
{
    public static Dictionary<string, TileBase> tls = new Dictionary<string, TileBase>();
    public string prefix;
    [SerializeField] private TileBase[] tiles;

    public void BuildDictionary()
    {
        tls.Clear();
        foreach(TileBase tb in tiles)
        {
            tls.Add(tb.id, tb);
        }
    }

    public static TileBase GetTile(string id)
    {
        return tls[id];
    }
}
