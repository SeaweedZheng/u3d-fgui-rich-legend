using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabHardwareTestScreenController
{
    GComponent onwner;

    GButton btnCheckColor;
    public void InitParam(GComponent comp)
    {
        onwner = comp;

        btnCheckColor = onwner.GetChild("checkColor").asCom.GetChild("value").asButton;
        btnCheckColor.onClick.Clear();
        btnCheckColor.onClick.Add(OnClickCheckColorButton);
    }


    void OnClickCheckColorButton()
    {
        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleCheckScreenColor,null,null);
    }
}
