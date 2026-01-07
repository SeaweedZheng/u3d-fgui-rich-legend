using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingProgressMod
{

    /// <summary> 复制包体中的hotfix dll 到缓存</summary>
    public const string COPY_SA_HOTFIX_DLL = nameof(COPY_SA_HOTFIX_DLL);

    /// <summary> 复制包体中AB包到缓存 </summary>
    public const string COPY_SA_ASSET_BUNDLE = nameof(COPY_SA_ASSET_BUNDLE);

    /// <summary> 复制备份文件 </summary>
    public const string COPY_SA_ASSET_BACKUP = nameof(COPY_SA_ASSET_BACKUP);

    /// <summary> 复制模块版本文件 </summary>
    public const string COPY_SA_ASSET_MOD_VER = nameof(COPY_SA_ASSET_MOD_VER);

    /// <summary> 检查有无待拷贝的文件 </summary>
    public const string CHECK_COPY_TEMP_HOTFIX_FILE = nameof(CHECK_COPY_TEMP_HOTFIX_FILE);

    /// <summary> 检查网络热更版本 </summary>
    public const string CHECK_WEB_VERSION = nameof(CHECK_WEB_VERSION);


    public const string DOWNLOAD_MOD_MAIN = nameof(DOWNLOAD_MOD_MAIN); // 下载ab、dll、backup、拷贝、写入版本号


    public const string DOWNLOAD_MOD_GAME = nameof(DOWNLOAD_MOD_GAME); // 下载ab、dll、backup、拷贝、写入版本号

    public const string DOWNLOAD_MOD_GAME_ONCE = nameof(DOWNLOAD_MOD_GAME_ONCE); // 下载ab、dll、backup、拷贝、写入版本号


    /// <summary> 删除无用的asset bundle </summary>
    public const string DELETE_UNUSE_ASSET_BUNDLE = nameof(DELETE_UNUSE_ASSET_BUNDLE); 

    /// <summary> 删除无用的asset dll </summary>
    public const string DELETE_UNUSE_ASSET_DLL = nameof(DELETE_UNUSE_ASSET_DLL);

    /// <summary> 删除无用的asset backup </summary>
    public const string DELETE_UNUSE_ASSET_BACKUP = nameof(DELETE_UNUSE_ASSET_BACKUP);

    /// <summary> 补充元数据给AOT,而不是给热更新dll补充元数据</summary>
    public const string LOAD_AOT_META_DATA = nameof(LOAD_AOT_META_DATA);

    /// <summary> 加载hotfix dll到内存 </summary>
    public const string LOAD_HOTFIX_DLL = nameof(LOAD_HOTFIX_DLL);







    /// <summary> 预加载AB包到内存 </summary>
    public const string PRELOAD_ASSET_BUNDLE = nameof(PRELOAD_ASSET_BUNDLE);

    /// <summary> 预加载资源到内存 </summary>
    public const string PRELOAD_ASSET = nameof(PRELOAD_ASSET);

    /// <summary> 链接机台（获取参数） </summary>
    public const string CONNECT_MACHINE = nameof(CONNECT_MACHINE);

    /// <summary> 初始化参数设置 </summary>
    public const string INIT_SETTINGS = nameof(INIT_SETTINGS);

    /// <summary> 进入游戏(游戏加载界面) </summary>
    public const string ENTER_GAME = nameof(ENTER_GAME);
}
