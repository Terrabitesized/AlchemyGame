using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager _instance;

    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // If it doesn't exist, create a new one
                if (_instance == null)
                {
                    GameObject go = new GameObject("TutorialManager");
                    _instance = go.AddComponent<TutorialManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
}
