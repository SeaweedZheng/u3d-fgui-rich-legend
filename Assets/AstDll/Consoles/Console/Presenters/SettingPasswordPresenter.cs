using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ConsoleSlot01;

public interface IVSettingPassword
{
    void SetButtonStaffActive(bool isActive);
    void SetButtonManagerActive(bool isActive);
    void SetButtonAdminActive(bool isActive);


    event Action onClickSetStaffPassword;
    event Action onClickSetManagerPassword;
    event Action onClickSetAdminPassword;
}

/// <summary> 【未使用】 </summary>
public class SettingPasswordPresenter  
{
    IVSettingPassword view;

    /// 修改用户密码
    PasswordSettingController adminChangePwdCtrl = new PasswordSettingController(PasswordSettingController.UserType.Admin);
    PasswordSettingController managerChangePwdCtrl = new PasswordSettingController(PasswordSettingController.UserType.Manager);
    PasswordSettingController staffChangePwdCtrl = new PasswordSettingController(PasswordSettingController.UserType.Staff);

    public void InitParam(IVSettingPassword v)
    {
        view = v;


        view.onClickSetStaffPassword -= OnClickSetStaffPassword;
        view.onClickSetManagerPassword -= OnClickSetManagerPassword;
        view.onClickSetAdminPassword -= OnClickSetAdminPassword;

        view.onClickSetStaffPassword += OnClickSetStaffPassword;
        view.onClickSetManagerPassword += OnClickSetManagerPassword;
        view.onClickSetAdminPassword += OnClickSetAdminPassword;

        view.SetButtonManagerActive(SBoxModel.Instance.curPermissions >=2);
        view.SetButtonAdminActive(SBoxModel.Instance.curPermissions >= 3);
    }

    void OnClickSetStaffPassword() => staffChangePwdCtrl.OnClickSetPassword();


    void OnClickSetManagerPassword() => managerChangePwdCtrl.OnClickSetPassword();

    void OnClickSetAdminPassword() => adminChangePwdCtrl.OnClickSetPassword();

}
