using System.Collections.Generic;

namespace _02_Scripts.Eobak
{ 
    public class Constants 
    {
        public const string ServerURL = "http://localhost:3000";
        public const string GameServerURL = "ws://localhost:3000";
        public enum MultiplayManagerState
        {
            Connecting,             // 서버 연결 시도 중
            Connected,              // 서버 연결 완료
            JoiningMatchQueue,      // 매칭 큐 참여 요청 중
            InMatchQueue,           // 매칭 큐 참여 완료 (내부 상태)
            WaitingForMatch,        // 매칭 대기 중 (상대방 찾는 중)
            MatchWithAI,            // AI와 매칭됨
            RoomCreatedForMatch,    // 매치를 위한 방 생성 완료 (매칭 성공)
            SetColor,               // 색깔 설정 중 (서버에서 색깔 받기)
            ShowColor,              // 색깔 표시 (애니메이션 등)
            StartingGame,           // 게임 시작 준비 완료 (양쪽 ready)
            Playing,                // 게임 플레이 중 (자신의 턴 또는 상대방 턴)
            OpponentResigned,       // 상대방이 기권함
            GameEndedByResign,      // 기권으로 인해 게임 종료
            OpponentDisconnectedGameplay, // 게임 플레이 도중 상대방 연결 끊김
            GameEnded,              // 게임 종료 (일반적인 종료)
            RoomCreatedForRematch,  // 재대국을 위한 방 생성 완료
            Disconnected,           // 서버 연결 끊김
            GameDraw,               // 게임 무승부
            RequestingDraw,         // 무승부 요청 중 (클라이언트 내부 상태)
            OpponentRequestedDraw,  // 상대방이 무승부를 요청함
            AcceptingDraw,          // 무승부 수락 중 (클라이언트 내부 상태)
            RejectingDraw,          // 무승부 거절 중 (클라이언트 내부 상태)
            DrawRejected,           // 무승부 거절됨
            RequestingRematch,      // 재대국 요청 중 (클라이언트 내부 상태)
            RematchRequested,       // 상대방이 재대국을 요청함
            AcceptingRematch,       // 재대국 수락 중 (클라이언트 내부 상태)
            RejectingRematch,       // 재대국 거절 중 (클라이언트 내부 상태)
            RematchRejected,        // 재대국 거절됨
            LeavingRoom,            // 방 나가기 요청 중 (클라이언트 내부 상태)
            ExitRoom,               // 방 나가기 완료
            MatchFailed             // 매칭 실패 (코인 부족 등)
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