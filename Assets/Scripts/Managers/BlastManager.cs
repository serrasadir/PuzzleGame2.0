using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BlastManager : MonoBehaviour
{
    public static BlastManager Instance { get; private set; }
    GridManager gridManager;
    PoolManager poolManager;
    public float spacing;
    [SerializeField] private CubeMover cubeMover;
    public HashSet<int> pendingAffectedColumns = new HashSet<int>();
    private bool isShiftScheduled = false;

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

    public void TryBlastCubes(int x, int y)
    {
        Cube[,] grid = gridManager.grid;
        if (grid[x, y] is not ColoredCube startCube) return;

        List<ColoredCube> connectedCubes = FindConnectedCubes(x, y);

        if (connectedCubes.Count >= 2)
        {
            HashSet<int> affectedColumns = new HashSet<int>(); 

            foreach (var cube in connectedCubes)
            {
                Vector2Int pos = cube.GetGridPosition();
                grid[pos.x, pos.y] = null; 
                RemoveCube(cube);

                affectedColumns.Add(pos.x);
            }
            if (connectedCubes.Count >= 5)
            {
                TNT.CreateTNT(x, y);
            }
            DamageAdjacentObstacles(connectedCubes, affectedColumns, grid);
            GridManager.Instance.DecreaseMoveCount();
            AddAffectedColumns(affectedColumns);

        }
        
    }

    private List<ColoredCube> FindConnectedCubes(int startX, int startY)
    {
        Cube[,] grid = gridManager.grid;
        List<ColoredCube> connectedCubes = new List<ColoredCube>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); //store the positionso we can track them
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        if (grid[startX, startY] is not ColoredCube startCube) return connectedCubes;

        queue.Enqueue(new Vector2Int(startX, startY));
        visited.Add(new Vector2Int(startX, startY));

        while (queue.Count > 0)
        {
            Vector2Int currentPos = queue.Dequeue();
            int x = currentPos.x, y = currentPos.y;

            ColoredCube currentCube = grid[x, y] as ColoredCube;
            if (currentCube == null) continue;

            connectedCubes.Add(currentCube);

            foreach (Vector2Int neighborPos in GetAdjacentPositions(x, y))
            {
                if (!visited.Contains(neighborPos))
                {
                    int nx = neighborPos.x, ny = neighborPos.y;
                    if (grid[nx, ny] is ColoredCube neighborCube && neighborCube.GetColor() == startCube.GetColor())
                    {
                        queue.Enqueue(neighborPos);
                        visited.Add(neighborPos);
                    }
                }
            }
        }
        return connectedCubes;
    }

    private List<Vector2Int> GetAdjacentPositions(int x, int y)
    {
        Cube[,] grid = gridManager.grid;
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;

            if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1))
            {
                neighbors.Add(new Vector2Int(newX, newY));
            }
        }

        return neighbors;
    }

    private void RemoveCube(ColoredCube cube)
    {
        cube.gameObject.SetActive(false);

        string colorKey = cube.GetColor().ToString();
        poolManager.ReturnToPool(cube.gameObject, colorKey);
    }
    // Add columns from a TNT explosion to the pending list and schedule the shift if not already scheduled.
    public void AddAffectedColumns(HashSet<int> columns)
    {
        foreach (int col in columns)
            pendingAffectedColumns.Add(col);

        if (!isShiftScheduled)
        {
            isShiftScheduled = true;
            StartCoroutine(ShiftPendingColumns());
        }
    }

    // Coroutine that waits one frame (or a short delay) before shifting all affected columns.
    private IEnumerator ShiftPendingColumns()
    {
        yield return null; // Wait one frame so that all chain reactions finish.
        foreach (int col in pendingAffectedColumns)
        {
            CubeMover.Instance.ShiftCubesDown(col);
        }
        pendingAffectedColumns.Clear();
        isShiftScheduled = false;
    }

    private void DamageAdjacentObstacles(List<ColoredCube> connectedCubes, HashSet<int> affectedColumns, Cube[,] grid)
    {
        HashSet<Vector2Int> processedObstacles = new HashSet<Vector2Int>();
        foreach (var cube in connectedCubes)
        {
            Vector2Int pos = cube.GetGridPosition();
            List<Vector2Int> adjacentPositions = GetAdjacentPositions(pos.x, pos.y);
            foreach (Vector2Int adjacent in adjacentPositions)
            {
                if (!processedObstacles.Contains(adjacent))
                {
                    if (grid[adjacent.x, adjacent.y] is ObstacleCube obstacle && obstacle.AffectedByBlast)
                    {
                        obstacle.TakeDamage();
                        // If the obstacle is destroyed, its TakeDamage() should set the grid cell to null.
                        if (grid[adjacent.x, adjacent.y] == null)
                        {
                            affectedColumns.Add(adjacent.x);
                        }
                    }
                    processedObstacles.Add(adjacent);
                }
            }
        }
    }

}
