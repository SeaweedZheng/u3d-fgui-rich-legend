using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// template_name: GameHistoryUITemplateCoinPusher1
// template_data: {}


/// <summary>
/// 通用游戏记录模板 - 推币机
/// </summary>
public class TemplateDataGameRecordCoinPusher1
{
    //public int gameId = -1;

    public string deckRowCol = "1,2,3,4,5#1,2,3,4,5#1,2,3,4,5";

    public string gameUid; //gameid-随机数-时间戳-第几局-触发局

    public string turnType = "spin"; //"spin", "free_spin"  "bonus_minigame"

    public long creatAt;
    /// <summary> 1分多少币 </summary>
    public int coinPerCredit;
    /// <summary> 1币多少分 </summary>
    public int creditPerCoin;
    public int baseGameWinCoins;
    public int jackpotWinCoins;
    public string jackpotType;
    public int bonusGameWinCoins;
    public string bonusType;


    /// <summary> 总免费游戏次数 </summary>
    public int freeSpinTotalCount = 0;
    /// <summary> 当前免费游戏第几局 </summary>
    public int freeSpinCurNumber = 0;
    /// <summary> 没费游戏额外增加次数 </summary>
    public int freeSpinAddCount = 0;


    public bool isFreeSpin => turnType == "free_spin";
    public bool isTriggerFreeSpin => !isFreeSpin && freeSpinTotalCount > 0 && freeSpinCurNumber == 0;
    public bool isEndFreeSpin => isFreeSpin && freeSpinTotalCount == freeSpinCurNumber;

    public string detail = ""; // "seat:{0} reward:{1}##seat:{0} reward:{1}"; //多语言模板
    public string args = "";  //  "1,50##2,100";  // 数值
}