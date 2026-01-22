using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
public class Save_date : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Saved_data data;
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            ScoreEntry score=new ScoreEntry();
            score.playername="jimmy";
            score.score=i;
            data.datas.Add(score);
            score=null;
        }

        for (int j = 0; j < 5; j++)
        {
            Debug.Log(data.datas[j]);
        }
        string path = @"D:\VS new clone\115_STG_showcase\Assets\Saved\saved.json";
        string json=JsonUtility.ToJson(data,true);
        try
        {
            File.WriteAllText(path,json);
            Debug.Log("Write complete");
        }
        catch (System.Exception e)
        {
            
            Debug.Log("Write fail"+e);
        }
    }

}
