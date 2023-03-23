using UnityEngine;
using UnityEngine.SceneManagement;
public class TEMPSceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
