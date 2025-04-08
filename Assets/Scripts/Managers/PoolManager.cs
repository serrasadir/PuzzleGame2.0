using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CubePrefab
{
    public string key; //this matches the JSON key (e.g., "r", "g")
    public GameObject prefab; 
}

public class PoolManager : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    [Header("Prefab List")]
    public Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>();
    public List<CubePrefab> cubePrefabs = new List<CubePrefab>(); //list the cubes from the inspector for a more dynamic structure -> new cubes can be add easily 
    [SerializeField] private int cubesToAdd = 10;

    public static PoolManager Instance { get; private set; }

    public event EventHandler OnPoolCreated;  


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        
        CreatePrefabLookup();
        CreatePool();
    }

    private void HandleOnLevelLoaded(object sender, GridManager.OnLevelLoadedEventArgs e)
    {
        CreatePool();
    }

    private void CreatePrefabLookup() //I create this function to automize my game, If I want to add a new cube it can be done easily 
    {
        prefabLookup.Clear(); 
        foreach (var cube in cubePrefabs)
        {
            if (!prefabLookup.ContainsKey(cube.key))
            {
                prefabLookup.Add(cube.key, cube.prefab);
                objectPool.Add(cube.key, new Queue<GameObject>()); //automatically create pool queues
            }
        }
    }

    public void CreatePool()
    {
        Dictionary<string, int> objectCounts = CountObjectsInLevel();

        foreach (var key in prefabLookup.Keys)
        {
            int countInLevel = objectCounts.ContainsKey(key) ? objectCounts[key] : 0;

            for (int i = 0; i <countInLevel + cubesToAdd; i++)
            {
                if (prefabLookup.ContainsKey(key))
                {
                    AddToPool(key, prefabLookup[key]);
                }
            }
        }

        OnPoolCreated?.Invoke(this, EventArgs.Empty);
    }


    private Dictionary<string, int> CountObjectsInLevel()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        GridManager gridManager = GridManager.Instance;
        foreach (string cubeType in gridManager.GetLevelData().grid)
        {
            if (cubeType == "rand")
            {
                continue; //"rand" does not need to be counted
            }
            else if (!counts.ContainsKey(cubeType))
            {
                counts[cubeType] = 0;
            }
            counts[cubeType]++;
        }

        return counts;
    }

    public void ReturnToPool(GameObject obj, string key)
    {
        obj.SetActive(false); 
        objectPool[key].Enqueue(obj); 
    }


    private void AddToPool(string key, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        objectPool[key].Enqueue(obj);
    }

    public GameObject GetFromPool(string key)
    {
        if (!objectPool.ContainsKey(key) || objectPool[key].Count < 4)
        {
            for (int i = 0; i < 5; i++)
            {
                AddToPool(key, prefabLookup[key]);
            }
        }
        GameObject obj = objectPool[key].Dequeue();
        obj.SetActive(true);
        return obj;
    }


}
