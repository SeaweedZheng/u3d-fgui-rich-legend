using GameMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVSettingRemoteControl
{
    event Action<bool> onChangeUseRemoteControl;
    event Action onClickSetRemoteControlServer;
    event Action onClickSetRemoteControlAccount;
    void SetRemoteControlAccount(string content);

    void SetRemoteControlAddress(string content);

    void SetUseRemoteControl(bool isUse);
}
public class SettingRemoteControlPresenter
{
    IVSettingRemoteControl view;

    public void Enable()
    {
        EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }
    public void Disable()
    {
        EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }
    void OnPropertyChange(EventData res = null)
    {
        string name = res.name;
        switch (name)
        {
            case "SBoxModel/isConnectRemoteControl":
                OnPropertyChangeIsConnectRemoteControl(res);
                break;
        }
    }

    public void InitParam(IVSettingRemoteControl v)
    {
        view = v;

        view.onChangeUseRemoteControl -= OnChangeUseRemoteControl;
        view.onClickSetRemoteControlServer -= OnClickRemoteControlSetting;
        view.onClickSetRemoteControlAccount -= OnClickRemoteControlAccount;

        view.onChangeUseRemoteControl += OnChangeUseRemoteControl;
        view.onClickSetRemoteControlServer += OnClickRemoteControlSetting;
        view.onClickSetRemoteControlAccount += OnClickRemoteControlAccount;


        view.SetUseRemoteControl(SBoxModel.Instance.isUseRemoteControl);
        OnPropertyChangeRemoteControlAccount();
        OnPropertyChangeIsConnectRemoteControl();
    }

    void OnChangeUseRemoteControl(bool isOn)
    {
        SBoxModel.Instance.isUseRemoteControl = isOn;
        MachineDeviceCommonBiz.Instance.CheckMqttRemoteButtonController();
        OnPropertyChangeIsConnectRemoteControl();
    }

    async void OnClickRemoteControlSetting()
    {

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSetParameter001,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Configure Remote Server Connection"),
                }
            ));

        if (res!= null && res.value != null)
        {
            string addr = (string)res.value;
            try
            {
                SBoxModel.Instance.remoteControlSetting = addr;

                OnPropertyChangeIsConnectRemoteControl();
                MachineDeviceCommonBiz.Instance.CheckMqttRemoteButtonController();
            }
            catch
            {

            }
        }
    }
    void OnPropertyChangeIsConnectRemoteControl(EventData data = null)
    {

        string text =
            SBoxModel.Instance.remoteControlSetting + " " +
            (SBoxModel.Instance.isConnectRemoteControl ? "<img src='ui://Console/icon_link4aff00' width='20' height='20'/>" : "<img src='ui://Console/icon_link666666' width='20' height='20'/>");
        view.SetRemoteControlAddress(text);
    }



    async void OnClickRemoteControlAccount()
    {

        Func<string, string> chekParam1Func = (res) =>
        {
            if (string.IsNullOrEmpty(res))
                return string.Format(I18nMgr.T("The {0} cannot be empty"), I18nMgr.T("Account"));
            return null;
        };

        Func<string, string> chekParam2Func = (res) =>
        {
            if (string.IsNullOrEmpty(res))
                return string.Format(I18nMgr.T("The {0} cannot be empty"), I18nMgr.T("Password"));
            return null;
        };

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSetParameter002,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Set Remote Control Account"),
                    ["paramName1"] = I18nMgr.T("Account:"),
                    ["paramName2"] = I18nMgr.T("Password:"),
                    ["checkParam1Func"] = chekParam1Func,
                    ["checkParam2Func"] = chekParam2Func,
                }
            ));

        if (res.value != null)
        {
            List<string> lst = (List<string>)res.value;
            try { 

                SBoxModel.Instance.remoteControlAccount = lst[0];
                SBoxModel.Instance.remoteControlPassword = lst[1];

                OnPropertyChangeRemoteControlAccount();
            }
            catch
            {

            }
        }

    }

    void OnPropertyChangeRemoteControlAccount(EventData data = null)
    {
        view.SetRemoteControlAccount( $"{SBoxModel.Instance.remoteControlAccount} / {SBoxModel.Instance.remoteControlPassword}");
    }



}
