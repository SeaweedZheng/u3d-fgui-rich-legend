using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IVPasswordSetting
{
    void SetButtonShiftActive(bool isActive);
    void SetButtonManagerActive(bool isActive);
    void SetButtonAdminActive(bool isActive);


    event Action onClickSetShiftPassword;
    event Action onClickSetManagerPassword;
    event Action onClickSetAdminPassword;
}

/// <summary> 【未使用】 </summary>
public class PasswordSettingPresenter  
{
    IVPasswordSetting view;

    public void InitParam(IVPasswordSetting v)
    {
        view = v;



    }
}
