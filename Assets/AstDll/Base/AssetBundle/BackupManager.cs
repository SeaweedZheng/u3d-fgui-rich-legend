using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video; // 关键：添加视频命名空间，识别VideoClip

/// <summary>
/// 备份资源加载
/// </summary>
/// <remarks>
/// * 加载项目里 或加载本地的资源。
/// * 加载本地资源时，要判断该资源是否存在，不存在则服务器尝试下载。
/// * 加载本地资源时，如果资源过期，则服务器尝试下载。
/// </remarks>
public class BackupManager : MonoSingleton<BackupManager>
{
    const string BACKUP = nameof(BACKUP);

    string GetBackupKey(string assetPath)
    {
        int hash = assetPath.GetHashCode();
        string key = $"{BACKUP}_{hash}";
        return key;
    }

    public async void LoadAsset<T>(string assetPath, System.Action<T> onFinishCallback, string markBundle = "Default") where T : class // 泛型约束：T必须是引用类型（排除值类型，缩小范围）
    {

        string assetKey = GetBackupKey(assetPath);

        // 运行时严格校验：T是否为允许的类型
        Type targetType = typeof(T);
        bool isAllowedType =
            targetType == typeof(byte[]) ||                // 字节数组
            targetType == typeof(TextAsset) ||             // 文本资源
            targetType == typeof(Texture2D) ||             // 图片（纹理，最常用）
            targetType == typeof(Sprite) ||                // 图片（精灵，可选，根据你的需求保留/删除）                                         // 新增：声音类型
            targetType == typeof(AudioClip);        // 新增：视频类型(mp3)
                                                    //targetType == typeof(VideoClip);  // 可选扩展：若需要支持其他音频/视频类型，在此追加即可（如AudioSource是组件，非资源类型，一般无需添加）


        if (!isAllowedType)
        {
            string errorMsg = $"类型 {targetType.Name} 不被支持！仅允许 byte[]、TextAsset、Texture2D、Sprite、AudioClip（声音）类型。";
            DebugUtils.LogError(errorMsg);
            onFinishCallback?.Invoke(null);
            return;
        }

        if (ApplicationSettings.Instance.IsUseHotfixBundle()) //加载热更新ab包资源
        {
            string localPth = PathHelper.GetAssetBackupLOCPTH(assetPath);

            /*
            // 资源超时使用
            long timeMs = PlayerPrefs.GetInt(assetKey, 0);
            long curTimeMS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if ( curTimeMS - timeMs > 1000)  // 资源超时
            {
                DownloadRemoteAssetBackup(assetPath, (res) =>
                {
                    if ( (int)res[0] == 0)
                    {
                        PlayerPrefs.SetInt(assetKey, (int)curTimeMS);
                    }
                });
            }
            */
            byte[] bytes = await Task.Run(() => File.ReadAllBytes(localPth));

            onFinishCallback?.Invoke(AssetConvertUtils.Convert<T>(bytes));
        }
        else if (ApplicationSettings.Instance.IsUseStreamingAssetsBundle())
        {
            byte[] bytes = await StreamingAssetsBundleLoader.Instance.LoadAssetBackUpAsync(assetPath);
            onFinishCallback?.Invoke(AssetConvertUtils.Convert<T>(bytes));
        }
        else
        {
#if UNITY_EDITOR

            if (typeof(UnityEngine.Object).IsAssignableFrom(targetType))
            {
                UnityEngine.Object target = null;
                if (assetPath != null)
                    target = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, targetType);

                onFinishCallback.Invoke(target as T);

            }
            else
            {
                string projPth = PathHelper.GetAssetBackupPROJPTH(assetPath);
                byte[] bytes = await Task.Run(() => File.ReadAllBytes(projPth));
                onFinishCallback?.Invoke(AssetConvertUtils.Convert<T>(bytes));
            }

            /*
                T target = null;
                if (assetPath != null)
                    target = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);

                //if (IsGameObjectType<T>())  target = Object.Instantiate(target);
                onFinishCallback.Invoke(target);
             */
#endif
        }
    }




    private IEnumerator DownloadRemoteAssetBackup(string nodeName, Action<object[]> onFinish)
    {
        string assetBackupDownloadUrl = PathHelper.GetAssetBackupWEBURL(nodeName);

        // 非cdn加载
        UnityWebRequest req01 = UnityWebRequest.Get(assetBackupDownloadUrl + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return req01.SendWebRequest();

        if (req01.result == UnityWebRequest.Result.Success)
        {
            string writePath = PathHelper.GetAssetBackupLOCPTH(nodeName);
            string directory = Path.GetDirectoryName(writePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(writePath, req01.downloadHandler.data);

            onFinish?.Invoke(new object[] { 0 });
        }
        else
        {
            onFinish?.Invoke(new object[] { 1, $"下载失败: {assetBackupDownloadUrl}" });
        }
    }

}


