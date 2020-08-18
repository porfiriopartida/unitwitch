using System;
using LopapaGames;
using UnityEngine;
using UnityEngine.UI;

public class DemoCheckConfiguration : MonoBehaviour
{
    public TwitchConnection TwitchConnection;

    public Text Text;
    
    // Start is called before the first frame update
    void Start()
    {
        string msg = "";
        bool hasErrors = false;
        if (String.IsNullOrEmpty(this.TwitchConnection.OAuth))
        {
            msg += "OAuth is missing. Go generate one at https://twitchapps.com/tmi/";
            hasErrors = true;
        }
        if (String.IsNullOrEmpty(this.TwitchConnection.BotName))
        {
            msg += "\nBotName is missing. This must be the user that belongs to the generated OAuth key.";
            hasErrors = true;
        }
        if (String.IsNullOrEmpty(this.TwitchConnection.ChannelName))
        {
            msg += "\nChannelName is missing. This can be any channel you want to parse the messages.";
            hasErrors = true;
        }

        if (hasErrors)
        {
            Text.text = msg;
        }
        else
        {
            Text.text = "Go to " + this.TwitchConnection.ChannelName + " and use the commands: \n!show | !hide";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
