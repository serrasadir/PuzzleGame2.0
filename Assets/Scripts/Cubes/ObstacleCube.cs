using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ObstacleCube : Cube
{
    [SerializeField] protected int durability = 1; //default 1
    public virtual bool AffectedByBlast => true;

    // Temel hasar alma metodu
    public virtual void TakeDamage()
    {
        GridManager.Instance.obstacleCubes.Remove(this);

        Vector2Int pos = GetGridPosition();
        GridManager.Instance.grid[pos.x, pos.y] = null;

        Destroy(gameObject);
        Debug.Log($"{GetType().Name} destroyed!");
        IsLevelCompleted();


    }

    public virtual void IsLevelCompleted()
    {
        if (GridManager.Instance.obstacleCubes.Count == 0)
        {
            Debug.Log("level accomplished.");
            CanvasManager.Instance.ShowGameSucceed();
            GridManager.Instance.ClearGrid();

        }
    }
}
