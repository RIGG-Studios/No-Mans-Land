using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(OceanManager))]
public class OceanEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OceanManager ocean = target as OceanManager;


        if (GUILayout.Button("Generate Tiles"))
        {
            ocean.GetComponentInChildren<OceanTileManager>().ResetTiles();
            ocean.GetComponentInChildren<OceanTileManager>().GenerateTiles();
        }

        if (GUILayout.Button("Reset Tiles"))
        {
            ocean.GetComponentInChildren<OceanTileManager>().ResetTiles();
        }
    }
}
