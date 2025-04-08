using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); //Sahne değişse bile yok olmasın
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void LoadMainMenu()
    {
        Debug.Log("main menu..");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameScene()
    {
        Debug.Log("game scene..");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}
