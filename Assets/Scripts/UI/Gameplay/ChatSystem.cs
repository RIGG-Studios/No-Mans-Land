using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class ChatSystem : NetworkBehaviour {
    [SerializeField] private MessageBox chatBox;
    [SerializeField] private GameObject chatUI;
    [SerializeField] private TMP_InputField chatInput;

    bool playerIsTyping = false;
    string messageToSend = "";
    string name = "Player";

    void Awake(){
        name = this.GetComponent<Player>().PlayerName.ToString();
    }

    void Update(){
        if(playerIsTyping){
            updateMessage(messageToSend);
        }
    }

    private void updateMessage(string message){
        foreach(char c in Input.inputString){
            if(c == '\b'){
                int length = message.Length;
                if(message.Length >= 1){
                    message = message.Substring(0, message.Length - 1);
                }

            } else if(c == '\n' || c == '\r'){
                if(message != ""){
                    SendMessage(name, message);
                    message = "";
                }
            } else {
                message += c;
            }
        }
    }

    public void SendMessage(string username, string message){
        RPC_ChatMessage($"<b>{username}:</b> {message}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_ChatMessage(string message, RpcInfo info = default){
        if(chatBox != null){
            chatBox.OnMessageReceived(message);
        }
    }

    public void ProcessInput(NetworkInputData input){
        if(input.Buttons.IsSet(PlayerButtons.ToggleChat)){
            if(Object.HasInputAuthority){
                if(!playerIsTyping){
                    chatUI.SetActive(true);
                    playerIsTyping = true;
                } else {
                    playerIsTyping = false;
                }
            }
            if(Object.HasStateAuthority){
                //if chat box is open, freeze player movement. If not, turn movement back on.
                if(playerIsTyping){
                    //stop movement; all inputs should now be redirected to the Input Field (chatInput);
                } else {
                    //turn movement back on
                }
            }
        }
    }
}