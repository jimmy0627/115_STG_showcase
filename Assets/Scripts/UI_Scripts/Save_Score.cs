using TMPro;
using UnityEngine;

public class Save_Score : MonoBehaviour
{
    [SerializeField] GameObject inputField;
    //彈窗的存檔腳本
    public void save_Score()
    {
        ScoreEntry session_data=new ScoreEntry();
        //設定名稱和從暫存讀取分數
        session_data.score=Temp_Store.Session_Score;
        session_data.playername = inputField.GetComponent<TextMeshProUGUI>().text;
        //寫入存檔
        Read_Write_data write_Data=new Read_Write_data();
        write_Data.write_json(session_data);
    }
}
