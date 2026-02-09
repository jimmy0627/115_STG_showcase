using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Read_Write_data
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "saved.json");
    //將data寫成json存入path
    
    public void write_json(ScoreEntry data)
    {
        //先讀取現有資料
        Saved_data currentData = read_json();

        //加入新資料
        if (currentData == null) currentData = new Saved_data();
        currentData.datas.Add(data);

        //轉成 JSON 並存檔
        string json = JsonUtility.ToJson(currentData, true);

        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"Write complete: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Write fail: {e.Message}");
        }
    }
    //將path中的json轉成Saved_data回傳
    public Saved_data read_json()
    {
        //檢查檔案是否存在
        if (!File.Exists(SavePath))
        {
            // 如果檔案不存在，直接回傳一個新的空物件
            return new Saved_data(); 
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            Saved_data data = JsonUtility.FromJson<Saved_data>(json);
            if (data == null) data = new Saved_data();
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Read fail: {e.Message}");
            // 讀取失敗時，回傳空物件以防止遊戲崩潰
            return new Saved_data(); 
        }
    }
}
    

