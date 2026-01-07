using GameMaker;
using GameUtil;
using Newtonsoft.Json;
using SBoxApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxController : Singleton<SandboxController>
{
    private DelayTimer coinOutTimer;

    public void Init()
    {
        AddEventListener();
        AddHardwareBtnEvent();
    }
    /*
    public void StartCoinOut()
    {

        int coinNum = 0;// TODO: Mathf.FloorToInt(player.credit / offset); 
        if (coinNum == 0)
        {
            //TODO: Show error message
            return;
        }
        SBoxModel.Instance.coinOuting = true;
        SBoxModel.Instance.totalCoinOutNum = coinNum;

        SBoxSandbox.CoinOutStart(0, coinNum, 0);

        CoinOutTimer();
    }*/

    private void AddEventListener()
    {
        EventCenter.Instance.AddEventListener<CoinInData>(SBoxSanboxEventHandle.COIN_IN, OnCoinIn);
        EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_OUT, OnCoinOut);
    }

    private void AddHardwareBtnEvent()
    {
        if (Application.isEditor)
        {
            //EventCenter.Instance.AddEventListener<ulong>(EventHandle.HARDWARE_KEY_DOWN, (key) => { });
            //EventCenter.Instance.AddEventListener<ulong>(EventHandle.HARDWARE_KEY_UP, (key) => { });
            //EventCenter.Instance.AddEventListener<ulong>(EventHandle.HARDWARE_KEY_CLICK, (key) => { });
            //EventCenter.Instance.AddEventListener<ulong>(EventHandle.HARDWARE_KEY_LONG_PRESS, (key) => { });
        }
        else
        {
            //SBoxSandboxListener.Instance.AddButtonDown(SBOX_SWITCH.SWITCH_CONFIRM, () => { });
            //SBoxSandboxListener.Instance.AddButtonUp(SBOX_SWITCH.SWITCH_CONFIRM, () => { });
            //SBoxSandboxListener.Instance.AddButtonClick(SBOX_SWITCH.SWITCH_CONFIRM, () => { });
            //SBoxSandboxListener.Instance.AddButtonLongPress(SBOX_SWITCH.SWITCH_CONFIRM, () => { });
        }
    }

    private void OnCoinIn(CoinInData coinInData)
    {
        /**
        SBoxModel.Instance.coinInFrame += coinInData.coinNum;
        if (IOCanvasModel.Instance.CfgData == null
            || IOCanvasModel.Instance.CfgData.CoinValue == 0)
            return;
        SaveFrame("CoinIn", SBoxModel.Instance.coinInFrame);

        //TODO: Add player.credit logic here
        **/
    }

    private void OnCoinOut(int coinOutNum)
    {
        /**
        if (IOCanvasModel.Instance.CfgData.TicketValue == 0
            || IOCanvasModel.Instance.CfgData.scoreTicket == 0)
            return;
        if (IOCanvasModel.Instance.CfgData.TicketValue >= IOCanvasModel.Instance.CfgData.scoreTicket)
            OnCoinOutLogic(coinOutNum);
        else
            OnCoinOutLogicAnotherWay(coinOutNum);
        **/
    }

    private void OnCoinOutLogic(int coinOutNum)
    {
        /*
        SBoxModel.Instance.totalCoinOutNum -= coinOutNum;

        //TODO:
        //if (player.Credit < coinOutNum * IOCanvasModel.Instance.CfgData.TicketValue / IOCanvasModel.Instance.CfgData.scoreTicket)
        //{
        //    //TODO: Show error message
        //    return;
        //}

        SBoxModel.Instance.coinOutFrame += coinOutNum;
        SaveFrame("CoinOut", SBoxModel.Instance.coinOutFrame);

        //TODO: Add player.credit logic here

        if (SBoxModel.Instance.totalCoinOutNum > 0)
            CoinOutTimer();
        */
    }

    private void OnCoinOutLogicAnotherWay(int coinOutNum)
    {
        /**
        SBoxModel.Instance.totalCoinOutNum -= coinOutNum;
        SBoxModel.Instance.curCoinOutNum += coinOutNum;

        if (SBoxModel.Instance.curCoinOutNum == IOCanvasModel.Instance.CfgData.scoreTicket)
        {
            SBoxModel.Instance.curCoinOutNum = 0;
            //TODO:
            //if (player.Credit < coinOutNum * IOCanvasModel.Instance.CfgData.TicketValue / IOCanvasModel.Instance.CfgData.scoreTicket)
            //{
            //    //TODO: Show error message
            //    return;
            //}

            //TODO: Add player.credit logic here
            SaveFrame("CoinOut", SBoxModel.Instance.coinOutFrame);
        }

        if (SBoxModel.Instance.totalCoinOutNum > 0)
            CoinOutTimer();
        **/
    }
    /*
    private void CoinOutTimer()
    {
        if (coinOutTimer == null)
            coinOutTimer = Timer.DelayAction(5, () => {
                if (SBoxModel.Instance.totalCoinOutNum > 0)
                {
                    //TODO: Show error message
                    DebugUtils.Log("CoinOut Timeout");
                }
                StopCoinOut();
                SBoxModel.Instance.coinOuting = false;
            });
        else
            coinOutTimer.Restart();
    }

    private void StopCoinOut()
    {
        for (int i = 0; i < 2; i++)
            SBoxSandbox.CoinOutStop(i);
    }

    private void SaveFrame(string str, int value)
    {
        PlayerPrefs.SetInt(str, value);
        PlayerPrefs.GetInt(str);
    }*/
}
