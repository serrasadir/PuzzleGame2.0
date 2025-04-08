using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private Transform gridParent;
    private const string PLAYER_LEVEL_KEY = "PlayerLevel";


    public float cubeSize = 1f;  // Cube prefab size
    public float spacingSize =1.4f;

    private int remainingMoves;

    public Cube[,] grid;

    private LevelData levelData;

    public HashSet<ObstacleCube> obstacleCubes = new HashSet<ObstacleCube>();

    public string[] randomColors = { "r", "b", "g", "y" }; //Available colors

    public static GridManager Instance { get; private set; } // Singleton instance

    public event EventHandler<OnLevelLoadedEventArgs> OnLevelLoaded;
    public class OnLevelLoadedEventArgs : EventArgs
    {
        public LevelData levelData;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentLevel = GetSavedLevel();
        LoadLevel(currentLevel);
        PoolManager poolManager = PoolManager.Instance.GetComponent<PoolManager>();
        poolManager.OnPoolCreated += HandlePoolCreated;
        CanvasManager.Instance.UpdateMoveCount(remainingMoves);
    }
    public Transform GetGridParent() => gridParent;

    private void LoadLevel(int levelNumber)
    {
        string levelFileName = $"Levels/level_{levelNumber:D2}";
        TextAsset jsonFile = Resources.Load<TextAsset>(levelFileName);

        if (jsonFile != null)
        {
            levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
            remainingMoves = levelData.move_count;
            Debug.Log($"Loaded Level {levelData.level_number} with Grid {levelData.grid_width}x{levelData.grid_height}");
        }
        else
        {
            Debug.LogError($"Level file not found: {levelFileName}.json");
        }
        OnLevelLoaded?.Invoke(this,new OnLevelLoadedEventArgs { levelData = levelData });
    }

    private void HandlePoolCreated(object sender, EventArgs e)
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        PoolManager poolManager = PoolManager.Instance;
        float spacing = cubeSize * spacingSize;

        grid = new Cube[levelData.grid_width, levelData.grid_height]; //2D arrayi başlat

        float startX = -(levelData.grid_width * spacing) / 2f + (spacing / 2f) ;
        float startY = -(levelData.grid_height * spacing) / 2f + (spacing / 2f);

        for (int y = 0; y < levelData.grid_height; y++)
        {
            for (int x = 0; x < levelData.grid_width; x++)
            {
                int index = y * levelData.grid_width + x;
                string cubeType = levelData.grid[index];

                if (cubeType == "rand")
                {
                    cubeType = randomColors[UnityEngine.Random.Range(0, randomColors.Length)];
                }

                if (poolManager.objectPool.ContainsKey(cubeType))
                {
                    GameObject cubeObj = poolManager.GetFromPool(cubeType);

                    if (cubeObj != null)
                    {
                        cubeObj.transform.position = new Vector2(startX + (x * spacing), startY + (y * spacing));
                        cubeObj.transform.parent = gridParent;
                        cubeObj.transform.localScale = Vector3.one * cubeSize;

                        Cube cube = cubeObj.GetComponent<Cube>();
                        if (cube != null)
                        {
                            cube.SetGridPosition(x, y); 
                        }

                        grid[x, y] = cube;
                        if (cube is ObstacleCube obstacle)
                        {
                            obstacleCubes.Add(obstacle);
                        }
                    }
                }
            }
        }
        Debug.Log("Grid generation complete!");
    }
    public void ClearGrid()
    {
        if (grid == null) return;

        PoolManager poolManager = PoolManager.Instance;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                {
                    string colorKey = (grid[x, y] is ColoredCube coloredCube) ? coloredCube.GetColor().ToString() : grid[x, y].gameObject.name;
                    if (poolManager.objectPool.ContainsKey(colorKey))
                    {
                        poolManager.ReturnToPool(grid[x, y].gameObject, colorKey);
                    }
                    else
                    {
                        Destroy(grid[x, y].gameObject);
                    }
                    grid[x, y] = null; 
                }
            }
        }
        Debug.Log("Gridc leared");
    }


    public Vector2 GetWorldPosition(int x, int y)
    {     
        float spacing = cubeSize * spacingSize;
        float startX = -(grid.GetLength(0) * spacing) / 2f + (spacing / 2f);
        float startY = -(grid.GetLength(1) * spacing) / 2f + (spacing / 2f);

        return new Vector2(startX + (x * spacing), startY + (y * spacing));
    }
    public void LoadNextLevel()
    {
        ClearGrid();
        currentLevel++;
        SaveLevel(currentLevel);
        if (currentLevel <= 10)
        {
            LoadLevel(currentLevel);
            GenerateGrid();
            remainingMoves = levelData.move_count;
            CanvasManager.Instance.NextLevel(remainingMoves);
        }
        else
        {
            //return to main menu
        }
    }
    public void RetryLevel()
    {
        Debug.Log("Restarting Level...");
        LoadLevel(currentLevel);
        GenerateGrid();
        remainingMoves = levelData.move_count;
        CanvasManager.Instance.RetryLevel(remainingMoves);
       
    }

    public void DecreaseMoveCount()
    {
        remainingMoves--;
        CanvasManager.Instance.UpdateMoveCount(remainingMoves); 
        if (remainingMoves <= 0)
        {
            if(obstacleCubes.Count > 0)
            {
                Debug.Log("Game Over! No moves left.");
                CanvasManager.Instance.ShowGameOver();
                ClearGrid();
            }
            else
            {

                CanvasManager.Instance.ShowGameSucceed();
                ClearGrid();
            }
        }

    }

    public void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(PLAYER_LEVEL_KEY, level);
        PlayerPrefs.Save();
        Debug.Log("Level saved: " + level);
    }

    public int GetSavedLevel()
    {
        return PlayerPrefs.GetInt(PLAYER_LEVEL_KEY, 1); // Eğer kayıtlı level yoksa, 1. seviyeden başlat
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }

}