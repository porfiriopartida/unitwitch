# UniTwitch

Thanks for using UniTwitch.

This project relies on using WebSockets, a WebSocketSharp library and a Thread dispatcher library.

UnityMainThreadDispatcher: Author: Pim de Witte (pimdewitte.com) and contributors
https://github.com/PimDeWitte/UnityMainThreadDispatcher

WebSocketSharp: [sta/websocket-sharp](https://github.com/sta/websocket-sharp).

## How to Setup:

1. Import the project.
2. Create an empty object to be your Twitch Handler (from now on "TwitchHandler").
3. Add the TwitchChatHandler component to your TwitchHandler object
    1. This will add a UnityMainThreadDispatcher component, used to handle async commands.
    2. The TwitchChatHandler will have 
        1. Listen to whispers: Allow direct messages to your bot to be parsed.
        2. Listen To Public Chat: Allows public chat messages to be parsed.
        3. Enable Help: Allows the usage of "!help" (configurable) and will LOG this (future state is to send it to such chat.)
        4. Configuration: The twitch connection configuration
        5. Twitch Chat Callback Handler: A MonoBehaviour Component that handles IRC methods
            1. Reconnect
            2. OnMessage
            3: OnError
            4. OnConnect
            5. OnClose
        6. Commands (Array): List of configured commands. Size is 0 by default.
        7. Help Command: The key message that users must send to get the help command to get executed if enabled (see III). Default is !help.
 4. Create a new asset through Assets>Create>LopapaGames>Twitch>TwitchConfiguration
    1. OAuth: the Chat OAuth key, generated through https://twitchapps.com/tmi/ this is NOT the application OAuth key (not even sure if that one works too, e. g. oauth:01a1abc1abc0a0abc0abc01ab0abc0 )
    2. Bot Name: the username that the script will behave as (matching above's account, e. g. porfiriopartida_bot)
    3. Channel Name: The channel to scan from. e. g. porfiriopartida
 5. Create a new empty GameObject and add the DemoTwitchCallback component to it (name it CallbacksObject).
 6. Hover the TwitchHandler GameObject on your scene
    1. Attach the new configuration in the Configuration field (created on step 4)
    2. Attach the CallbacksObject to the handler's Twitch Chat Callback Handler (for custom callback, see below)
        1. If you want to customize your IRC callbacks, create a new Script that inherits from TwitchChatCallbackHandler (which inherits from MonoBehaviour), for an example, see: Assets/LopapaGames/Demo/Scripts/DemoTwitchCallback.cs
        2. Replace the component added in step #5 using your own script instead.
 
 ## Meat and potatoes:
 Congratulations, you are all setup. You should be able to run the scene without errors, it will connect and listen for messages.
 You can send messages and see the Console receive those messages.
 
 There are some extra steps to fully customize your scene handling.
 
 1. Create a new Scriptable Object Script that inherits from ACommand, see Assets/LopapaGames/Demo/ScriptableObjects/Scripts/SampleCommand.cs
    1. Make sure to have it as with the CreateAssetMenu handler so you can create it from the menu, example:
       ```[CreateAssetMenu(fileName = "SampleCommand", menuName = "LopapaGames/Twitch/Sample/Command")]```
    2. Your Execute implementation will receive the user name that executed the command as well as the full message sent (including the command)
 2. Create a ScriptableObject through the Assets > and the path defined in your menuName, example: 
    1. Assets > Create > LopapaGames > Twitch > Sample > Command
    2. It will create a new object given the name you used as fileName, e. g. SampleCommand
 3. Hover your new scriptable object in your assets
    1. You will have at least the Command Key, which is the command to be triggered: e. g. !hide
    2. If you added more public serializable fields, they will also show there, e.g. the Demo Sample Command will also have a Set Active boolean to be selected.
 4. Hover the TwitchHandler game object in the scene.
    1. In the Commands Array, put a new Size, from 0 to 1 (you can add as many as you have created above.)
        1. A new Element 0 will be displayed if you added Size 1. This accepts an ACommand script (created in step 2)
    2. Drag the scriptable object on top of the first element of the Commands list (Element 0).
    
    
 ## You are done!
  
 Hit the play button and go to the scanned chat. Send !hide (assuming this was your key command) and your Scriptable Object's Execute will be invoked (enqueued to be invoked actually)
 
 In our SampleCommand example, it will execute a Singleton's function using the Set Active attribute.
 
 ```DemoUIManager.Instance.textElement.SetActive(setActive);```
 
 You can pass the message to be parsed and do something more interesting.