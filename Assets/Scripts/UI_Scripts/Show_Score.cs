using TMPro;
using UnityEngine;

public class Show_Score : MonoBehaviour
{
    void Awake()
    {
        //跨場景暫存分數用
        int score=Temp_Store.Session_Score;
        transform.GetComponent<TextMeshProUGUI>().text=score.ToString();
    }
}
