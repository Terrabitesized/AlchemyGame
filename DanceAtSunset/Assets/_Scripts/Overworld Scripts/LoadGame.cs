using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void loadIntoScene()
    {
        StaticOverworldData.CreateNewGame = false;
        StaticOverworldData.LoadFromMainMenu = true;
        SceneManager.LoadScene("CyreneTestScene");
    }

    public void loadNewGame()
    {
        StaticOverworldData.CreateNewGame = true;
        StaticOverworldData.LoadFromMainMenu = true;
        SceneManager.LoadScene("CyreneTestScene");
    }

    public void quitGame()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void openSettings()
    {

    }
}
