using FairyGUI;
using SBoxApi;
using System;
using UnityEngine;



public class TabHardwareTestButtonController
{

    GButton btnTicketOut, btnSpin, btnSet, btnDoor, btnScoreUp, btnScoreDown;

    GTextField txtTip;

    GComponent owner;
    public void InitParam(GComponent comp)
    {
        owner = comp;

        txtTip = owner.GetChild("tip").asCom.GetChild("title").asTextField;
        txtTip.text = "";

        btnTicketOut = owner.GetChild("btnTicketOut").asButton;
        btnTicketOut.onClick.Clear();

        btnSpin = owner.GetChild("btnSpin").asButton;
        btnSpin.onClick.Clear();

        btnSet = owner.GetChild("btnSet").asButton;
        btnSet.onClick.Clear();

        btnDoor = owner.GetChild("btnDoor").asButton;
        btnDoor.onClick.Clear();

        btnScoreUp = owner.GetChild("btnScoreUp").asButton;
        btnScoreUp.onClick.Clear();

        btnScoreDown = owner.GetChild("btnScoreDown").asButton;
        btnScoreDown.onClick.Clear();
    }

    public void Enable()
    {
        Timers.inst.Remove(ReadIORepeat);
        Timers.inst.Add(1,0, ReadIORepeat);
    }

    public void Disable()
    {
        Timers.inst.Remove(ReadIORepeat);
    }


    void ReadIORepeat(object param)
    {
        if (txtTip == null) return;

        int buttonValue = SBoxIdea.GetCoinPushKeyState();
        txtTip.text = string.Format(I18nMgr.T("[IO Value]: {0}"), buttonValue)  + " - ";

        // 遍历枚举的所有值
        foreach (SBOX_IDEA_COIN_PUSH_KEYVALUE button in Enum.GetValues(typeof(SBOX_IDEA_COIN_PUSH_KEYVALUE)))
        {
            // 检查当前按钮是否被按下
            if ((buttonValue & (int)button) != 0)
            {
                txtTip.text += $"{Enum.GetName(typeof(SBOX_IDEA_COIN_PUSH_KEYVALUE), button)} ; ";
            }
        }
    }

    public void OnMachineButtonClickUp(MachineButtonKey btn)
    {
        switch (btn)
        {
            case MachineButtonKey.BtnSpin:
                {
                    btnSpin.selected = false;
                }
                break;
            case MachineButtonKey.BtnTicketOut:
                { 
                    btnTicketOut.selected = false;
                }
                break;
            case MachineButtonKey.BtnConsole:
                {
                    btnSet.selected = false;
                }
                break;
            case MachineButtonKey.BtnDoor:
                {
                    btnDoor.selected = false;
                }
                break;
            case MachineButtonKey.BtnCreditDown:
                {
                    btnScoreDown.selected = false;
                }
                break;
            case MachineButtonKey.BtnCreditUp:
                {
                    btnScoreUp.selected = false;
                }
                break;
        }
    }

    public void OnMachineButtonClickDown(MachineButtonKey btn)     
    {
        switch (btn)
        {
            case MachineButtonKey.BtnSpin:
                {
                    btnSpin.selected = true;
                }
                break;
            case MachineButtonKey.BtnTicketOut:
                {
                    btnTicketOut.selected = true;
                }
                break;
            case MachineButtonKey.BtnConsole:
                {
                    btnSet.selected = true;
                }
                break;
            case MachineButtonKey.BtnDoor:
                {
                    btnDoor.selected = true;
                }
                break;
            case MachineButtonKey.BtnCreditDown:
                {
                    btnScoreDown.selected = true;
                }
                break;
            case MachineButtonKey.BtnCreditUp:
                {
                    btnScoreUp.selected = true;
                }
                break;
        }
    }


}
