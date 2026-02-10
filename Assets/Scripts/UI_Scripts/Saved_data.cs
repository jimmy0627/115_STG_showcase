using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class Saved_data
{
    public List<ScoreEntry> datas = new List<ScoreEntry>(); //存檔單元串
}

[Serializable]
public class ScoreEntry //一個標準的存檔單元
{
    public int score; //分數
    public string playername; //玩家名稱
}