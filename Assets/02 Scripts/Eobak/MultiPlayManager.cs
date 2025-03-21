using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Eobak;
using UnityEngine;
using Newtonsoft.Json;
using SocketIOClient;



public class MultiPlayManager : IDisposable
{
    private SocketIOUnity _socket;
    private event Action<Constants.MultiplayManagerState, string> _onMultiplayStateChanged;
    
    public void Dispose()
    {
        
    }
}
