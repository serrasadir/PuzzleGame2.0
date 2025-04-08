using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColoredCube : Cube
{
    
    [SerializeField] private CubeColor cubeColor;

    private void OnMouseDown()
    {
        BlastManager.Instance.TryBlastCubes(gridPosition.x, gridPosition.y); // X, Y’yi gönderiyoruz
    }

    public CubeColor GetColor()
    {
        return cubeColor;
    }
}


public enum CubeColor
{
    r,
    b,
    g,
    y
}
