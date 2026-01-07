using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalXMLInfo
{
    public string content = "";
    public long hash = 0;
}

/// <summary> 每个xml文件的信息 </summary>
public class I18nXMLInfo
{
    public string path;
    public string mark;
}

/// <summary> 每个xml文件的信息 </summary>
public class I18nJsonInfo
{
    public string path;
    public string mark;  //如 gameId 、  bundleName
}