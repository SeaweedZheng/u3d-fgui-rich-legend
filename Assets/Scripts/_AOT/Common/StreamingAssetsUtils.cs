using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
public class StreamingAssetsUtils 
{
    // string path = PathHelper.GetAssetBackupSAPTH(assetPath);
    public static async Task<byte[]> LoadAssetAsync(string path, Action<byte[]> onFinishCallback = null)
    {

        if (Application.platform == RuntimePlatform.Android)
        {

            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                request.SendWebRequest();

                while (!request.isDone) await Task.Yield(); // 等待下载完成


                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load bundle: {request.error}");


                    onFinishCallback?.Invoke(null);
                    return null;
                }

                onFinishCallback?.Invoke(request.downloadHandler.data);
                return request.downloadHandler.data; // 更推荐使用downloadHandler.data，兼容性更好
            }
        }
        else
        {
            try
            {
                // 关键修改：用Task.Run包装同步读取，实现异步执行（不阻塞主线程）
                byte[] data = await Task.Run(() => File.ReadAllBytes(path));

                onFinishCallback?.Invoke(data);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"非Android平台读取文件失败: {ex.Message}，路径：{path}");

                onFinishCallback?.Invoke(null);
                return null;
            }
        }
    }



}
