using FairyGUI;
using System;
using System.Collections;

public class SettingPrinterView : IVSettingPrinter
{
    public event Action<bool> onChangeUsePrinter;

    public event Action onClickPrinterModel;

    GButton btnPrinterModel, tgPrinter;
    public void InitParam(GComponent comp)
    {
        btnPrinterModel = comp.GetChild("printerModel").asCom.GetChild("value").asButton;
        btnPrinterModel.onClick.Clear();
        btnPrinterModel.onClick.Add(() =>
        {
            onClickPrinterModel?.Invoke();
        });


        tgPrinter = comp.GetChild("usePrinter").asCom.GetChild("switch").asButton;
        tgPrinter.onChanged.Clear();
        tgPrinter.onChanged.Add(OnChangeIsUsePrinter);
        tgPrinter.selected = SBoxModel.Instance.isUsePrinter;
    }
    void OnChangeIsUsePrinter(EventContext context)
    {
        GButton toggle = context.sender as GButton;

        onChangeUsePrinter?.Invoke(toggle.selected);
    }

    public void SetPrinterModel(string content)
    {
        btnPrinterModel.title = content;
    }

    public void SetUsePrinter(bool isUse)
    {
        tgPrinter.selected = isUse;
    }
}
