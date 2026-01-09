using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public partial class StreamingAssetsBundleLoader : MonoBehaviour
{
    private static object _mutex = new object();
    static StreamingAssetsBundleLoader _instance;

    public static StreamingAssetsBundleLoader Instance
    {
        get
        {

            lock (_mutex)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<StreamingAssetsBundleLoader>();
                    // FindObjectOfType(typeof(DevicePrinterOut)) as DevicePrinterOut;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<StreamingAssetsBundleLoader>();

                        obj.name = _instance.GetType().Name;
                        if (obj.transform.parent == null)
                        {
                            DontDestroyOnLoad(obj);
                        }
                    }
                }
                return _instance;
            }
        }
    }



    // 用于存储已加载的AssetBundle的字典，键为bundle名称，值为对应的AssetBundle
    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    //Assets/AstBundle/Consoles/ConsoleFGUIs/Console_fui.bytes
    public void LoadAssetBundleAsync(string assetPathOrBundleName, System.Action<AssetBundle> onComplete)
    {

        string bundelName = GetBundleName(assetPathOrBundleName);

        StartCoroutine(LoadAssetBundleIE(bundelName, onComplete));
    }




    public string GetBundleName(string assetPathOrBundleName)
    {
        string result = assetPathOrBundleName.ToLower();

        string prefixToRemove = "Assets/AstBundle/".ToLower();

        if (result.StartsWith(prefixToRemove))
        {
            result = result.Substring(prefixToRemove.Length);  //去掉 "assets/gameres/"
        }
        string[] str = result.Split('/');
        string fileNameSuffix = str[str.Length - 1];
        if (fileNameSuffix.Contains("."))
        {
            string[] str01 = fileNameSuffix.Split('.');
            int leg = str01[str01.Length - 1].Length + 1;  //".png 或 .prefab"
            result = result.Substring(0, result.Length - leg);
        }
        result += ".unity3d";

        // Assets/AstBundle/Consoles/ConsoleFGUIs 或  Assets/AstBundle/Consoles/ConsolePrefabs/test.prefab
        // 得到 games/console/fguis.unity3d 或 games/console/prefabs/fguis.prefab
        return result;
    }

#if false
    public string GetBundleName(string assetPathOrBundleName)
    {

        if (assetPathOrBundleName.EndsWith(".unity3d"))
        {
            return assetPathOrBundleName;
        }

        int gameResIndex = assetPathOrBundleName.IndexOf("AstBundle");
        string pathAfterAstBundle = assetPathOrBundleName.Substring(gameResIndex + "AstBundle".Length + 1);

        //* Assets/AstBundle/Consoles/ConsoleFGUIs  加载包 games/console/fguis.unity3d
        //* Assets/AstBundle/Consoles/ConsoleFGUIs/Console_atlas0.png
        //加载包 games/console/fguis.unity3d  的资源  Console_atlas0

        /* 【这个代码可以用】
        string bundelName = pathAfterAstBundle;
        if (pathAfterAstBundle.Contains("FGUIs"))  // 目录名
        {
            string folderName = "FGUIs";
            int folderIndex = pathAfterAstBundle.IndexOf(folderName);
            bundelName = pathAfterAstBundle.Substring(0, folderIndex + folderName.Length);
        }
        else // 文件名
        {
            bundelName = bundelName.Split('.')[0]; //去掉后缀名
        }*/


        // Assets/AstBundle/Consoles/ConsoleFGUIs 或  Assets/AstBundle/Consoles/ConsolePrefabs/test.prefab
        string bundelName = pathAfterAstBundle.Split('.')[0]; //去掉后缀名

        bundelName = bundelName.ToLower();
        bundelName += ".unity3d";
        return bundelName;   //Consoles/Consolefguis.unity3d
    }
#endif

    public void LoadAsset<T>(string assetPathOrBundleName, System.Action<T> onComplete) where T : UnityEngine.Object
    {
        // 从路径中提取文件名（不含扩展名）
        string[] str = assetPathOrBundleName.Split('/');
        string fileName = str[str.Length - 1].Split('.')[0];

        // 加载AssetBundle（假设这个方法已实现）
        LoadAssetBundleAsync(assetPathOrBundleName, (bundle) => {
            if (bundle == null)
            {
                Debug.LogError($"AssetBundle加载失败: {assetPathOrBundleName}");
                onComplete?.Invoke(null);
                return;
            }

            string fileNameLow = fileName.ToLower();

            // 查找匹配文件名的资源路径
            string[] allAssetNames = bundle.GetAllAssetNames();
            string targetAssetPath = allAssetNames.FirstOrDefault(
                name =>
                {
                    string res = System.IO.Path.GetFileNameWithoutExtension(name);  // 这里的名字怎么会是小写
                    // name = assets/AstBundle/_Commons/Game Maker/abs/g152/game icon/psson00152.png (资源变成小写？)
                    Debug.Log($"包里资源名：res：{res}  name：{name}  fileNameLow: {fileNameLow}");
                    return res == fileNameLow;
                }
            );


            if (string.IsNullOrEmpty(targetAssetPath))
            {
                Debug.LogError($"未找到匹配资源: {fileName}");
                onComplete?.Invoke(null);
                return;
            }

            // 加载指定类型的资源
            T asset = bundle.LoadAsset<T>(targetAssetPath);

            if (asset != null)
            {
                onComplete?.Invoke(asset);
            }
            else
            {
                Debug.LogError($"资源类型不匹配或加载失败: {targetAssetPath} (请求类型: {typeof(T)})");
                onComplete?.Invoke(null);
            }

            // 如果不需要保留AssetBundle，可以卸载
            // UnloadBundle(bundle,false); // bundle.Unload(false);

        });
    }


    public void UnloadBundle(string assetPathOrBundleName , bool unloadAllLoadedObjects)
    {
        string bundleName = GetBundleName(assetPathOrBundleName);
        if (loadedBundles.ContainsKey(bundleName))
        {
            loadedBundles[bundleName].Unload(unloadAllLoadedObjects);
            loadedBundles.Remove(bundleName);
        }
    }
    public void UnloadBundle(AssetBundle bundle, bool unloadAllLoadedObjects)
    {
        if (loadedBundles.ContainsValue(bundle))
        {
            List<string> keys = loadedBundles.Keys.ToList();

            string bundleName = null;
            for (int i = 0; i < keys.Count; i++)
            {
                if (loadedBundles[keys[i]] == bundle)
                {
                    bundleName = keys[i];
                    break;
                }
            }
            loadedBundles[bundleName].Unload(unloadAllLoadedObjects);
            loadedBundles.Remove(bundleName);
        }
    }

    bool isLoadBundleing = false;

    // 异步加载AB包
    IEnumerator LoadAssetBundleIE(string bundleName, System.Action<AssetBundle> onComplete)
    {
        //避免并发
        while (isLoadBundleing == true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f,0.1f));           
        }
        isLoadBundleing = true;
        System.Action<AssetBundle> _onComplete = (ab) => {
            isLoadBundleing = false;
            onComplete?.Invoke(ab);
        };


        if (loadedBundles.ContainsKey(bundleName))
        {
            _onComplete.Invoke(loadedBundles[bundleName]);
            yield break;
        }

        string path = GetStreamingAssetsPath(bundleName);

        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load bundle: {request.error}");
                    _onComplete?.Invoke(null);
                    yield break;
                }

                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
                loadedBundles.Add(bundleName, assetBundle);

                _onComplete?.Invoke(assetBundle);
            }
        }
        else
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;

            if (request.assetBundle == null)
            {
                Debug.LogError($"Failed to load bundle from: {path}");
            }

            loadedBundles.Add(bundleName, request.assetBundle);
            _onComplete?.Invoke(request.assetBundle);
        }
    }


    public async Task<T> LoadAssetAsync<T>(string assetPathOrBundleName) where T : UnityEngine.Object
    {
        // 从路径中提取文件名（不含扩展名）
        string[] str = assetPathOrBundleName.Split('/');
        string fileName = str[str.Length - 1].Split('.')[0];
        string bundelName = GetBundleName(assetPathOrBundleName);

        AssetBundle bundle = await LoadAssetBundleAsync(bundelName);

        if (bundle == null)
        {
            Debug.LogError($"AssetBundle加载失败: {assetPathOrBundleName}");
            return null;
        }

        string fileNameLow = fileName.ToLower();

        // 查找匹配文件名的资源路径
        string[] allAssetNames = bundle.GetAllAssetNames();
        string targetAssetPath = allAssetNames.FirstOrDefault(
            name =>
            {
                string res = System.IO.Path.GetFileNameWithoutExtension(name);  // 这里的名字怎么会是小写
                                                                                // name = assets/AstBundle/_Commons/Game Maker/abs/g152/game icon/psson00152.png (资源变成小写？)
                Debug.Log($"包里资源名：res：{res}  name：{name}  fileNameLow: {fileNameLow}");
                return res == fileNameLow;
            }
        );

        if (string.IsNullOrEmpty(targetAssetPath))
        {
            Debug.LogError($"未找到匹配资源: {fileName}");
            return null;
        }

        // 加载指定类型的资源
        T asset = bundle.LoadAsset<T>(targetAssetPath);

        if (asset != null)
        {
            return asset;
        }
        else
        {
            Debug.LogError($"资源类型不匹配或加载失败: {targetAssetPath} (请求类型: {typeof(T)})");
            return null;
        }
    }

     
    public async Task<AssetBundle> LoadAssetBundleAsync(string bundleName)
    {
        if (loadedBundles.ContainsKey(bundleName))
        {
            return loadedBundles[bundleName];
        }

        string path = GetStreamingAssetsPath(bundleName);

        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                request.SendWebRequest();

                while (!request.isDone) await Task.Yield(); // 等待下载完成

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load bundle: {request.error}");
                    return null;
                }

                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

                loadedBundles.Add(bundleName, assetBundle);
                return assetBundle;
            } 
        }
        else
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);

            while (!request.isDone) await Task.Yield(); // 等待加载完成

            if (request.assetBundle == null)
            {
                Debug.LogError($"Failed to load bundle from: {path}");
                return null;
            }

            loadedBundles.Add(bundleName, request.assetBundle);
            return request.assetBundle;
        }
    }



    // 获取StreamingAssets中的AB包路径
    private string GetStreamingAssetsPath(string bundleName)
    {
        // Application.streamingAssetsPath 在安卓环境下，路劲自动添加前缀：  jar:file:///
        string path = Path.Combine(PathHelper.abDirSAPTH, bundleName);
        return path;
    }



    /* 【BUG】 这样会导致主线程的阻塞！
    public AssetBundle loadAssetBundle(string pth)
    {

        string bundelName = GetBundleName(pth);

        return LoadAssetBundle(bundelName);
    }
    /// <summary>
    /// 同步方法
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public AssetBundle LoadAssetBundle(string bundleName)
    {
        if (loadedBundles.ContainsKey(bundleName))
        {
            return loadedBundles[bundleName];
        }

        AssetBundle bundle = null;
        string path = GetStreamingAssetsPath(bundleName);

        if (Application.platform == RuntimePlatform.Android)
        {
            // Android 平台：使用 UnityWebRequest 同步加载（不推荐，但可行）
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                request.SendWebRequest(); // 发送请求（异步，但我们可以阻塞等待）

                // 阻塞等待下载完成（会卡住主线程！）
                while (!request.isDone)
                {
                    System.Threading.Thread.Sleep(10); // 避免 CPU 100%
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load bundle: {request.error}");
                    return null;
                }

                bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
        }
        else
        {
            // 非 Android 平台：直接使用同步加载（AssetBundle.LoadFromFile）
            bundle = AssetBundle.LoadFromFile(path);

            if (bundle == null)
            {
                Debug.LogError($"Failed to load bundle from: {path}");
                return null;
            }
        }

        // 缓存加载的 AssetBundle
        if (bundle != null)
        {
            loadedBundles.Add(bundleName, bundle);
        }

        return bundle;
    }*/
}


public partial class StreamingAssetsBundleLoader : MonoBehaviour
{


    public async Task<byte[]> LoadAssetBackUpAsync(string assetPath)
    {

        string path = PathHelper.GetAssetBackupSAPTH(assetPath);

        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                request.SendWebRequest();

                while (!request.isDone) await Task.Yield(); // 等待下载完成

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load bundle: {request.error}");
                    return null;
                }

                return request.downloadHandler.data; // 更推荐使用downloadHandler.data，兼容性更好
            }
        }
        else
        {
            try
            {
                // 关键修改：用Task.Run包装同步读取，实现异步执行（不阻塞主线程）
                byte[] data = await Task.Run(() => File.ReadAllBytes(assetPath));
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"非Android平台读取文件失败: {ex.Message}，路径：{assetPath}");
                return null;
            }
        }
    }


    public async Task<T> LoadAssetBackUpAsync<T>(string assetPath) where T : class
    {
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
            return null;
        }


        byte[] bytes = await LoadAssetBackUpAsync(assetPath);
        return AssetConvertUtils.Convert<T>(bytes);
    }
}
