using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class SettingInOutView : IVSettingInOutPresnter
{

    GButton btnCoinInScale, btnCoinOutScale, btnScoreScale;

    public void InitParam(GComponent comp)
    {
        btnCoinInScale = comp.GetChild("coinInScale").asCom.GetChild("value").asButton;
        btnCoinInScale.onClick.Clear();
        btnCoinInScale.onClick.Add(() => { onClickCoinInScale?.Invoke();   });

        btnCoinOutScale = comp.GetChild("coinOutScale").asCom.GetChild("value").asButton;
        btnCoinOutScale.onClick.Clear();
        btnCoinOutScale.onClick.Add(() => { onClickCoinOutScale?.Invoke();   });

        btnScoreScale = comp.GetChild("scoreScale").asCom.GetChild("value").asButton;
        btnScoreScale.onClick.Clear();
        btnScoreScale.onClick.Add(() => { onClickScoreScale?.Invoke(); });

    }

    public event Action onClickCoinInScale;
    public event Action onClickCoinOutScale;
    public event Action onClickScoreScale;

    public void SetCoinInScaleTxt(string content)
    {
        btnCoinInScale.title = content;
    }

    public void SetCoinOutScaleTxt(string content)
    {
        btnCoinOutScale.title = content;
    }
    public void SetScoreScaleTxt(string content)
    {
        btnScoreScale.title = content;
    }
}
