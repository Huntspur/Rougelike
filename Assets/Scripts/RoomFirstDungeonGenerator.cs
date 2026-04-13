using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    [Header("Spawning")]
    [SerializeField] 
    private SpawnableItem enemyItem;
    [SerializeField] 
    private SpawnableItem lootItem;
    [SerializeField] 
    private SpawnableItem decorationItem;
    [SerializeField] 
    private GameObject playerPrefab;

    private List<BoundsInt> _rooms;
    private HashSet<Vector2Int> _floor;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        _rooms = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(_rooms);
        }
        else 
        {
            floor = CreateSimpleRooms(_rooms);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in _rooms) 
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualiser.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualiser);
        
        _floor = floor;
        SpawnAll(_rooms, floor);
    }

    private void SpawnAll(List<BoundsInt> rooms, HashSet<Vector2Int> floor)
    {
        SpawnPlayer(rooms[0], floor);

        for (int i = 1; i < rooms.Count; i++)
        {
            List<Vector2Int> roomFloorTiles = GetFloorTilesInRoom(rooms[i], floor);

            if (roomFloorTiles.Count == 0) continue;

            SpawnItemInRoom(enemyItem, roomFloorTiles);
            SpawnItemInRoom(lootItem, roomFloorTiles);
            SpawnItemInRoom(decorationItem, roomFloorTiles);
        }
    }
    private void SpawnPlayer(BoundsInt firstRoom, HashSet<Vector2Int> floor)
    {
        Vector2Int center = GetRoomCenter(firstRoom);

        // Walk outward from center until we find a valid floor tile
        Vector2Int spawnTile = FindNearestFloorTile(center, floor);
        Instantiate(playerPrefab, new Vector3(spawnTile.x, spawnTile.y, 0), Quaternion.identity);
    }

    private void SpawnItemInRoom(SpawnableItem item, List<Vector2Int> roomTiles)
    {
        if (item == null) return;

        // Shuffle a copy so picks are random without repetition
        var available = new List<Vector2Int>(roomTiles);
        Shuffle(available);

        int spawned = 0;
        foreach (var tile in available)
        {
            if (spawned >= item.maxPerRoom) break;
            if (UnityEngine.Random.value <= item.spawnChance)
            {
                Instantiate(item.prefab, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
                spawned++;
            }
        }
    }
    private List<Vector2Int> GetFloorTilesInRoom(BoundsInt room, HashSet<Vector2Int> floor)
    {
        var tiles = new List<Vector2Int>();
        for (int col = offset; col < room.size.x - offset; col++)
        {
            for (int row = offset; row < room.size.y - offset; row++)
            {
                var pos = (Vector2Int)room.min + new Vector2Int(col, row);
                if (floor.Contains(pos)) tiles.Add(pos);
            }
        }
        return tiles;
    }

    private Vector2Int GetRoomCenter(BoundsInt room)
        => new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y));

    private Vector2Int FindNearestFloorTile(Vector2Int origin, HashSet<Vector2Int> floor)
    {
        if (floor.Contains(origin)) return origin;
        // Spiral outward
        for (int radius = 1; radius < 10; radius++)
            for (int x = -radius; x <= radius; x++)
                for (int y = -radius; y <= radius; y++)
                {
                    var candidate = origin + new Vector2Int(x, y);
                    if (floor.Contains(candidate)) return candidate;
                }
        return origin;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++) 
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset)) 
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];

        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0) 
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y) 
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y) 
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x) 
        {
            if (destination.x > position.x) 
            {
                position += Vector2Int.right;

            }else if (destination.x < position.x )
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters) 
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance) 
            {
                distance = currentDistance;
                closest = position;
            }
            
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList) 
        {
            for (int col = offset; col < room.size.x - offset; col++) 
            {
                for (int row = offset; row < room.size.y - offset; row++) 
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}
