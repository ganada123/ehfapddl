using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Eobak;
using UnityEngine;
using Newtonsoft.Json;
using SocketIOClient;

public class StonePlacedData
{
    [JsonProperty("row")]
    public int row { get; set; }
    [JsonProperty("col")]
    public int col { get; set; }
    // 게임 중 상대방이 돌을 놓았을 때 서버에서 'stonePlaced' 이벤트를 통해 착수된 돌의 행(row)과 열(col) 정보를 받아와 게임 화면에 반영할 때 사용됩니다.
}

public class RoomIdData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
    // 서버에서 방이 생성되었거나 재대국을 위해 새로운 방이 생성되었을 때, 해당 방의 고유 ID(roomId)를 받아올 때 사용됩니다.
}

public class SetColorData
{
    [JsonProperty("opponentNickname")]
    public string opponentNickname { get; set; }
    [JsonProperty("myColor")]
    public string myColor { get; set; }
    [JsonProperty("opponentColor")]
    public string opponentColor { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
    // 매칭 후 서버에서 'setColor' 이벤트를 통해 상대방 닉네임(opponentNickname), 자신의 색깔(myColor), 적 색(opponentColor) 그리고 방 ID(roomId)를 받아오고
    // 초기 방설정 시 사용됩니다.
}

public class ResignData
{
    [JsonProperty("opponentNickname")]
    public string opponentNickname { get; set; }
    // 자신이 'resignGame' 이벤트를 통해 기권했을 때,
    // 서버에서 'resigned' 이벤트를 통해 상대방의 닉네임(opponentNickname)을 받아와
    // 게임 종료 화면 등에 표시할 때 사용됩니다.
}

public class OpoonentResignData
{
    [JsonProperty("winnerNickname")]
    public string winnerNickname { get; set; }
    [JsonProperty("loserNickname")]
    public string loserNickname { get; set; }
    // 상대방이 기권하여 게임이 종료되었을 때,
    // 서버에서 'opponentResigned' 이벤트를 통해 승자(winnerNickname)와 패자(loserNickname)의 닉네임을 받아와
    // 게임 종료 화면 등에 표시할 때 사용됩니다.
}

public class DrawRequestData
{
    [JsonProperty("requesterNickname")]
    public string requesterNickname { get; set; }
    // 서버에서 'opponentRequestedDraw' 이벤트를 통해 상대방이 무승부를 요청했을 때,
    // 요청한 사람의 닉네임(requesterNickname)을 받아와 무승부 수락/거절 UI를 표시할 때 사용됩니다.
}

public class DrawRejectData
{
    [JsonProperty("rejecterNickname")]
    public string rejecterNickname { get; set; }
    // 상대방이 자신의 무승부 요청을 거절했을 때,
    // 서버에서 'drawRejected' 이벤트를 통해 거절한 사람의 닉네임(rejecterNickname)을 받아와
    // 사용자에게 알릴 때 사용됩니다.
}

public class RematchRequestData
{
    [JsonProperty("requesterNickname")]
    public string requesterNickname { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
    // 서버에서 'rematchRequested' 이벤트를 통해 상대방이 재대국을 요청했을 때,
    // 요청한 사람의 닉네임(requesterNickname)과 방 ID(roomId)를 받아와 재대국 수락/거절 UI를 표시할 때 사용됩니다.
}

public class RematchRejectData
{
    [JsonProperty("rejecterNickname")]
    public string rejecterNickname { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
    // 서버에서 'rematchRejected' 이벤트를 통해 상대방이 재대국 요청을 거절했을 때,
    // 거절한 사람의 닉네임(rejecterNickname)과 방 ID(roomId)를 받아와 사용자에게 알릴 때 사용됩니다.
}

public class DisconnectedData
{
    [JsonProperty("winner")]
    public string winner { get; set; }
    // 서버에서 'opponentDisconnectedGameplay' 이벤트를 통해 게임 도중 상대방 연결이 끊어졌을 때,
    // 승자(winner)의 닉네임을 받아올 때 사용됩니다.
}

public class MultiplayManager : IDisposable
{
    private SocketIOUnity _socket;
    /// <summary>
    /// TODO: 오류수정을 위한 Constants.MultiplayManagerState -> _02_Scripts.Eobak.Constants.MultiplayManagerState로 수정했습니다.
    /// 이후에 수정 부탁드립니다.
    /// </summary>
    private event Action<_02_Scripts.Eobak.Constants.MultiplayManagerState, string> _onMultiplayStateChanged;

    public Action<RoomIdData> OnRoomCreatedForMatch;                // 매치를 위한 방 생성 완료 이벤트
    public Action<SetColorData> OnSetColor;                         // 색을 받아올 때 이벤트
    public Action OnShowColor;                                      // 색을 화면에 표시하거나 룰렛 연출 시작할 때 이벤트
    public Action<StonePlacedData> OnStonePlaced;                   // 상대가 착수했을 때 착수 데이터를 받아오는 이벤트
    public Action<OpoonentResignData> OnOpponentResigned;           // 상대방이 기권했을 때 이벤트
    public Action<ResignData> OnResigned;                           // 자신이 기권했을 때 이벤트
    public Action<DrawRequestData> OnOpponentRequestedDraw;         // 상대방이 무승부를 요청했을 때 이벤트
    public Action OnGameDraw;                                       // 게임이 무승부로 종료되었을 때 이벤트
    public Action<DrawRejectData> OnDrawRejected;                   // 상대방이 무승부 요청을 거절했을 때 이벤트
    public Action<RematchRequestData> OnRematchRequested;           // 상대방이 재대국을 요청했을 때 이벤트
    public Action<RematchRejectData> OnRematchRejected;             // 상대방이 재대국 요청을 거절했을 때 이벤트
    public Action<DisconnectedData> OnOpponentDisconnectedGameplay; // 게임 플레이 도중 상대방 연결이 끊어졌을 때 이벤트
    public Action OnOpponentDisconnectedBeforeStart;                // 게임 시작 전에 상대방 연결이 끊어졌을 때 이벤트
    public Action OnExitRoom;                                       // 방을 나갔을 때 이벤트
    public Action OnGameEndedByResign;                              // 기권으로 인해 게임이 종료되었을 때 이벤트
    public Action<RoomIdData> OnRoomCreatedForRematch;              // 재대국을 위한 방 생성 완료 이벤트
    public Action OnWaitingForMatch;                                // 매칭 대기 중 이벤트
    public Action OnMatchWithAI;                                    // AI와 매칭되었을 때 이벤트
    public Action OnMatchmakingCancelled;                           // 매칭이 취소되었을 때 이벤트

    /// <summary>
    /// TODO: 오류수정을 위한 Constants.MultiplayManagerState -> _02_Scripts.Eobak.Constants.MultiplayManagerState로 수정했습니다.
    /// 이후에 수정 부탁드립니다.
    /// </summary>
    public MultiplayManager(Action<_02_Scripts.Eobak.Constants.MultiplayManagerState, string> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;

        var uri = new Uri(Constants.GameServerURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        _socket.On("roomCreatedForMatch", RoomCreatedForMatch);
        _socket.On("setColor", SetColor);
        _socket.On("showColor", ShowColor);
        _socket.On("stonePlaced", StonePlaced);
        _socket.On("opponentResigned", OpponentResigned);
        _socket.On("resigned", Resigned);
        _socket.On("opponentRequestedDraw", OpponentRequestedDraw);
        _socket.On("gameDraw", GameDraw);
        _socket.On("drawRejected", DrawRejected);
        _socket.On("rematchRequested", RematchRequested);
        _socket.On("rematchRejected", RematchRejected);
        _socket.On("opponentDisconnectedGameplay", OpponentDisconnectedGameplay);
        _socket.On("opponentDisconnectedBeforeStart", OpponentDisconnectedBeforeStart);
        _socket.On("exitRoom", ExitRoom);
        _socket.On("gameEndedByResign", GameEndedByResign);
        _socket.On("roomCreatedForRematch", RoomCreatedForRematch);
        _socket.On("waitingForMatch", WaitingForMatch);
        _socket.On("matchWithAI", MatchWithAI);
        _socket.On("matchmakingCancelled", MatchmakingCancelled);

        _socket.Connect();
    }

    // 매치를 위한 방 생성 완료, param: (roomID)
    private void RoomCreatedForMatch(SocketIOResponse response)
    {
        var data = response.GetValue<RoomIdData>();
        OnRoomCreatedForMatch?.Invoke(data);
    }
    
    // TODO: 돌 색 변수값 정해서 서버랑 조율, 흑돌(STONE_BLACK: "Black"), 백돌(STONE_WHITE: "White")
    private void SetColor(SocketIOResponse response) // 만든 의도: 자기색 설정하고 상대방의 색이랑 이름도 설정해라
    {
        var data = response.GetValue<SetColorData>();
        OnSetColor?.Invoke(data);
    }
    
    // 색을 각 클라이언트에 화면에 표시 or 룰렛 연출 시작 신호
    private void ShowColor(SocketIOResponse response) // 만든 의도: 내 색과 상대 색이 정해지는 시점을 정의 하기 위해
    {
        OnShowColor?.Invoke();
    }

    // 상대가 착수한 돌의 정보를 받아오는 이벤트
    private void StonePlaced(SocketIOResponse response)
    {
        var data = response.GetValue<StonePlacedData>();
        OnStonePlaced?.Invoke(data);
    }

    private void OpponentResigned(SocketIOResponse response)
    {
        var data = response.GetValue<OpoonentResignData>();
        OnOpponentResigned?.Invoke(data);
    }

    private void Resigned(SocketIOResponse response)
    {
        var data = response.GetValue<ResignData>();
        OnResigned?.Invoke(data);
    }

    private void OpponentRequestedDraw(SocketIOResponse response)
    {
        var data = response.GetValue<DrawRequestData>();
        OnOpponentRequestedDraw?.Invoke(data);
    }

    private void GameDraw(SocketIOResponse response)
    {
        OnGameDraw?.Invoke();
    }

    private void DrawRejected(SocketIOResponse response)
    {
        var data = response.GetValue<DrawRejectData>();
        OnDrawRejected?.Invoke(data);
    }

    private void RematchRequested(SocketIOResponse response)
    {
        var data = response.GetValue<RematchRequestData>();
        OnRematchRequested?.Invoke(data);
    }

    private void RematchRejected(SocketIOResponse response)
    {
        var data = response.GetValue<RematchRejectData>();
        OnRematchRejected?.Invoke(data);
    }

    private void OpponentDisconnectedGameplay(SocketIOResponse response)
    {
        var data = response.GetValue<DisconnectedData>();
        OnOpponentDisconnectedGameplay?.Invoke(data);
    }

    private void OpponentDisconnectedBeforeStart(SocketIOResponse response)
    {
        OnOpponentDisconnectedBeforeStart?.Invoke();
    }
    
    private void ExitRoom(SocketIOResponse response)
    {
        OnExitRoom?.Invoke();
    }

    private void GameEndedByResign(SocketIOResponse response)
    {
        OnGameEndedByResign?.Invoke();
    }

    private void RoomCreatedForRematch(SocketIOResponse response)
    {
        var data = response.GetValue<RoomIdData>();
        OnRoomCreatedForRematch?.Invoke(data);
    }

    private void WaitingForMatch(SocketIOResponse response) { OnWaitingForMatch?.Invoke(); }
    private void MatchWithAI(SocketIOResponse response) { OnMatchWithAI?.Invoke(); }
    private void MatchmakingCancelled(SocketIOResponse response) { OnMatchmakingCancelled?.Invoke(); } // TODO:※ 매칭 취소 할 시 Dispose 해주기

    // 게임이 시작할 때 넣어주세요.
    // 서버에선 플레이어가 연결 끊길 때 게임 시작 여부를 판단하는 기능을 넣어뒀어요. <- 이점 유의해서 넣어주세요.
    public void StartGame()
    {
        _socket.Emit("startGame");
    }
    
    // TODO: 방을 나갈 때 Dispose 해주기
    public void LeaveRoom(string roomId)
    {
        _socket.Emit("leaveRoom", new { roomId });
    }

    // TODO: 필요시 서버 matching.js 파일 matchingTime 수정, 매칭 제한 시간: 60초:60000
    // TODO: 매칭을 시작할 때 코인 개수가 100 이상인가 체크하기
    // 매칭 시작 버튼 눌렀을 때 코인 체크하고 통과 시켜주세요.
    // 서버에서는 매칭 잡혔을 때 코인 차감하게끔 코드 짜놨습니다.
    public void JoinMatchQueue(string email, int rank, string nickname)
    {
        _socket.Emit("joinMatchQueue", new { email, rank, nickname });
    }
    
    public void CancelMatchmaking()
    {
        _socket.Emit("cancelMatchmaking");
    }

    // 착수된 돌의 정보를 상대에게 보내는 이벤트
    public void PlaceStone(int row, int col)
    {
        _socket.Emit("placeStone", new { row, col });
    }

    public void GameEnded(string winner, int result)
    {
        _socket.Emit("gameEnded", new { winner, result });
    }

    public void ResignGame()
    {
        _socket.Emit("resignGame");
    }

    public void RequestDraw()
    {
        _socket.Emit("requestDraw");
    }

    public void AcceptDraw()
    {
        _socket.Emit("acceptDraw");
    }

    public void RejectDraw()
    {
        _socket.Emit("rejectDraw");
    }

    // TODO: 재대국을 신청한 쪽이 코인 개수가 100 이상인가 체크하기
    // 재대국 신청 전에 코인(재대국 신청자) 체크하고 통과 시켜주세요.
    // 서버에서는 재대국 수락시 재대국 신청자에게만 코인이 차감되도록 해놨습니다.
    // 코인 이벤트 함수는 따로 만들지 않았습니다.
    public void RequestRematch()
    {
        _socket.Emit("requestRematch");
    }

    public void AcceptRematch(string roomId)
    {
        _socket.Emit("acceptRematch", new { roomId });
    }

    public void RejectRematch(string roomId)
    {
        _socket.Emit("rejectRematch", new { roomId });
    }

    // TODO: 클라이언트에서 Application.Quit 했을 때 소켓을 Dispose 해주기
    // TODO: 방을 나갈 때 LeaveRoom()을 불러오고 Dispose 해주기
    // 서버에서 연결이 끊겼을 때 기보, 랭크, 승, 패 처리 및 저장해놨습니다. gamePlay.js 'disconnect' 이벤트 참고
    public void Dispose()
    {
        if (_socket != null)
        {
            _socket.Disconnect();
            _socket.Dispose();
            _socket = null;
        }
    }
}