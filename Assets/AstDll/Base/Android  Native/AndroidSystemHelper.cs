using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public static partial class AndroidSystemHelper
{
    /// <summary>
    /// 设置系统音量
    /// </summary>
    /// <param name="volume">音量值 (0.0-1.0)</param>
    public static void SetSystemVolume(float volume)
    {
        volume = Mathf.Clamp01(volume); // 确保音量在 0-1 范围内

#if UNITY_ANDROID && !UNITY_EDITOR
        SetAndroidSystemVolume(volume);
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        //SetWindowsSystemVolume(volume);
#endif
    }

    /// <summary>
    /// 获取系统音量
    /// </summary>
    /// <returns>音量值 (0.0-1.0)</returns>
    public static float GetSystemVolume()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetAndroidSystemVolume();
//#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
//        return GetWindowsSystemVolume();
#else
        return 1.0f;
#endif
    }

#if UNITY_ANDROID
    private static void SetAndroidSystemVolume(float volume)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject audioManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "audio"))
            {
                int maxVolume = audioManager.Call<int>("getStreamMaxVolume", 3);
                int setVolume = Mathf.RoundToInt(volume * maxVolume);
                audioManager.Call("setStreamVolume", 3, setVolume, 0);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"设置Android系统音量失败: {e.Message}");
        }
    }

    private static float GetAndroidSystemVolume()
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject audioManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "audio"))
            {
                int maxVolume = audioManager.Call<int>("getStreamMaxVolume", 3);
                int currentVolume = audioManager.Call<int>("getStreamVolume", 3);
                return maxVolume > 0 ? (float)currentVolume / maxVolume : 1.0f;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"获取Android系统音量失败: {e.Message}");
            return 1.0f;
        }
    }
#endif
}