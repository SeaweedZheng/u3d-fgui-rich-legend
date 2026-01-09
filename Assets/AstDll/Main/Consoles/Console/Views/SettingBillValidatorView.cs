using FairyGUI;
using System;

public class SettingBillValidatorView : IVSettingBillValidator
{

    GButton btnBillValidatorModel, tgBillValidator;

    public void InitParam(GComponent comp)
    {
        btnBillValidatorModel = comp.GetChild("billValidatorModel").asCom.GetChild("value").asButton;
        btnBillValidatorModel.onClick.Clear();
        btnBillValidatorModel.onClick.Add(() =>
        {
            onClickBillValidatorModel?.Invoke();
        });

        tgBillValidator = comp.GetChild("billValidator").asCom.GetChild("switch").asButton;
        tgBillValidator.onChanged.Clear();
        tgBillValidator.onChanged.Add(OnChangeIsUseBiller);
        tgBillValidator.selected = SBoxModel.Instance.isUseBiller;
    }

    public event Action<bool> onChangeBillValidator;

    public event Action onClickBillValidatorModel;

    public void SetBillValidatorModel(string content)
    {
        btnBillValidatorModel.text = content;
    }

    public void SetUseBillValidator(bool isUse)
    {
        tgBillValidator.selected = isUse;
    }


    void OnChangeIsUseBiller(EventContext context)
    {
        GButton toggle = context.sender as GButton;
        onChangeBillValidator?.Invoke(toggle.selected);
    }


}
