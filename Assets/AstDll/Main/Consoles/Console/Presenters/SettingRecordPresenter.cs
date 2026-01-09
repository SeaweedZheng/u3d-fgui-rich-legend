using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;
public interface IVSettingRecord
{
    event Action onClickMaxCoinInOutRecord;
    event Action onClickMaxGameRecord;
    event Action onClickMaxEventRecord;
    event Action onClickMaxErrorRecord;
    event Action onClickMaxBusinessDayRecord;
    event Action onClickMaxJackpotRecord;
    void SetMaxCoinInOutRecord(int count);
    void SetMaxGameRecord(int count);
    void SetMaxEventRecord(int count);
    void SetMaxErrorRecord(int count);
    void SetMaxBusinessDayRecord(int count);

    void SetMaxJackpotRecord(int count);
}
public class SettingRecordPresenter
{
    IVSettingRecord view;

    public void InitParam(IVSettingRecord v)
    {
        view = v;

        view.onClickMaxErrorRecord -= OnClickMaxErrorRecord;
        view.onClickMaxEventRecord -= OnClickMaxEventRecord;
        view.onClickMaxGameRecord -= OnClickMaxGameRecord;
        view.onClickMaxBusinessDayRecord -= OnClickMaxBusinessDayRecord;
        view.onClickMaxCoinInOutRecord -= OnClickMaxCoinInOutRecord;
        view.onClickMaxJackpotRecord -= OnClickMaxJackpotRecord;

        view.onClickMaxErrorRecord += OnClickMaxErrorRecord;
        view.onClickMaxEventRecord += OnClickMaxEventRecord;
        view.onClickMaxGameRecord += OnClickMaxGameRecord;
        view.onClickMaxBusinessDayRecord += OnClickMaxBusinessDayRecord;
        view.onClickMaxCoinInOutRecord += OnClickMaxCoinInOutRecord;
        view.onClickMaxJackpotRecord += OnClickMaxJackpotRecord;


        view.SetMaxCoinInOutRecord(SBoxModel.Instance.coinInOutRecordMax);
        view.SetMaxGameRecord(SBoxModel.Instance.gameRecordMax);
        view.SetMaxEventRecord(SBoxModel.Instance.eventRecordMax);
        view.SetMaxErrorRecord(SBoxModel.Instance.errorRecordMax);
        view.SetMaxBusinessDayRecord(SBoxModel.Instance.businiessDayRecordMax);
    }





    async void OnClickMaxCoinInOutRecord()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleKeyboard002,
        new EventData<Dictionary<string, object>>("",
            new Dictionary<string, object>()
            {
                ["title"] = I18nMgr.T("Max Coin In Out Record"),
                ["isPlaintext"] = true,
            }));

        if (res.value != null)
        {
            bool isErr = true;

            int minMaxCoinInOutRecord = DefaultSettingsUtils.minMaxCoinInOutRecord;
            int maxMaxCoinInOutRecord = DefaultSettingsUtils.maxMaxCoinInOutRecord;
            try
            {
                int val = int.Parse((string)res.value);  // (long)res.value;

                if (val >= minMaxCoinInOutRecord
                    && val <= maxMaxCoinInOutRecord
                    )
                {
                    isErr = false;
                    SBoxModel.Instance.coinInOutRecordMax = val;
                    view.SetMaxCoinInOutRecord(SBoxModel.Instance.coinInOutRecordMax);

                }

            }
            catch { }

            if (isErr)
                TipPopupHandler.Instance.OpenPopup(string.Format(I18nMgr.T("The {0} must be between {1} and {2}"),
                    I18nMgr.T("Max Coin In Out Record"),
                    minMaxCoinInOutRecord, maxMaxCoinInOutRecord));
        }
    }



    async void OnClickMaxGameRecord()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleKeyboard002,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Max Game Record"),
                    ["isPlaintext"] = true,
                }));

        if (res.value != null)
        {
            bool isErr = true;

            int minMaxGameRecord = DefaultSettingsUtils.minMaxGameRecord;
            int maxMaxGameRecord = DefaultSettingsUtils.maxMaxGameRecord;
            try
            {
                int val = int.Parse((string)res.value);  // (long)res.value;

                if (val >= minMaxGameRecord
                    && val <= maxMaxGameRecord
                    )
                {
                    isErr = false;
                    SBoxModel.Instance.gameRecordMax = val;
                    view.SetMaxGameRecord(SBoxModel.Instance.gameRecordMax);
                }
            }
            catch { }

            if (isErr)
                TipPopupHandler.Instance.OpenPopup(string.Format(I18nMgr.T("The {0} must be between {1} and {2}"),
                    I18nMgr.T("Max Game Record"),
                    minMaxGameRecord, maxMaxGameRecord));
        }
    }



    async void OnClickMaxEventRecord()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleKeyboard002,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Max Event Record"),
                    ["isPlaintext"] = true,
                }));

        if (res.value != null)
        {
            bool isErr = true;

            int minMaxRecord = DefaultSettingsUtils.minMaxEventRecord;
            int maxMaxRecord = DefaultSettingsUtils.maxMaxEventRecord;
            try
            {
                int val = int.Parse((string)res.value);  // (long)res.value;

                if (val >= minMaxRecord
                    && val <= maxMaxRecord
                    )
                {
                    isErr = false;
                    SBoxModel.Instance.eventRecordMax = val;
                    view.SetMaxEventRecord(SBoxModel.Instance.eventRecordMax);
                }
            }
            catch { }

            if (isErr)
                TipPopupHandler.Instance.OpenPopup(string.Format(I18nMgr.T("The {0} must be between {1} and {2}"),
                    I18nMgr.T("Max Event Record"),
                    minMaxRecord, maxMaxRecord));
        }
    }






    async void OnClickMaxErrorRecord()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleKeyboard002,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Max Warning Record"),
                    ["isPlaintext"] = true,
                }));

        if (res.value != null)
        {
            bool isErr = true;

            int minMaxRecord = DefaultSettingsUtils.minMaxErrorRecord;
            int maxMaxRecord = DefaultSettingsUtils.maxMaxErrorRecord;
            try
            {
                int val = int.Parse((string)res.value);  // (long)res.value;

                if (val >= minMaxRecord
                    && val <= maxMaxRecord
                    )
                {
                    isErr = false;
                    SBoxModel.Instance.errorRecordMax = val;
                    view.SetMaxErrorRecord(SBoxModel.Instance.errorRecordMax);
                }
            }
            catch { }

            if (isErr)
                TipPopupHandler.Instance.OpenPopup(string.Format(I18nMgr.T("The {0} must be between {1} and {2}"),
                    I18nMgr.T("Max Warning Record"),
                    minMaxRecord, maxMaxRecord));
        }
    }


    async void OnClickMaxBusinessDayRecord()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleKeyboard002,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Max Business Day Record"),
                    ["isPlaintext"] = true,
                }));

        if (res.value != null)
        {
            bool isErr = true;

            int minMaxRecord = DefaultSettingsUtils.minMaxBusinessDayRecord;
            int maxMaxRecord = DefaultSettingsUtils.maxMaxBusinessDayRecord;
            try
            {
                int val = int.Parse((string)res.value);  // (long)res.value;

                if (val >= minMaxRecord
                    && val <= maxMaxRecord
                    )
                {
                    isErr = false;
                    SBoxModel.Instance.businiessDayRecordMax = val;
                    view.SetMaxBusinessDayRecord(SBoxModel.Instance.businiessDayRecordMax);
                }
            }
            catch { }

            if (isErr)
                TipPopupHandler.Instance.OpenPopup(string.Format(I18nMgr.T("The {0} must be between {1} and {2}"),
                    I18nMgr.T("Max Business Day Record"),
                    minMaxRecord, maxMaxRecord));
        }
    }


    async void OnClickMaxJackpotRecord()
    {

    }
}
