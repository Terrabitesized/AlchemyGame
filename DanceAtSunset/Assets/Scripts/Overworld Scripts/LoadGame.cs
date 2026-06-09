using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void loadIntoScene()
    {
        StaticOverworldData.createNewGame = false;
        StaticOverworldData.loadFromMainMenu = true;
        SceneManager.LoadScene("CyreneTestScene");
    }

    public void loadNewGame()
    {
        StaticOverworldData.createNewGame = true;
        StaticOverworldData.loadFromMainMenu = true;
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
