using FairyGUI;
using System;

public class SettingRemoteControlView : IVSettingRemoteControl
{
    GButton btnRemoteControlServer, btnRemoteControlAccount, tgUseRemoteControl;
    public void InitParam(GComponent comp)
    {
        tgUseRemoteControl = comp.GetChild("useRemoteControl").asCom.GetChild("switch").asButton;
        tgUseRemoteControl.onChanged.Clear();
        tgUseRemoteControl.onChanged.Add(OnChangeIsUseBiller);

        btnRemoteControlServer = comp.GetChild("remoteControlSetting").asCom.GetChild("value").asButton;
        btnRemoteControlServer.onClick.Add(() =>
        {
            onClickSetRemoteControlServer?.Invoke();
        });

        btnRemoteControlAccount = comp.GetChild("remoteControlAccount").asCom.GetChild("value").asButton;
        btnRemoteControlAccount.onClick.Add(() =>
        {
            onClickSetRemoteControlAccount?.Invoke();
        });
    }




    public event Action<bool> onChangeUseRemoteControl;
    public event Action onClickSetRemoteControlServer;
    public event Action onClickSetRemoteControlAccount;


    void OnChangeIsUseBiller(EventContext context)
    {
        GButton toggle = context.sender as GButton;
        onChangeUseRemoteControl?.Invoke(toggle.selected);
    }


    public void SetRemoteControlAccount(string content)
    {
        btnRemoteControlAccount.text = content;
    } 

    public void SetRemoteControlAddress(string content)
    {
        btnRemoteControlServer.text = content;
    }

    public void SetUseRemoteControl(bool isUse)
    {
        tgUseRemoteControl.selected = isUse;
    }
}
