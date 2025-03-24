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
}

public class RoomIdData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class UserIdData
{
    [JsonProperty("userId")]
    public string userId { get; set; }
}

public class OpponentInfoData
{
    [JsonProperty("opponentNickname")]
    public string opponentNickname { get; set; }
    [JsonProperty("myColor")]
    public string myColor { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class ResignData
{
    [JsonProperty("opponentNickname")]
    public string opponentNickname { get; set; }
}

public class OpoonentResignData
{
    [JsonProperty("winnerNickname")]
    public string winnerNickname { get; set; }
    [JsonProperty("loserNickname")]
    public string loserNickname { get; set; }
}

public class DrawRequestData
{
    [JsonProperty("requesterNickname")]
    public string requesterNickname { get; set; }
}

public class DrawRejectData
{
    [JsonProperty("rejecterNickname")]
    public string rejecterNickname { get; set; }
}

public class RematchRequestData
{
    [JsonProperty("requesterNickname")]
    public string requesterNickname { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class RematchRejectData
{
    [JsonProperty("rejecterNickname")]
    public string rejecterNickname { get; set; }
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class DisconnectedData
{
    [JsonProperty("winner")]
    public string winner { get; set; }
}

public class MultiplayManager : IDisposable
{
    private SocketIOUnity _socket;
    private event Action<Constants.MultiplayManagerState, string> _onMultiplayStateChanged;
    
    public Action<RoomIdData> OnRoomCreatedForMatch;
    public Action<OpponentInfoData> OnSendInfo;
    public Action OnShowColor;
    public Action OnStartGameEvent;
    public Action<StonePlacedData> OnStonePlaced;
    public Action<OpoonentResignData> OnOpponentResigned;
    public Action<ResignData> OnResigned;
    public Action<DrawRequestData> OnOpponentRequestedDraw;
    public Action OnGameDraw;
    public Action<DrawRejectData> OnDrawRejected;
    public Action<RematchRequestData> OnRematchRequested;
    public Action<RematchRejectData> OnRematchRejected;
    public Action<DisconnectedData> OnOpponentDisconnectedGameplay;
    public Action OnOpponentDisconnectedBeforeStart;
    public Action OnExitRoomEvent;
    public Action OnGameEndedByResign;
    public Action<RoomIdData> OnRoomCreatedForRematch;
    public Action OnWaitingForMatch;
    public Action OnMatchWithAI;
    public Action OnMatchmakingCancelled;

    public MultiplayManager(Action<Constants.MultiplayManagerState, string> onMultiplayStateChanged)
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
        _socket.On("startGame", StartGame);
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
    
    // 상대 플레이어(opponentNickname), 자기 색깔(myColor), 방 고유 ID(roomId)를 받아오는 이벤트 
    private void SetColor(SocketIOResponse response) // 만든 의도: 색 설정하고 상대방의 색도 설정해라
    {
        var data = response.GetValue<OpponentInfoData>();
        OnSendInfo?.Invoke(data);
    }

    // 색을 각 클라이언트에 화면에 표시하거나 룰렛 연출 같은 게 있으면 하라는 이벤트
    private void ShowColor(SocketIOResponse response) // 만든 의도: 내 색과 상대 색이 정해지는 시점을 정의 하기 위해
    {
        OnShowColor?.Invoke();
    }

    // 서버쪽 startGameTime 이후 실행 TODO: 필요시 startGameTime 조정
    private void StartGame(SocketIOResponse response)
    {
        OnStartGameEvent?.Invoke();
    }

    // 착수된 돌의 정보를 받아오는 이벤트
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

    // TODO:※ 방나갈 때 Dispose 해주기
    private void ExitRoom(SocketIOResponse response)
    {
        OnExitRoomEvent?.Invoke();
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
    
    public void LeaveRoom(string roomId)
    {
        _socket.Emit("leaveRoom");
    }

    public void JoinMatchQueue(string email, int rank, string nickname)
    {
        _socket.Emit("joinMatchQueue", new { email, rank, nickname });
    }

    public void CancelMatchmaking()
    {
        _socket.Emit("cancelMatchmaking");
    }

    // TODO: 돌 색깔 정해서 서버랑 조율, 흑돌(STONE_BLACK: 1), 백돌(STONE_WHITE: 2)
    public void SelectColor(int color)
    {
        _socket.Emit("selectColor", color);
    }

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