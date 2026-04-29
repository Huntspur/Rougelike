using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CameraFollow camFollow;
    public RoomFirstDungeonGenerator roomFirstDungeonGenerator;

    [Header("Tilemap data")]
    public Tilemap floorTileMap;
    public Tilemap wallTileMap;

    [Header("Progress")]
    public int dungeonLevel = 1;
    public int totalXP = 0;

    [Header("High Score")]
    public int highScoreDungeons;
    public int highScoreXP;

    [Header("Scaling")]
    public float healthScalePerLevel = 0.2f;  // +20% health per dungeon
    public float damageScalePerLevel = 0.1f;  // +10% damage per dungeon

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1) 
        {
            GenerateNewDungeon();
        }
    }

    private void Start()
    {
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameMusic);

        highScoreDungeons = PlayerPrefs.GetInt("HighScoreDungeons", 0);
        highScoreXP = PlayerPrefs.GetInt("HighScoreXP", 0);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);
        }
        else
        {
            AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameMusic);
        }
    }

    public void AddXP(int amount)
    {
        totalXP += amount;
        Debug.Log($"Total XP: {totalXP}");
        //UI here later
    }

    public void DungeonCleared()
    {
        dungeonLevel++;
        Debug.Log($"Dungeon Level: {dungeonLevel}");
    }
    public int GetScaledHealth(int baseHealth)
    {
        return Mathf.RoundToInt(baseHealth * (1 + healthScalePerLevel * (dungeonLevel - 1)));
    }

    // scaled damage for an enemy given its base damage
    public int GetScaledDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * (1 + damageScalePerLevel * (dungeonLevel - 1)));
    }

    public void GenerateNewDungeon()
    {
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameMusic);
        StartCoroutine(GenerationProcess());
    }

    public void SaveHighScore()
    {
        if (dungeonLevel > highScoreDungeons)
        {
            highScoreDungeons = dungeonLevel;
            PlayerPrefs.SetInt("HighScoreDungeons", highScoreDungeons);
        }
        if (totalXP > highScoreXP)
        {
            highScoreXP = totalXP;
            PlayerPrefs.SetInt("HighScoreXP", highScoreXP);
        }
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        dungeonLevel = 1;
        totalXP = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            AudioManager.Instance?.PlayMusic(AudioManager.Instance.mainMenuMusic);
        }
        else if (scene.buildIndex == 1)
        {
            
            camFollow = FindFirstObjectByType<CameraFollow>();
            roomFirstDungeonGenerator = FindFirstObjectByType<RoomFirstDungeonGenerator>();

            floorTileMap = GameObject.Find("FloorTilemap")?.GetComponent<Tilemap>();
            wallTileMap = GameObject.Find("WallTilemap")?.GetComponent<Tilemap>();

            AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameMusic);
            GenerateNewDungeon();
        }
    }

    private IEnumerator GenerationProcess() 
    {
        yield return new WaitForEndOfFrame(); 

        Boss.IsDefeated = false;
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();

        roomFirstDungeonGenerator.ClearSpawnedObjects();
        roomFirstDungeonGenerator.CreateRooms();

        camFollow.GetComponent<CameraFollow>().FindTarget();
    }
}
