using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AssetConvertUtils
{

    public static T Convert<T>(byte[] data) where T : class
    {
        // 1. 前置校验：字节数组为空直接返回null
        if (data == null || data.Length == 0)
        {
            Debug.LogError("转换失败：传入的byte[]为空！");
            return null;
        }

        // 2. 获取目标类型，进行类型校验
        Type targetType = typeof(T);
        object result = null;

        // 3. 分支处理：不同类型的专属转换逻辑
        try
        {

            if (targetType == typeof(byte[]))
            {
                result = data;
            }
            else if (targetType == typeof(TextAsset))
            {
                // byte[] 转 TextAsset：先转UTF8字符串，再构造TextAsset
                string textContent = Encoding.UTF8.GetString(data);
                result = new TextAsset(textContent);
            }
            else if (targetType == typeof(Texture2D))
            {
                // byte[] 转 Texture2D：使用LoadImage自动识别PNG/JPG等格式
                Texture2D texture = new Texture2D(2, 2); // 初始尺寸无意义，LoadImage会自动重置
                bool loadSuccess = texture.LoadImage(data); // 自动识别图片格式
                if (!loadSuccess)
                {
                    Debug.LogError("转换Texture2D失败：传入的byte[]不是有效的图片格式（PNG/JPG等）！");
                    GameObject.DestroyImmediate(texture); // 销毁无效纹理，避免内存泄漏
                    return null;
                }
                result = texture;
            }
            else if (targetType == typeof(Sprite))
            {
                // byte[] 转 Sprite：先转Texture2D，再通过Texture2D创建Sprite
                Texture2D texture = Convert<Texture2D>(data);
                if (texture == null)
                {
                    Debug.LogError("转换Sprite失败：无法先将byte[]转为Texture2D！");
                    return null;
                }
                // 创建Sprite：使用纹理完整区域，中心点居中
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f) // pivot：中心点
                );
                result = sprite;
            }
            else if (targetType == typeof(AudioClip))
            {
                // byte[] 转 AudioClip：仅支持WAV格式（Unity原生支持，MP3需额外插件）
                result = ConvertByteToAudioClip(data, "TempAudioClip");
            }
            else
            {
                // 非法类型提示
                Debug.LogError($"不支持将byte[]转换为 {targetType.Name} 类型！仅支持 TextAsset、Texture2D、Sprite、AudioClip。");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"转换 {targetType.Name} 失败：{ex.Message}");
            return null;
        }

        // 4. 类型转换并返回
        return result as T;
    }


    /// <summary>
    /// 辅助方法：byte[]（WAV格式）转 AudioClip
    /// </summary>
    /// <param name="audioData">WAV格式的字节数组</param>
    /// <param name="clipName">AudioClip名称</param>
    /// <returns>转换后的AudioClip（失败返回null）</returns>
    private static AudioClip ConvertByteToAudioClip(byte[] audioData, string clipName)
    {
        try
        {
            // 解析WAV文件头：获取采样率、声道数等信息（简化版，支持标准WAV格式）
            if (audioData.Length < 44) // 标准WAV文件头至少44字节
            {
                Debug.LogError("转换AudioClip失败：传入的byte[]不是有效的WAV格式（文件头过短）！");
                return null;
            }

            // 从WAV头中读取采样率（第24-27字节）
            int sampleRate = BitConverter.ToInt32(audioData, 24);
            // 从WAV头中读取声道数（第22-23字节）
            int channels = BitConverter.ToInt16(audioData, 22);
            // 音频数据长度（总长度 - 头长度44）
            int audioDataLength = audioData.Length - 44;

            // 创建AudioClip
            AudioClip audioClip = AudioClip.Create(
                clipName,
                audioDataLength / 2, // 样本数：16位音频，每个样本占2字节
                channels,
                sampleRate,
                false // 非循环
            );

            // 将WAV音频数据写入AudioClip
            float[] audioFloatData = new float[audioDataLength / 2];
            for (int i = 0; i < audioFloatData.Length; i++)
            {
                // 16位有符号整数转浮点数（范围：-1 ~ 1）
                short sample = BitConverter.ToInt16(audioData, 44 + i * 2);
                audioFloatData[i] = sample / 32768f;
            }
            audioClip.SetData(audioFloatData, 0);

            return audioClip;
        }
        catch (Exception ex)
        {
            Debug.LogError($"转换AudioClip失败：{ex.Message}");
            return null;
        }
    }

}
