using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Leave_Game : MonoBehaviour
{
    //離開遊戲頁面時
    [SerializeField] GameObject scoreBoard;
    public void leave_Game()
    {
        Temp_Store.Session_Score=int.Parse(scoreBoard.GetComponent<TextMeshProUGUI>().text);
        scoreBoard.GetComponent<TextMeshProUGUI>().text=0.ToString();
        SceneManager.LoadScene("Settlement_scene");
    }
}
