using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SettingPasswordView : IVSettingPassword
{

    GButton  btnChangePwdStaff, btnChangePwdManager, btnChangePwdAdmin;
    public void InitParam(GComponent _comp)
    {
        btnChangePwdStaff = _comp.GetChild("changeShiftPassword").asCom.GetChild("btn").asButton;
        btnChangePwdStaff.onClick.Clear();
        btnChangePwdStaff.onClick.Add(() =>
        {
            onClickSetStaffPassword?.Invoke();
        });


        btnChangePwdManager = _comp.GetChild("changeManagerPassword").asCom.GetChild("btn").asButton;
        btnChangePwdManager.onClick.Clear();
        btnChangePwdManager.onClick.Add(() =>
        {
            onClickSetManagerPassword?.Invoke();
        });


        btnChangePwdAdmin = _comp.GetChild("changeAdminPassword").asCom.GetChild("btn").asButton;
        btnChangePwdAdmin.onClick.Clear();
        btnChangePwdAdmin.onClick.Add(() =>
        {
            onClickSetAdminPassword?.Invoke();
        });
    }

    public void SetButtonStaffActive(bool isActive)
    {
        btnChangePwdStaff.visible = isActive;
    }
    public void SetButtonManagerActive(bool isActive)
    {
        btnChangePwdManager.visible = isActive;
    }
    public void SetButtonAdminActive(bool isActive)
    {
        btnChangePwdAdmin.visible = isActive;
    }


    public event Action onClickSetStaffPassword;
    public event Action onClickSetManagerPassword;
    public event Action onClickSetAdminPassword;

}
