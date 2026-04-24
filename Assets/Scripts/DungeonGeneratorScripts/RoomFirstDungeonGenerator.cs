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

    [Header("Prefabs To Spawn")]
    [SerializeField] 
    private List<SpawnableItem> enemyItems;
    [SerializeField] 
    private List<SpawnableItem> lootItems;
    [SerializeField] 
    private List<SpawnableItem> decorationItems;
    [SerializeField]
    private List<SpawnableItem> lightItems;
    [SerializeField] 
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject exitDoor;
    [SerializeField]
    private List<GameObject> bossPrefabs;

    private List<BoundsInt> _rooms;

    private List<GameObject> _spawnedObjects = new List<GameObject>();

    public override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    public void CreateRooms()
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
        
        SpawnAll(_rooms, floor);
    }

    public void ClearSpawnedObjects()
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        _spawnedObjects.Clear();
    }

    private void SpawnAll(List<BoundsInt> rooms, HashSet<Vector2Int> floor)
    {
        SpawnPlayer(rooms[0], floor);

        for (int i = 1; i < rooms.Count; i++)
        {
            List<Vector2Int> roomFloorTiles = GetFloorTilesInRoom(rooms[i], floor);
            if (roomFloorTiles.Count == 0) 
            {
                continue;
            } 

            SpawnItemInRoom(enemyItems, roomFloorTiles);
            SpawnItemInRoom(lootItems, roomFloorTiles);
            SpawnItemInRoom(lightItems, roomFloorTiles);
            SpawnItemInRoom(decorationItems, roomFloorTiles); //seperate list for torches and other things? quite easy to implement more
        }

        SpawnBossAndExit(rooms, floor);
    }
    private void SpawnPlayer(BoundsInt firstRoom, HashSet<Vector2Int> floor)
    {
        Vector2Int center = GetRoomCenter(firstRoom);

        Vector2Int spawnTile = FindNearestFloorTile(center, floor);

        //This spawns new player
        /*
        var player = Instantiate(playerPrefab, new Vector3(spawnTile.x, spawnTile.y, 0), Quaternion.identity);
        _spawnedObjects.Add(player);
        */
        //We will instead just change position of player everytime new dungeon generated

        playerPrefab.gameObject.transform.position = new Vector3(spawnTile.x, spawnTile.y, 0);

    }

    private void SpawnItemInRoom(List<SpawnableItem> items, List<Vector2Int> roomTiles)
    {
        if (items == null || items.Count == 0) return;

        int thingToSpawn = UnityEngine.Random.Range(0, items.Count);
        SpawnableItem item = items[thingToSpawn];

        var available = new List<Vector2Int>(roomTiles);
        Shuffle(available);

        int spawned = 0;
        foreach (var tile in available)
        {
            if (spawned >= item.maxPerRoom) break;

            if (UnityEngine.Random.value <= item.spawnChance)
            {
                var spawned_obj = Instantiate(item.prefab, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
                _spawnedObjects.Add(spawned_obj);
                spawned++;
            }
        }
    }
    
    private void SpawnBossAndExit(List<BoundsInt> rooms, HashSet<Vector2Int> floor) 
    {
        if ((exitDoor == null && bossPrefabs.Count == 0)  || rooms.Count < 2)
        {
            return;
        }

        Vector2Int startCenter = GetRoomCenter(rooms[0]);
        BoundsInt furthestRoom = rooms[1];
        float maxDistance = 0f;

        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int center = GetRoomCenter(rooms[i]);
            float distance = Vector2Int.Distance(startCenter, center);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoom = rooms[i];
            }
        }

        Vector2Int roomCenter = GetRoomCenter(furthestRoom);
        Vector2Int exitTile = FindNearestFloorTile(roomCenter, floor);

        var exit = Instantiate(exitDoor, new Vector3(exitTile.x, exitTile.y, 0), Quaternion.identity);
        _spawnedObjects.Add(exit);

        int bossToSpawn = UnityEngine.Random.Range(0, bossPrefabs.Count);
        var boss = Instantiate(bossPrefabs[bossToSpawn], new Vector3(exitTile.x, exitTile.y, 0), Quaternion.identity); //spawning boss for now, would like own function
        _spawnedObjects.Add(boss);//boss just spawns in same room as exit, would like seperate rooms so player can collect key?

    }
    private List<Vector2Int> GetFloorTilesInRoom(BoundsInt room, HashSet<Vector2Int> floor)
    {
        var tiles = new List<Vector2Int>();
        for (int col = offset; col < room.size.x - offset; col++)
        {
            for (int row = offset; row < room.size.y - offset; row++)
            {
                var pos = (Vector2Int)room.min + new Vector2Int(col, row);
                if (floor.Contains(pos) && HasAllNeighbours(pos, floor)) 
                {
                    tiles.Add(pos);
                }
            }
        }
        return tiles;
    }
    private bool HasAllNeighbours(Vector2Int pos, HashSet<Vector2Int> floor)
    {
        for (int x = -1; x <= 1; x++) 
        {
            for (int y = -1; y <= 1; y++) 
            {
                if (!floor.Contains(pos + new Vector2Int(x, y))) 
                {
                    return false;
                }   
            }
        }     
        return true;
    }
    private Vector2Int GetRoomCenter(BoundsInt room) => new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y));

    private Vector2Int FindNearestFloorTile(Vector2Int origin, HashSet<Vector2Int> floor)
    {
        if (floor.Contains(origin)) 
        {
            return origin;
        } 
        for (int radius = 1; radius < 10; radius++) 
        {
            for (int x = -radius; x <= radius; x++) 
            {
                for (int y = -radius; y <= radius; y++)
                {
                    var candidate = origin + new Vector2Int(x, y);
                    if (floor.Contains(candidate)) return candidate;
                }
            }
               
        }
        return origin;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); //i fear having so many random function calls is a bit inefficient 
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
