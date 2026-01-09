using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
public interface IVSettingBillValidator
{

    event Action<bool> onChangeBillValidator;

    event Action onClickBillValidatorModel;

    void SetBillValidatorModel(string content);

    void SetUseBillValidator(bool isUse);
}
public class SettingBillValidatorPresenter
{
    IVSettingBillValidator view;

    public void InitParam(IVSettingBillValidator v)
    {
        view = v;

        view.onClickBillValidatorModel -= OnClickBillValidatorModel;
        view.onChangeBillValidator -= OnChangeIsUseBiller;


        view.onClickBillValidatorModel += OnClickBillValidatorModel;
        view.onChangeBillValidator += OnChangeIsUseBiller;


        view.SetUseBillValidator(SBoxModel.Instance.isUseBiller);
        OnPropertyChangeIsConnectBiller();
    }


    public void Enable()
    {
        EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }
    public void Disable()
    {
        EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }

    void OnPropertyChangeIsConnectBiller(EventData data = null)
    {
        string text =
            SBoxModel.Instance.selectBillerModel + " " +
            (SBoxModel.Instance.IsConnectBiller ? "<img src='ui://Console/icon_link4aff00' width='20' height='20'/>" : "<img src='ui://Console/icon_link666666' width='20' height='20'/>");
       
        view.SetBillValidatorModel(text);
    }
    void OnPropertyChange(EventData res = null)
    {
        string name = res.name;
        switch (name)
        {
            case "SBoxModel/IsConnectBiller":
                OnPropertyChangeIsConnectBiller(res);
                break;
        }
    }

    void OnChangeIsUseBiller(bool isUse)
    {
        SBoxModel.Instance.isUseBiller = isUse;

        MachineDeviceCommonBiz.Instance.InitBiller(() => { }, (err) => { });
    }

    async void OnClickBillValidatorModel()
    {

        Dictionary<string, string> selectLst = new Dictionary<string, string>();
        for (int i = 0; i < SBoxModel.Instance.suppoetBillers.Count; i++)
        {
            DeviceInfo item = SBoxModel.Instance.suppoetBillers[i];
            selectLst.Add(item.number.ToString(), $"{item.manufacturer} : {item.model}");
        }



        Func<string, string> getSelectedDes = (number) =>
        {
            return string.Format("{0} : {1}", I18nMgr.T("Manufacturer"), I18nMgr.T("Model"));
        };

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleChoose001,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Choose Bill Validator Model"),
                    ["selectLst"] = selectLst,
                    ["selectNumber"] = $"{SBoxModel.Instance.selectBillerNumber}",
                    ["getSelectedDes"] = getSelectedDes,
                }));

        if (res.value != null)
        {
            string selectNumber = (string)res.value;
            int number = int.Parse(selectNumber);

            SBoxModel.Instance.selectBillerNumber = number;
            OnPropertyChangeIsConnectBiller();

            MachineDeviceCommonBiz.Instance.InitBiller(() =>
            {
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Biller setup successful"));
            }, (err) =>
            {
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Biller setup failed"));
            });
        }
    }


}
