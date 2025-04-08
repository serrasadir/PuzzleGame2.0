using UnityEngine;
using System.Collections;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject succeed;
    public static CanvasManager Instance { private set; get; }

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
    public void UpdateMoveCount(int remainingMoves)
    {
        moveCountText.text = $"{remainingMoves}";
    }

    public void HideGameOver()
    {
        gameOver.SetActive(false);
    }

    public void ShowGameSucceed()
    {
        succeed.SetActive(true);
    }
    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }

    public void HideGameSucceed()
    {
        succeed.SetActive(false);
    }
    public void RetryLevel(int remainingMoves)
    {
        HideGameOver();
        HideGameSucceed();
        UpdateMoveCount(remainingMoves);
    }

    public void NextLevel(int remainingMoves)
    {
        HideGameSucceed();
        UpdateMoveCount(remainingMoves);
    }
}
