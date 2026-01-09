using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;
public class SettingLineIdMachineIdView : IVSettingLineIdMachineId
{
    public event Action onClickLineIdMachineId;

    GButton btnAgentID, btnMachineID;
    public void InitParam(GComponent _comp)
    {
        btnAgentID = _comp.GetChild("agentID").asCom.GetChild("value").asButton;
        btnAgentID.onClick.Clear();
        btnAgentID.onClick.Add(OnClickAgentIDMachineID);

        btnMachineID = _comp.GetChild("machineID").asCom.GetChild("value").asButton;
        btnMachineID.onClick.Clear();
        btnMachineID.onClick.Add(OnClickAgentIDMachineID);
    }

    void OnClickAgentIDMachineID()=> onClickLineIdMachineId?.Invoke();

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    public void SetMachineId(string id)
    {
        btnMachineID.text = id;
    }
    public void SetLineId(string id)
    {
        btnAgentID.text = id;
    }
    public void SetBtnsActive(bool isActive)
    {
        btnAgentID.touchable = isActive;
        btnMachineID.touchable = isActive;

        btnAgentID.GetChild("untouchable").visible = !isActive;
        btnMachineID.GetChild("untouchable").visible = !isActive;
    }
}
