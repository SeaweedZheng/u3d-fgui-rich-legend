using GameMaker;
using Newtonsoft.Json;
using PusherEmperorsRein;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils 
{
    public static void ParseGameInfo(string gameInfo)
    {
        GameConfigRoot config = JsonConvert.DeserializeObject<GameConfigRoot>(gameInfo);
        if (config?.SymbolPaytable == null)
        {
            DebugUtils.LogError("解析symbol_paytable失败，数据为空");
            return;
        }

        MainModel.Instance.gameID = config.GameId;
        MainModel.Instance.gameName = config.GameName;
        MainModel.Instance.displayName = config.DisplayName;
        foreach (var item in config.WinLevelMultiple)
        {
            string winKey = item.Key;
            long winValue = item.Value;
            MainModel.Instance.contentMD.winLevelMultiple.Add(new WinMultiple(winKey, winValue));
        }

        ContentModel.Instance.payTableSymbolWin = new List<PayTableSymbolInfo>();
        foreach (var kvp in config.SymbolPaytable)
        {
            string symbolKey = kvp.Key; // 如 "s0"、"s1"、"s2"
            var jsonData1 = kvp.Value; // 对应x3、x4、x5的数据

            // 1. 从symbolKey中提取索引（如"s0" → 0，"s1" → 1）
            if (int.TryParse(symbolKey.Replace("s", ""), out int index))
            {
                // 2. 检查索引是否在列表有效范围内
                if (index >= 0)
                {
                    PayTableSymbolInfo item = new PayTableSymbolInfo()
                    {
                        symbol = index,
                        x3 = jsonData1.x3,
                        x4 = jsonData1.x4,
                        x5 = jsonData1.x5,
                    };
                    ContentModel.Instance.payTableSymbolWin.Add(item);
                }
            }
            else
            {
                DebugUtils.LogError($"无效的符号键：{symbolKey}，无法解析索引");
            }
        }

        // DebugUtils.LogError($"payTableSymbolWin ({ContentModel.Instance.name}) = ：{JsonConvert.SerializeObject(ContentModel.Instance.payTableSymbolWin)}");

        ContentModel.Instance.payLines = new List<List<int>>() { };
        foreach (var item in config.pay_lines)
        {
            ContentModel.Instance.payLines.Add(item);
        }

        ///payTableController.OnPropertyChangeTotalBet();
    }
}
