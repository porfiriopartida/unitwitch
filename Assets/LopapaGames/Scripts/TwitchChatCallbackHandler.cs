using UnityEngine;

namespace LopapaGames
{
    
    public abstract class TwitchChatCallbackHandler : MonoBehaviour
    {
        public virtual void Reconnect()
        {
            Debug.Log("Reconnect");
        }

        public virtual void OnMessage(string msg)
        { 
            Debug.Log("OnMessage: " + msg);
        }

        public virtual void OnError(string errorMsg)
        {
            Debug.Log("OnError: " + errorMsg);
        }

        public virtual void OnConnect(string s)
        {
            Debug.Log("Connection open: " + s);
        }

        public virtual void OnClose(string reason)
        {   
            Debug.Log("Connection closed: " + reason);
        }
    }

}