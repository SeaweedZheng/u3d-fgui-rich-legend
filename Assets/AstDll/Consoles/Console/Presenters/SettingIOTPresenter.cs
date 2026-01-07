using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;
using IOT;
public interface IVSettingIOT
{
    event Action onClickIOTAccessMethod;
    event Action<bool> onChangeUseIOT;

    /// <summary>
    /// 设置IOT的连接方式
    /// </summary>
    /// <param name="content"></param>
    void SetIOTAccessMethod(string content);

    /// <summary>
    /// 设置IOT服务器地址
    /// </summary>
    /// <param name="content"></param>
    void SetIOTServer(string content);

    /// <summary>
    /// 设置IOT是否使用
    /// </summary>
    /// <param name="isUse"></param>
    void SetUseIOT(bool isUse);
}


public class SettingIOTPresenter { 
    IVSettingIOT view;

    public void Enable()
    {
        EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }
    public void Disable()
    {
        EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }



    public void InitParam(IVSettingIOT v)
    {
        view = v;
        view.onChangeUseIOT -= OnChangeUseIOT;
        view.onClickIOTAccessMethod -= OnClickIOTAccessMethods;

        view.onChangeUseIOT += OnChangeUseIOT;
        view.onClickIOTAccessMethod += OnClickIOTAccessMethods;

        view.SetIOTServer(IoTConst.GetDevParamURL);
        view.SetUseIOT(SBoxModel.Instance.isUseIot);
        OnPropertyChangeIsConnectIot();
    }



    void OnPropertyChange(EventData res = null)
    {
        string name = res.name;

        if(name == $"{nameof(SBoxModel)}/{nameof(SBoxModel.isConnectIot)}")
        {
             OnPropertyChangeIsConnectIot(res);
        }
    }

    void OnPropertyChangeIsConnectIot(EventData data = null)
    {
        string text =
            I18nMgr.T(SBoxModel.Instance.selectIOTAccessMethod) + " " +
            (SBoxModel.Instance.isConnectIot ? "<img src='ui://Console/icon_link4aff00' width='20' height='20'/>" : "<img src='ui://Console/icon_link666666' width='20' height='20'/>");

        view.SetIOTAccessMethod(text);
    }


    async void OnClickIOTAccessMethods()
    {

        Dictionary<string, string> selectLst = new Dictionary<string, string>();
        foreach (KeyValuePair<int,string>kv in SBoxModel.Instance.iotAccessMethodsLst)
        {
            selectLst.Add($"{kv.Key}", kv.Value);
        }

        Func<string, string> getSelectedDes = (number) =>
        {
            return string.Format(I18nMgr.T("Selected Methods: {0}"), I18nMgr.T(selectLst[number]));
        };


        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleChoose001,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Choose IOT Access Methods"),
                    ["selectLst"] = selectLst,
                    ["selectNumber"] = $"{SBoxModel.Instance.iotAccessMethods}",
                    ["getSelectedDes"] = getSelectedDes,
                }));



        if (res!=null && res.value != null)
        {
            string numberStr = (string)res.value;
            try
            {
                int idx = int.Parse(numberStr);

                SBoxModel.Instance.iotAccessMethods = idx;

                OnPropertyChangeIsConnectIot();
                MachineDeviceCommonBiz.Instance.CheckIOT();
            }
            catch
            {

            }
        }
    }


    void OnChangeUseIOT(bool isUse)
    {
        SBoxModel.Instance.isUseIot = isUse;
        MachineDeviceCommonBiz.Instance.CheckIOT();
    }


}
