using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby01
{

    public enum SoundKey
    {
        /// <summary> 大厅背景音乐 </summary>
        LobbyBG1,
        /// <summary> 大厅背景音乐 </summary>
        LobbyBG2,
        /// <summary> 进入游戏 </summary>
        EnterGame,
    }
    public class SoundModel : MonoSingleton<SoundModel>
    {
        public Dictionary<SoundKey, GSHandler> gsHandlers = new Dictionary<SoundKey, GSHandler>
        {


            [SoundKey.LobbyBG1] = new GSHandler()
            {
                assetPath = "Assets/AstBundle/Lobbys/Lobby01/Sounds/BGM_Lobby.wav",
                outputType = GSOutType.Music,
                loop = true,
            },
            [SoundKey.LobbyBG2] = new GSHandler()
            {
                assetPath = "Assets/AstBundle/Lobbys/Lobby01/Sounds/BGM_Lobby(Slots3).wav",
                outputType = GSOutType.Music,
                loop = true,
            },
            [SoundKey.EnterGame] = new GSHandler()
            {
                assetPath = "Assets/AstBundle/Lobbys/Lobby01/Sounds/UI_Game_Select.wav",
            }
        };
    }
}