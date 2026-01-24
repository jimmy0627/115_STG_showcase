using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro;
public class Load_Rank : MonoBehaviour
{

    void Start()
    {
        Read_Write_data tool=transform.parent.parent.GetComponent<Read_Write_data>();
        TextMeshProUGUI Rank_borad=transform.GetComponent<TextMeshProUGUI>();
        Saved_data data=tool.read_json();

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
        // 1. 安全檢查：確保資料不是 null
        if (savedData == null || savedData.datas.Count == 0)
        {
            Debug.Log("Empty Save data");
            return new List<ScoreEntry>(); // 回傳空清單避免報錯
        }
        // 2. 核心邏輯
        var top5List = savedData.datas.OrderByDescending(entry => entry.score).Take(5).ToList();
        return top5List;
    }

}
