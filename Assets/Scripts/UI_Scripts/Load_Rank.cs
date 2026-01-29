using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro;
public class Load_Rank : MonoBehaviour
{

    void Start()
    {
        Read_Write_data tool=transform.parent.parent.GetComponent<Read_Write_data>(); //存讀檔工具
        TextMeshProUGUI Rank_borad=transform.GetComponent<TextMeshProUGUI>(); //排行榜的文字
        Saved_data data=tool.read_json(); //讀取出來的存檔

        List<ScoreEntry> ranklist=new List<ScoreEntry>();
        ranklist=GetTop5HighScores(data);

        for (int i = 0; i <ranklist.Count; i++)
        {
            Debug.Log(ranklist[i].playername+" "+ranklist[i].score);
            Rank_borad.text+=(i+1)+".  "+ranklist[i].playername+" : "+ranklist[i].score+"\n";
        }
    }


    public List<ScoreEntry> GetTop5HighScores(Saved_data savedData)
    {
        Debug.Log(savedData.datas.Count);
        //安全檢查：確保資料不是 null
        if (savedData == null || savedData.datas.Count == 0)
        {
            Debug.Log("Empty Save data");
            return new List<ScoreEntry>(); // 回傳空清單避免報錯
        }
        // 依據Score從大排到小，並取出五個回傳
        var top5List = savedData.datas.OrderByDescending(entry => entry.score).Take(5).ToList();
        return top5List;
    }

}
