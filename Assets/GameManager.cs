using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public CameraFollow camFollow;
    public RoomFirstDungeonGenerator roomFirstDungeonGenerator;

    public Tilemap floorTileMap;
    public Tilemap wallTileMap;

    void Start()
    {
        GenerateNewDungeon();
    }

    void GenerateNewDungeon() 
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();

        roomFirstDungeonGenerator.ClearSpawnedObjects();
        roomFirstDungeonGenerator.CreateRooms();
        

        camFollow.GetComponent<CameraFollow>().FindTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
