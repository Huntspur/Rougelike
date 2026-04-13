using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualiser : MonoBehaviour
{
    [SerializeField]
    private Tilemap floor;
    [SerializeField]
    private TileBase floorTile; //Array select random ones


    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) 
    {

        PaintTiles(floorPositions, floor, floorTile);
    
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions) 
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() 
    {
        floor.ClearAllTiles();
    }
}
