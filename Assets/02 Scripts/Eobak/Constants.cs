using System.Collections.Generic;

namespace _02_Scripts.Eobak
{ 
    public class Constants 
    {
        public const string ServerURL = "http://localhost:3000";
        public const string GameServerURL = "ws://localhost:3000";
        public enum MultiplayManagerState
        {
            
        }
        
        public const int CoinConsumption = 100;
        public const int AdReward = 500;
        public static readonly Dictionary<string, int> PurchaseOptions = new Dictionary<string, int>()
        {
            { "1000", 3000 }, // 1000원, 3000코인
            { "1800", 6000 }, // 1800원, 6000코인
            { "2500", 10000 } // 2500원, 10000코인
        };
    }
}