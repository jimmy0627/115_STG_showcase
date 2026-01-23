using UnityEngine;
using UnityEngine.SceneManagement;
public class Mange_scenes : MonoBehaviour
{
    public string SceneName;
    //前往SceneName
    public void Change_Scenes(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    //離開遊戲
    public void Exit_game()
    {
        Application.Quit();
    }
}
