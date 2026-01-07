using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotfixState 
{
    public const string HOTFIX_STATE = nameof(HOTFIX_STATE);
    public const string HotfixCompleted = nameof(HotfixCompleted);
    public const string HotfixCopying = nameof(HotfixCopying);
    public const string HotfixDownloading = nameof(HotfixDownloading);
    public const string HotfixDownloadFail = nameof(HotfixDownloadFail);


    public const string HOTFIX_THROW_ERR = nameof(HOTFIX_THROW_ERR);
    public static string hotfixThrowErrMsg = "";
}
