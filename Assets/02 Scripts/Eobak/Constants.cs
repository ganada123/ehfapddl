namespace _02_Scripts.Eobak
{ 
    public class Constants 
    {
        public const string ServerURL = "http://localhost:3000";
        public enum MultiplayManagerState
        {
            JoinRoom,
            CreateRoom,
            OpponentJoinRoom,
            StartGame
        }
    }
}