using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
public class Read_Write_data : MonoBehaviour
{
    //將data寫成json存入path
    public void write_json(Saved_data data)
    {
        string path = Path.Combine(Application.persistentDataPath, "saved.json");
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
        string path = Path.Combine(Application.persistentDataPath, "saved.json");
        string json = File.ReadAllText(path);
        try
        {
            Saved_data data=JsonUtility.FromJson<Saved_data>(json);
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
