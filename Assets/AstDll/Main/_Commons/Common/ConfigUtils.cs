using GameMaker;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class ConfigUtils
{


    /// <summary> 当前游戏主题 </summary>
    //public string curGameName = "";


    /// <summary> 游戏id </summary>
    public static int curGameId => MainModel.Instance.gameID;


    /// <summary> 多语言文件名（不带后缀）</summary>
    //public static readonly string[] i18nLoadFile = new string[] { "i18n_po152", "i18n_console001" };

    /*
    /// <summary> 多语言文件名（不带后缀）</summary>
    public static readonly string[] i18nLoadFile = new string[] {
        "Assets/AstBundle/LuBan/GenerateDatas/bytes/i18n_po152.json",
        "Assets/AstBundle/LuBan/GenerateDatas/bytes/i18n_console001.json"
    };*/

    /// <summary> 游戏配置文件路劲 </summary>
    public static string GetGameInfoURL(int gameId) => $"Assets/AstBundle/_Datas/ABs/Games/G{gameId}/game_info.json";
    //$"Assets/AstBundle/_Commons/Game Maker/ABs/Datas/{gameId}/game_info_g{gameId}.json";

    /// <summary> 游戏图片路劲 </summary>
    public static string GetGameAvatarURL(int gameId) => $"Assets/AstBackup/Lobbys/Game Info/G{gameId}/Game Avatar/Game Avatar G{gameId}.png";
    //$"Assets/AstBackup/Games/G{gameId}/Game Avatar/Game Avatar G{gameId}.png";
    // $"Assets/AstBundle/_Commons/Game Maker/ABs/{gameId}/Game Icon/Game Avatar {gameId}.png";

    

    public static string GetErrorCode() => "Assets/AstBundle/_Datas/ABs/Error Code/error_code.json";

    /// <summary> 游戏GM文件路劲 </summary>
    public static string GetGameGMURL(int gameId) =>
        $"Assets/AstBundle/_Datas/ABs/Games/G{gameId}/tmg_mock_gm.json";
        //$"Assets/AstBundle/_Commons/Game Maker/ABs/{gameId}/Game Icon/Game Avatar {gameId}.png";
    
    /// <summary> 游戏信息 </summary>      
    public static string curGameGMURL => GetGameGMURL(curGameId);

    /// <summary> 游戏信息 </summary>
    public static string curGameInfoURL => GetGameInfoURL(curGameId);

    /// <summary> 游戏头像 </summary>
    public static string curGameAvatarURL => GetGameAvatarURL(curGameId);

    public static string GetGameAvararWebUrl(int gameId) => PathHelper.GetAssetBackupWEBURL(GetGameAvatarURL(gameId));
        //PathHelper.GetAstBackupWEBURL($"Assets/AstBackup/Games/G{gameId}/Game Avatar/Game Avatar {gameId}.png");

    public static string curGameAvararWebUrl => GetGameAvararWebUrl(curGameId);

}

// 拉霸机的参数
public partial class ConfigUtils
{
    public static string GetSlotSymbolAssetPth(int gameId, int symbolNumber) => $"Assets/AstBackup/Consoles/Game Info/G{gameId}/Symbols/symbol{symbolNumber}.png";
}