using TMPro;
using UnityEngine;

public class Popour_setting : MonoBehaviour
{
    //設定彈窗分數顯示
    [SerializeField]GameObject ScoreBorad;
    void Awake()
    {
        ScoreBorad.GetComponent<TextMeshProUGUI>().text=Temp_Store.Session_Score.ToString();
    }
}
