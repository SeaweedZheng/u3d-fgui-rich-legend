using FairyGUI;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _spinWEMD = SlotMaker.SpinWinEffectSettingModel;
public class Symbol01 : Symbol
{
    /// <summary>
    /// fgui动画用
    /// </summary>
    /// <param name="anchorSymbolEffect"></param>
    /// <param name="isAmin"></param>
    /// <returns></returns>
    public override GComponent AddSymbolEffect(GComponent anchorSymbolEffect, bool isAmin = true)
    {
        anchorSymbolEffect.visible = true;
        GComponent goAnimator = goOwnerSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(anchorSymbolEffect); //边长为1的点
        anchorSymbolEffect.xy = new Vector2(0, 0);
        anchorSymbolEffect.SetPivot(0.5f, 0.5f);
        // 是否隐藏原有图标
        if (_spinWEMD.Instance.isHideBaseIcon)
        {
            HideBaseSymbolIcon(true);
        }

        return anchorSymbolEffect;
        // 播放动画
    }
}
