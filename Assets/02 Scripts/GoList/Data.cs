
using System.Collections.Generic;

[System.Serializable]
public class OmokData
{
    public int TurnNumber;
    public string Player;
    //public string Color;
    public int X, Y;
}

[System.Serializable]
public class GameRecord
{
    public List<OmokData> Data = new List<OmokData>();
}