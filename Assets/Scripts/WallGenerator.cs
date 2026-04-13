using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class WallGenerator 
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualiser tilemapVisualiser)
    {

        var basicWallPositions = FindWallsInDirection(floorPositions, Direction2D.cardinalDirectionList);
        foreach (var position in basicWallPositions) 
        {
            tilemapVisualiser.PaintSingleBasicWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirection(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false) 
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
