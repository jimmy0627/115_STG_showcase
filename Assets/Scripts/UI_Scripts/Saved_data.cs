using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class Saved_data
{
    public List<ScoreEntry> datas;
}

[Serializable]
public class ScoreEntry
{
    public int score;
    public string playername;
}