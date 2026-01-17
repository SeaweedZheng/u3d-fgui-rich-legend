using FairyGUI;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class SlotMachineConfig 
{

    public SlotMachineConfig(string configStr)
    {
        config = JSONNode.Parse(configStr);
    }

    JSONNode config;

    string curReelSetting = "regular";
    string curWinEffectSetting = "default";



    public int Row => (int)config["custom"]["row"];
    public int Column => (int)config["custom"]["column"];
    public float ReelMaxOffsetY => (int)config["custom"]["symbol_height"] * Row;
    public int DeckUpStartIndex => 1;
    public int DeckUpEndIndex => (int)config["custom"]["row"];
    public int DeckDownStartIndex => DeckUpEndIndex + 1;
    public int DeckDownEndIndex => 2 * DeckUpEndIndex;

    public int SymbolCount => config["custom"]["symbol_number"].Count;

     List<int> _symbolNumbers = null;
    public List<int> SymbolNumbers
    {
        get
        {
            if (_symbolNumbers == null)
            {
                _symbolNumbers = new List<int>();
                foreach (JSONNode node in config["custom"]["symbol_number"])
                {
                    _symbolNumbers.Add((int)node);
                }
            }
            return _symbolNumbers;
        }
    }

    public string BorderEffect => (string)config["custom"]["border_effect"];

    JSONNode GetReelSettingValue(int reelIdx, string key)
    {
        string nodeName = "reel_setting";
        JSONNode target = (config[nodeName].HasKey(curReelSetting)
                           && config[nodeName][curReelSetting].HasKey($"reel{reelIdx + 1}")) ?
          config[nodeName][curReelSetting][$"reel{reelIdx + 1}"] : config[nodeName][curReelSetting]["default"];

        if (target.HasKey(key))
            return target[key];
        else
            return config[nodeName][curReelSetting]["default"][key];
    }
    public float GetTimeReboundStart(int reelIdx) => (float)GetReelSettingValue(reelIdx, "time_rebound_start");
    public int GetOffsetYReboundStart(int reelIdx) => (int)GetReelSettingValue(reelIdx, "offset_y_rebound_start");

    public float GetTimeTurnOnce(int reelIdx) => (float)GetReelSettingValue(reelIdx, "time_turn_once");
    public float GetTimeReboundEnd(int reelIdx) => (float)GetReelSettingValue(reelIdx, "time_rebound_end");

    public int GetOffsetYReboundEnd(int reelIdx) => (int)GetReelSettingValue(reelIdx, "offset_y_rebound_end");
    public int GetNumReelTurn(int reelIdx) => (int)GetReelSettingValue(reelIdx, "num_reel_turn");
    public int GetNumReelTurnGap(int reelIdx) => (int)GetReelSettingValue(reelIdx, "num_reel_turn_gap");

    public float GetTimeTurnStartDelay(int reelIdx) => (int)GetReelSettingValue(reelIdx, "time_turn_start_delay");

    public Dictionary<string, string> GetSymbolAppearEffect()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (KeyValuePair<string, JSONNode> kv in config["custom"]["symbol_appear_effect"])
        {
            dic.Add(kv.Key, (string)kv.Value);
        }
        return dic;
    }
    public Dictionary<string, string> GetSymbolHitEffect()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (KeyValuePair<string, JSONNode> kv in config["custom"]["symbol_hit_effect"])
        {
            dic.Add(kv.Key, (string)kv.Value);
        }
        return dic;
    }




    // bool isWinEffect  config["custom"][curWinEffectSetting]

    public JSONNode GetWinEffectSettingValue(string key)
    {
        string nodeName = "win_effect_setting";
        JSONNode target = (config[nodeName].HasKey(curWinEffectSetting)
                           && config[nodeName][curWinEffectSetting].HasKey(key)) ?
          config[nodeName][curWinEffectSetting][key] : config[nodeName]["default"][key];

        return target;
    }

    public bool IsWEFrame => (bool)GetWinEffectSettingValue("frame");
    public bool IsWEShowLine => (bool)GetWinEffectSettingValue("line");
    public bool IsWEBigger => (bool)GetWinEffectSettingValue("bigger");
    public bool IsWETwinkle => (bool)GetWinEffectSettingValue("twinkle");

    public bool IsWESymbolAnim => IsWETwinkle ? false : (bool)GetWinEffectSettingValue("anim");
    public bool IsWETotalWinLine => (bool)GetWinEffectSettingValue("total_win_line");
    public bool IsWESingleWinLine => (bool)GetWinEffectSettingValue("single_win_line");
    public bool IsWECredit => (bool)GetWinEffectSettingValue("credit");
    public bool IsWEShowCover => (bool)GetWinEffectSettingValue("cover");
    public bool IsWEHideBaseSymbol => (bool)GetWinEffectSettingValue("hide_base_symbol");

    public bool IsWESkipAtStopImmediately => (bool)GetWinEffectSettingValue("skip_at_stop_immediately");
    public float WETimeS => (float)GetWinEffectSettingValue("time_s");








    public void SelectWinEffectSetting(string key)
    {
        if (!config["win_effect_setting"].HasKey(key))
        {
            DebugUtils.LogError($"can not find key:\"{key}\" in  node \"win_effect_setting\"");
            return;
        }
        curWinEffectSetting = key;
    }

    public void SelectReelSetting(string key)
    {
        if (!config["reel_setting"].HasKey(key))
        {
            DebugUtils.LogError($"can not find key:\"{key}\" in  node \"reel_setting\"");
            return;
        }
        curReelSetting = key;
    }


    public string GetSymbolUrl(int symbolNumber) => config["custom"]["symbol_icon"][$"{symbolNumber}"];

}
