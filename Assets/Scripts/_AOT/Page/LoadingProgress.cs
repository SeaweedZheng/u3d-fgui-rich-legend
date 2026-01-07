using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingProgress
{

    /// <summary> 复制包体中的hotfix dll 到缓存</summary>
    public const string COPY_SA_HOTFIX_DLL = "COPY_SA_HOTFIX_DLL";
    /// <summary> 复制包体中AB包到缓存 </summary>
    public const string COPY_SA_ASSET_BUNDLE = "COPY_SA_ASSET_BUNDLE";
    /// <summary> 复制备份文件 </summary>
    public const string COPY_SA_ASSET_BACKUP = "COPY_SA_ASSET_BACKUP";

    /// <summary> 检查有无待拷贝的文件 </summary>
    public const string CHECK_COPY_TEMP_HOTFIX_FILE = "CHECK_COPY_TEMP_HOTFIX_FILE";

    /// <summary> 检查网络热更版本 </summary>
    public const string CHECK_WEB_VERSION = "CHECK_WEB_VERSION";

    /// <summary> 下载hotfix dll </summary>
    public const string DOWNLOAD_ASSET_DLL = "DOWNLOAD_ASSET_DLL";

    /// <summary> 下载热更AB包 </summary>
    public const string DOWNLOAD_ASSET_BUNDLE = "DOWNLOAD_ASSET_BUNDLE";

    /// <summary> 下载"资源备份" </summary>
    public const string DOWNLOAD_ASSET_BACKUP = "DOWNLOAD_ASSET_BACKUP";


    /// <summary> 拷贝下载的文件 </summary>
    public const string COPY_TEMP_HOTFIX_FILE = "COPY_TEMP_HOTFIX_FILE";


    /// <summary> 删除无用的ab包 </summary>
    public const string DELETE_UNUSE_ASSET_BUNDLE = "DELETE_UNUSE_ASSET_BUNDLE";

    /// <summary> 删除无用的hotfix dll </summary>
    public const string DELETE_UNUSE_ASSET_DLL = "DELETE_UNUSE_ASSET_DLL";


    /// <summary> 删除无用的hotfix dll </summary>
    public const string DELETE_UNUSE_ASSET_BACKUP = "DELETE_UNUSE_ASSET_BACKUP";


    /// <summary> 加载AOT dll到内存 </summary>
    //public const string LOAD_AOT_DLL = "LOAD_AOT_DLL";

    /// <summary> 补充元数据给AOT,而不是给热更新dll补充元数据</summary>
    public const string LOAD_AOT_META_DATA = "LOAD_AOT_META_DATA";

    /// <summary> 加载hotfix dll到内存 </summary>
    public const string LOAD_ASSET_DLL = "LOAD_HOTFIX_DLL";






    /// <summary> 预加载AB包到内存 </summary>
    public const string PRELOAD_ASSET_BUNDLE = "PRELOAD_ASSET_BUNDLE";

    /// <summary> 预加载资源到内存 </summary>
    public const string PRELOAD_ASSET = "PRELOAD_ASSET";

    /// <summary> 链接机台（获取参数） </summary>
    public const string CONNECT_MACHINE = "CONNECT_MACHINE";

    /// <summary> 初始化参数设置 </summary>
    public const string INIT_SETTINGS = "INIT_SETTINGS";

    /// <summary> 进入游戏(游戏加载界面) </summary>
    public const string ENTER_GAME = "ENTER_GAME";
}
