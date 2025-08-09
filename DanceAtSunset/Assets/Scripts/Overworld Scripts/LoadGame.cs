using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void loadIntoScene()
    {
        SceneManager.LoadScene("NateTestScene");
    }
}
