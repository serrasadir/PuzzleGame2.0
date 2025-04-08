using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Cube : MonoBehaviour
{
    public virtual bool AffectedByGravity => true;
    protected Vector2Int gridPosition;//store the grid position

    protected virtual void OnTap()
    {
        Debug.Log($"{GetType().Name} tapped.");
    }

    public void SetGridPosition(int x, int y)
    {
        gridPosition = new Vector2Int(x, y);
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    public void MoveToTargetPos(Vector3 targetPos)
    {
        StartCoroutine(MoveToPosition(targetPos));
    }

    private IEnumerator MoveToPosition(Vector3 target, float speed = 8f)
    {
        Vector3 start = transform.position;

        while (Vector3.Distance(transform.position, target) > 0.01f) // Hedefe yaklaşana kadar devam et
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target; // Kesin olarak hedefe ulaş
    }

}
