using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ChatSystem : NetworkBehaviour {
    public MessageBox chatBox;

    //This method should be called when the player hits Enter while typing
    public void SendMessage(string username, string message){
        if(message != ""){
            RPC_ChatMessage($"<b>{username}:</b> {message}");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChatMessage(string message, RpcInfo info = default){
        if(chatBox != null){
            chatBox.OnMessageReceived(message);
        }
    }


}
