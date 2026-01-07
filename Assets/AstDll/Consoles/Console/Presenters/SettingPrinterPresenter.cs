using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;

public interface IVSettingPrinter
{
    event Action<bool> onChangeUsePrinter;

    event Action  onClickPrinterModel;

    void SetPrinterModel(string content);

    void SetUsePrinter(bool isUse);
}
public class SettingPrinterPresenter
{
    IVSettingPrinter view;

    public void InitParam(IVSettingPrinter v)
    {
        view = v;

        view.onClickPrinterModel -= OnClickPrinterModel;
        view.onChangeUsePrinter -= OnChangeUsePrinter;


        view.onClickPrinterModel += OnClickPrinterModel;
        view.onChangeUsePrinter += OnChangeUsePrinter;


        view.SetUsePrinter(SBoxModel.Instance.isUsePrinter);
        OnPropertyChangeIsConnectPrinter();
    }

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
            case "SBoxModel/IsConnectPrinter":
                OnPropertyChangeIsConnectPrinter(res);
                break;
        }
    }

    void OnPropertyChangeIsConnectPrinter(EventData data = null)
    {
        string text =
             SBoxModel.Instance.selectPrinterModel + " " +
            (SBoxModel.Instance.IsConnectPrinter ? "<img src='ui://Console/icon_link4aff00' width='20' height='20'/>" : "<img src='ui://Console/icon_link666666' width='20' height='20'/>");
        view.SetPrinterModel(text);
    }



    async void OnClickPrinterModel()
    {

        Dictionary<string, string> selectLst = new Dictionary<string, string>();
        for (int i = 0; i < SBoxModel.Instance.supportPrinters.Count; i++)
        {
            DeviceInfo item = SBoxModel.Instance.supportPrinters[i];
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
                    ["title"] = I18nMgr.T("Choose Printer Model"),
                    ["selectLst"] = selectLst,
                    ["selectNumber"] = $"{SBoxModel.Instance.selectPrinterNumber}",
                    ["getSelectedDes"] = getSelectedDes,
                }));

        if (res.value != null)
        {
            string selectNumber = (string)res.value;
            int number = int.Parse(selectNumber);
            SBoxModel.Instance.selectPrinterNumber = number;
            OnPropertyChangeIsConnectPrinter();

            MachineDeviceCommonBiz.Instance.InitPrinter(() =>
            {
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Printer setup successful"));
            }, (err) =>
            {
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Printer setup failed"));
            });
        }
    }

    void OnChangeUsePrinter(bool isUse)
    {
        SBoxModel.Instance.isUsePrinter = isUse;

        MachineDeviceCommonBiz.Instance.InitPrinter(() => { }, (err) => { });
    }
}
