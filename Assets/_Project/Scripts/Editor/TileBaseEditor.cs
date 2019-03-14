using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileBase), true, isFallback = true)]
public class TileBaseEditor : Editor
{
    TileBase _target;
    bool foldo;

    public override void OnInspectorGUI()
    {
        _target = (TileBase)target;

        _target.id = EditorGUILayout.TextField("ID", _target.id);
        _target.tileName = EditorGUILayout.TextField("Name", _target.tileName);

        foldo = EditorGUILayout.Foldout(foldo, "Block Space");
        if (foldo)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"x: {_target.collX} y: {_target.collY}");
            _target.collPivot = EditorGUILayout.Vector2IntField("Coll Pivot", _target.collPivot);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Width"))
            {
                _target.collX -= 1;
                _target.coll = new bool[_target.collX * _target.collY];
            }
            if (GUILayout.Button("Add Width"))
            {
                _target.collX += 1;
                _target.coll = new bool[_target.collX * _target.collY];
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Height"))
            {
                _target.collY -= 1;
                _target.coll = new bool[_target.collX * _target.collY];
            }
            if (GUILayout.Button("Add Height"))
            {
                _target.collY += 1;
                _target.coll = new bool[_target.collX * _target.collY];
            }
            EditorGUILayout.EndHorizontal();

            for (int i = _target.collY-1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < _target.collX; j++)
                {
                    _target.coll[j + i * _target.collX] = EditorGUILayout.Toggle(_target.coll[j + i * _target.collX]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_target);

        }
    }
}
