using UnityEngine;
using System.Collections.Generic;

public class TNT : Cube
{
    private bool hasExploded = false;
    [SerializeField] private int explosionRadius = 1;


    private void OnMouseDown()
    {
        GridManager.Instance.DecreaseMoveCount();
        Explode();
    }

    public void Explode()
    {
        if (hasExploded)
            return;
        hasExploded = true;

        Vector2Int pos = GetGridPosition();
        Cube[,] grid = GridManager.Instance.grid;
        grid[pos.x, pos.y] = null;

        HashSet<int> affectedColumns = new HashSet<int>();

        for (int i = pos.x - explosionRadius; i <= pos.x + explosionRadius; i++)
        {
            for (int j = pos.y - explosionRadius; j <= pos.y + explosionRadius; j++)
            {
                if (i >= 0 && i < grid.GetLength(0) && j >= 0 && j < grid.GetLength(1))
                {
                    Cube cube = grid[i, j];
                    if (cube != null)
                    {
                        affectedColumns.Add(i);

                        //alanda başka bir TNT varsa patlama tetiklenir
                        if (cube is TNT otherTNT)
                        {
                            if (!otherTNT.hasExploded)
                            {
                                otherTNT.Explode();
                            }
                        }
                        else if (cube is ObstacleCube obstacle)
                        {
                            obstacle.TakeDamage();
                        }
                        else if (cube is ColoredCube coloredCube)
                        {
                            grid[i, j] = null;
                            string colorKey = coloredCube.GetColor().ToString();
                            PoolManager.Instance.ReturnToPool(coloredCube.gameObject, colorKey);
                        }
                        else
                        {
                            grid[i, j] = null;
                            cube.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        Debug.Log("TNT explosion!");
        BlastManager.Instance.AddAffectedColumns(affectedColumns);
        Destroy(gameObject);
    }
    public static void CreateTNT(int x, int y)
    {
        TNT tntPrefab = Resources.Load<TNT>("TNT");
        Vector2 worldPos = GridManager.Instance.GetWorldPosition(x, y);

        TNT newTNT = Instantiate(tntPrefab, worldPos, Quaternion.identity, GridManager.Instance.GetGridParent());
        newTNT.SetGridPosition(x, y);

        GridManager.Instance.grid[x, y] = newTNT;
    }

}
