using GameMaker;
using SBoxApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FairyGUI;

public interface IVSettingLineIdMachineId
{
    event Action onClickLineIdMachineId;

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void SetMachineId(string id);
    void SetLineId(string id);
    void SetBtnsActive(bool isActive);
}
public class SettingLineIdMachineIdPresenter
{
    IVSettingLineIdMachineId view;

    public void InitParam(IVSettingLineIdMachineId v)
    {
        view = v;

        view.SetMachineId(SBoxModel.Instance.MachineId);
        view.SetLineId(SBoxModel.Instance.LineId);


        view.onClickLineIdMachineId -= OnClickAgentIDMachineID;
        view.onClickLineIdMachineId += OnClickAgentIDMachineID;

        bool isActive = SBoxModel.Instance.isCurPermissionsAdmin ? true :
        SBoxModel.Instance.isCurPermissionsManager ? !SBoxIdea.IsMachineIdReady(): false;

        view.SetBtnsActive(isActive);
    }


    async void OnClickAgentIDMachineID()
    {

        Func<string, string> checkAgnetIDFunc = (res) =>
        {
            if (string.IsNullOrEmpty(res))
                return string.Format(I18nMgr.T("The {0} cannot be empty"), I18nMgr.T("Agent ID"));

            try
            {
                int num = int.Parse(res);
            }
            catch (Exception ex)
            {
                return I18nMgr.T("The input value must be a number");
            }

            if (res.Length != 4)
                return string.Format(I18nMgr.T("The {0} must be {1} digits long"), I18nMgr.T("Agent ID"), 4);

            return null;
        };

        Func<string, string> checkMachineIDFunc = (res) =>
        {
            if (string.IsNullOrEmpty(res))
                return string.Format(I18nMgr.T("The {0} cannot be empty"), I18nMgr.T("Machine ID"));

            try
            {
                int num = int.Parse(res);
            }
            catch (Exception ex)
            {
                return I18nMgr.T("The input value must be a number");
            }

            if (res.Length != 8)
                return string.Format(I18nMgr.T("The {0} must be {1} digits long"), I18nMgr.T("Machine ID"), 8);

            return null;
        };

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSetParameter002,
                new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Set Machine ID"),
                    ["paramName1"] = I18nMgr.T("Agent ID:"),
                    ["paramName2"] = I18nMgr.T("Machine ID:"),
                    ["checkParam1Func"] = checkAgnetIDFunc,
                    ["checkParam2Func"] = checkMachineIDFunc,
                }
            ));


        if (res.value != null)
        {
            List<string> lst = (List<string>)res.value;
            string machineId = lst[1];
            string agentId = lst[0];  //machineId.Substring(0, 4);
            if (machineId == SBoxModel.Instance.MachineId)
            {
                TipPopupHandler.Instance.OpenPopup(I18nMgr.T("The settings have not changed and do not need to be saved"));
            }
            else if (!machineId.StartsWith(agentId))
            {
                TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Machine ID must start with Agent ID"));
            }
            else
            {

                UnityAction OnConfirmModify = () =>
                {
                    MachineDataUtils.RequestSetLineIDMachineID(int.Parse(agentId), int.Parse(machineId),
                    (res) =>
                    {
                        SBoxPermissionsData data = res as SBoxPermissionsData;
                        if (data.result == 0)
                            TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Successfully saved"));
                        else
                            TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Save failed"));

                        //要延时？
                        CheckButtonActiveLineIdMachineId();
                    },
                    (err) =>
                    {
                        TipPopupHandler.Instance.OpenPopup(I18nMgr.T(err.msg));

                        CheckButtonActiveLineIdMachineId();
                    });
                };


                if (SBoxModel.Instance.isCurPermissionsAdmin)
                {
                    OnConfirmModify();
                }
                else
                {
                    CommonPopupHandler.Instance.OpenPopup(new CommonPopupInfo()
                    {
                        // 只能修改一次线号机台号，确定要修改？
                        type = CommonPopupType.YesNo,
                        text = I18nMgr.T("You can only modify the Agent ID and Machine ID once. Are you sure you want to modify it?"),
                        buttonText1 = I18nMgr.T("Cancel"),
                        buttonText2 = I18nMgr.T("OK"),
                        callback1 = null,
                        callback2 = OnConfirmModify,
                    });
                }
            }
        }
    }


    void CheckButtonActiveLineIdMachineId()
    {

        TimerCallback callback = (param) =>
        {
            view.SetLineId(SBoxModel.Instance.LineId);
            view.SetMachineId(SBoxModel.Instance.MachineId);

            if (SBoxModel.Instance.isCurPermissionsAdmin)
            {
                view.SetBtnsActive(true);
            }
            else
            {
                view.SetBtnsActive(false);
            }
        };

        callback(null);

        Timers.inst.Add(0.5f,1, callback);
        //Timer.DelayAction(0.5f, callback);
    }




}
