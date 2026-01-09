using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingIOTView : IVSettingIOT
{
    GButton btnIotAccessMechods, tgUseIot;

    GRichTextField txtIotServer;
    public void InitParam(GComponent comp)
    {
        btnIotAccessMechods = comp.GetChild("iotAccessMethods").asCom.GetChild("value").asButton;
        btnIotAccessMechods.onClick.Clear();
        btnIotAccessMechods.onClick.Add(() =>
        {
            onClickIOTAccessMethod?.Invoke();
        });

        tgUseIot = comp.GetChild("useIOT").asCom.GetChild("switch").asButton;
        tgUseIot.onChanged.Clear();
        tgUseIot.onChanged.Add(OnChangeUseIOT);

        txtIotServer = comp.GetChild("iotAddresse").asCom.GetChild("value").asRichTextField;
    }

    public event Action onClickIOTAccessMethod;
    public event Action<bool> onChangeUseIOT;


    void OnChangeUseIOT(EventContext context)
    {
        GButton toggle = context.sender as GButton;

        onChangeUseIOT?.Invoke(toggle.selected);
    }

    public void SetIOTAccessMethod(string content)
    {
        btnIotAccessMechods.text = content;
    }
    public void SetIOTServer(string content)
    {
        txtIotServer.text = content;
    }

    public void SetUseIOT(bool isUse)
    {
        tgUseIot.selected  = isUse;
    }
}
