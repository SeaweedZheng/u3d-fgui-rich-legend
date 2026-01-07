using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

//主工程使用
public class DllHelper
{
    private static DllHelper _instance;
    public static DllHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DllHelper();
            }
            return _instance;
        }
    }

    /// <summary> 需要热更的dll (排序先后有要求)</summary>
    public  List<string> DllNameList = new List<string>() { "Base", "Main" };

    public Dictionary<string, int> DllLevelDic = new Dictionary<string, int>()
    {
        ["Base"] = 0,
        ["Main"] = 1
    };


    /// <summary> 
    /// 针对后期热更dll个数变多的情况
    /// </summary>
    public List<string> GetDllNameNoSuffixList(JObject node)
    {
        List<string> lst = DllNameList;
        try
        {
            if (node.ContainsKey("asset_dll"))
            {
                JObject arr = node["asset_dll"] as JObject;
                if (arr.Count > 0)
                {
                    lst = new List<string>();

                    for (int i = 0; i<10; i++)
                    {
                        foreach (KeyValuePair<string,JToken> item in arr)
                        {
                            if ( item.Value["level"].Value<int>() == i)
                                lst.Add(item.Key.Replace(".dll.bytes",""));
                        }                      
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError($"节点hotfix_dll_load_order 解析错误：{node.ToString()}");
        }

        return lst;
    }



    private Dictionary<string, Assembly> DllDic = new Dictionary<string, Assembly>();
    public void SethotUpdateAss(string name, Assembly ass)
    {
        if (!DllDic.ContainsKey(name))
        {
            DllDic.Add(name, ass);
        }
    }

    public Assembly GetAss(string AssName)
    {
        if (DllDic.ContainsKey(AssName))
        {
            return DllDic[AssName];
        }
        return null;
    }

}
