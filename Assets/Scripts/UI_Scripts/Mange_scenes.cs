using UnityEngine;
using UnityEngine.SceneManagement;
public class Mange_scenes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string sceneName;
    public void Change_Scenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Exit_game()
    {
        Application.Quit();
    }
}
