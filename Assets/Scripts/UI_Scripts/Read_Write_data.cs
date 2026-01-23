using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
public class Read_Write_data : MonoBehaviour
{

    private string path = @"D:\VS new clone\115_STG_showcase\Assets\Saved\saved.json";
    public Saved_data data;
    //start為測試用途，可刪
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            ScoreEntry score=new ScoreEntry();
            score.playername="jimmy";
            score.score=Random.Range(0,1000);
            data.datas.Add(score);
            score=null;
        }
        write_json(data);
    }
    //將data寫成json存入path
    public void write_json(Saved_data data)
    {
        Debug.Log(path);
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
    //將path中的json轉成Saved_data回傳
    public Saved_data read_json()
    {
        string json = File.ReadAllText(path);
        try
        {
            Debug.Log("read complete");
            return data;
        }
        catch (System.Exception e)
        {
            
            Debug.Log("read fail"+e);
            return null;
        }
    }
}
