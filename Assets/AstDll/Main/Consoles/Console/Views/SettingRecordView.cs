using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;

public class SettingRecordView : IVSettingRecord
{
    GButton
    btnMaxCoinInOutRecord, btnMaxGameRecord, btnMaxEventRecord, btnMaxErrorRecord, btnMaxBusinessDayRecord;


    public void InitParam(GComponent comp)
    {
        btnMaxCoinInOutRecord = comp.GetChild("maxCoinInOutRecord").asCom.GetChild("value").asButton;
        btnMaxCoinInOutRecord.onClick.Clear();
        btnMaxCoinInOutRecord.onClick.Add(()=> {
            onClickMaxCoinInOutRecord?.Invoke();
        });
        btnMaxCoinInOutRecord.title = SBoxModel.Instance.coinInOutRecordMax.ToString();

        btnMaxGameRecord = comp.GetChild("maxGameRecord").asCom.GetChild("value").asButton;
        btnMaxGameRecord.onClick.Clear();
        btnMaxGameRecord.onClick.Add(()=> {
            onClickMaxGameRecord?.Invoke();
            });
        btnMaxGameRecord.title = SBoxModel.Instance.gameRecordMax.ToString();

        btnMaxEventRecord = comp.GetChild("maxEventRecord").asCom.GetChild("value").asButton;
        btnMaxEventRecord.onClick.Clear();
        btnMaxEventRecord.onClick.Add(() => {
            onClickMaxEventRecord?.Invoke();
        });
        btnMaxEventRecord.title = SBoxModel.Instance.eventRecordMax.ToString();

        btnMaxErrorRecord = comp.GetChild("maxErrorRecord").asCom.GetChild("value").asButton;
        btnMaxErrorRecord.onClick.Clear();
        btnMaxErrorRecord.onClick.Add(() => {
            onClickMaxErrorRecord?.Invoke();
        });
        btnMaxErrorRecord.title = SBoxModel.Instance.errorRecordMax.ToString();

        btnMaxBusinessDayRecord = comp.GetChild("maxBusinessDayRecord").asCom.GetChild("value").asButton;
        btnMaxBusinessDayRecord.onClick.Clear();
        btnMaxBusinessDayRecord.onClick.Add(() => {
            onClickMaxBusinessDayRecord?.Invoke();
        });
        btnMaxBusinessDayRecord.title = SBoxModel.Instance.businiessDayRecordMax.ToString();
    }


    public event Action onClickMaxCoinInOutRecord;
    public event Action onClickMaxGameRecord;
    public event Action onClickMaxEventRecord;
    public event Action onClickMaxErrorRecord;
    public event Action onClickMaxBusinessDayRecord;
    public event Action onClickMaxJackpotRecord;






    public void SetMaxCoinInOutRecord(int count)
    {
        btnMaxCoinInOutRecord.title = count.ToString();
    }
    public void SetMaxGameRecord(int count)
    {
        btnMaxGameRecord.title = count.ToString();
    }
    public void SetMaxEventRecord(int count)
    {
        btnMaxEventRecord.title = count.ToString();
    }
    public void SetMaxErrorRecord(int count)
    {
        btnMaxErrorRecord.title = count.ToString();
    }
    public void SetMaxBusinessDayRecord(int count)
    {
        btnMaxBusinessDayRecord.title = count.ToString();
    }


    public void SetMaxJackpotRecord(int count)
    {

    }
}
