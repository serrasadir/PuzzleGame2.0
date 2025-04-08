using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public static CubeMover Instance { get; private set; }
    GridManager gridManager;
    PoolManager poolManager;
    public float spacing;

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
        gridManager = GridManager.Instance;
        poolManager = PoolManager.Instance;
        spacing = gridManager.spacingSize * gridManager.cubeSize;
    }

    public void ShiftCubesDown(int x)
    {
        Cube[,] grid = gridManager.grid;

        //check the entire col
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            if (grid[x, y] == null) // if a cell is null, start search for a cube
            {
                int aboveY = y + 1;

                while (aboveY < grid.GetLength(1) && grid[x, aboveY] == null)
                {
                    aboveY++; //find the topmost cube
                }

                if (aboveY < grid.GetLength(1) && grid[x, aboveY] != null)
                {
                    //make the above cube fall
                    Cube fallingCube = grid[x, aboveY];
                    grid[x, y] = fallingCube;
                    grid[x, aboveY] = null;


                    Vector3 targetPos = gridManager.GetWorldPosition(x, y);
                    fallingCube.MoveToTargetPos(targetPos);
                    fallingCube.SetGridPosition(x, y);
                }
            }
        }
        SpawnNewCubes(x);
    }

    private void SpawnNewCubes(int x)
    {
        Cube[,] grid = gridManager.grid;

        for (int y = grid.GetLength(1) - 1; y >= 0; y--)
        {
            if (grid[x, y] == null) //check the topmost empty cells
            {
                string[] randomColors = gridManager.randomColors;
                string randomType = randomColors[UnityEngine.Random.Range(0, randomColors.Length)];
                GameObject newCubeObj = PoolManager.Instance.GetFromPool(randomType);

                if (newCubeObj != null)
                {
                    Vector3 spawnPosition = gridManager.GetWorldPosition(x, grid.GetLength(1)); //start from the top
                    newCubeObj.transform.position = spawnPosition;

                    Cube newCube = newCubeObj.GetComponent<Cube>();
                    grid[x, y] = newCube;

                    if (newCube != null)
                    {
                        newCube.SetGridPosition(x, y);
                        Vector3 targetPosition = gridManager.GetWorldPosition(x, y);
                        newCube.MoveToTargetPos(targetPosition);
                    }
                }
            }
        }
    }

}
