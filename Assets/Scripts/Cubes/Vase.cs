using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : ObstacleCube
{
    [SerializeField] private Sprite fullVaseSprite;   // Full vase sprite
    [SerializeField] private Sprite halfBrokenVaseSprite;  // Half-broken vase sprite
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        durability = 2; // Vase requires 2 hits to break
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the initial sprite
        if (spriteRenderer != null)
            spriteRenderer.sprite = fullVaseSprite;
    }

    public override void TakeDamage()
    {
        durability--;

        // Change the sprite when the vase is half-broken
        if (durability == 1)
        {
            spriteRenderer.sprite = halfBrokenVaseSprite;
            Debug.Log("Vase is half broken but still standing!");
        }

        // Destroy the vase if durability reaches 0
        if (durability <= 0)
        {
            GridManager.Instance.obstacleCubes.Remove(this);

            Vector2Int pos = GetGridPosition();
            GridManager.Instance.grid[pos.x, pos.y] = null;
            Destroy(gameObject);
            IsLevelCompleted();
        }
    }
}
