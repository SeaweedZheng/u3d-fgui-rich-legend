using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameHistoryTemplateSlot1 布局
/// </summary>
/// <remarks>
/// * 上中下布局，上是滚轮结果，中是键值对，下是自定义内容。
/// </remarks>
public class TemplateDataGameRecordSlot1  // 类代表UI布局面板
{
    public string deckRowCol = "1,2,3,4,5#1,2,3,4,5#1,2,3,4,5";

    public string gameUid; //gameid-随机数-时间戳-第几局-触发局

    public string turnType = "spin"; //"spin", "free_spin"  "bonus_minigame"

    public long creatAt;
    public int totalBet;
    public int creditBefore;
    public int creditAfter;
    public int baseGameWinCredit;
    public int jackpotWinCredit;  
    public string jackpotType;
    public int bonusGameWinCredit;
    public string bonusType;


    /// <summary> 总免费游戏次数 </summary>
    public int freeSpinTotalCount = 0;
    /// <summary> 当前免费游戏第几局 </summary>
    public int freeSpinCurNumber = 0;
    /// <summary> 没费游戏额外增加次数 </summary>
    public int freeSpinAddCount = 0;

    public bool isFreeSpin => turnType == "free_spin";
    public bool isTriggerFreeSpin => !isFreeSpin && freeSpinTotalCount > 0 && freeSpinCurNumber==0;
    public bool isEndFreeSpin => isFreeSpin && freeSpinTotalCount == freeSpinCurNumber;

    public string detail  = "seat:{0} reward:{1}##seat:{0} reward:{1}"; //多语言模板
    public string args = "1,50##2,100";  // 数值
}
