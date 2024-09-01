using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Tilemap))]
public class TilemapMirrorer : Editor
{
    private void OnEnable()
    {
        Tilemap.tilemapTileChanged += OnTileChanged;
    }

    private void OnTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles)
    {
        TileBase[] tiles = new TileBase[syncTiles.Length];
        Vector3Int[] positions = new Vector3Int[syncTiles.Length];

        for (int i = 0; i < syncTiles.Length; i++)
        {
            tiles[i] = syncTiles[i].tile;
            positions[i] = new Vector3Int(-(syncTiles[i].position.x + 1), syncTiles[i].position.y, syncTiles[i].position.z);
        }

        tilemap.SetTiles(positions, tiles);
    }

    private void OnDisable()
    {
        Tilemap.tilemapTileChanged -= OnTileChanged;
    }
}
