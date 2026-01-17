using FairyGUI;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using SlotMaker;
using Sirenix.OdinInspector;
public interface IVSlotMachine
{
    void ShowPayLines(SymbolWin symbolWin);
    void ShowTwinkleEffect(int colIndex, int rowIndex);

    void ShowBiggerEffect(int colIndex, int rowIndex);
    void AddBorderEffect(int colIndex, int rowIndex);
    void AddSymbolEffect(int colIndex, int rowIndex, string symbolName, bool isAmin = true);
    void SymbolAppearEffect(int reelIndex);
    void ClearTween(int reelIndex);
    void SetReelEndResult(int reelIndex, List<int> reelResult);
    void ResetIconData(int reelIndex);
    void MoveY(int reelIndex, float yTo, float duration, Action onFinish);
    IEnumerator Rebound(int reelIndex, float yTo = 80, float durationS = 0.05f);


    //void InitDeck();

   // void SetVisiableDeck(List<List<int>> deckColRowNumber);
    void CloseAllPlayLines();

    void SetReelsDeck(List<List<int>> reelsResultColRowNumber);

    // 去除层级
    void ReturnSortingOrder();

    void ReturnSymbolEffectToPool(int colIndex, int rowIndex, string[] exclude);

    void StopSymbolEffect(int colIndex, int rowIndex );
    void HideBaseSymbolIcon(int colIndex, int rowIndex, bool isHide);


    void SetReelDeck(int colIndex, List<int> reelResult);

    event Action<int> onSymbolPointerEnter; //void OnSymbolPointerEnter(int reelIndex);
    event Action<int> onSymbolPointerExit;


    void SetSlotCover(bool isShow);

    int GetVisibleSymbolNumberFromDeck(int colIndex, int rowIndex);
}

public partial class SlotMachinePresenter : MonoBehaviour, IPSlotMachine
{

    IVSlotMachine view;
    SlotMachineConfig config;

    bool isInit = false;
    protected bool isSymbolAppearEffectWhenReelStop;

    public bool isStopImmediately = false;

    List<List<int>> deckColRowNumber;

    List<bool> isReelPointeringLst;

    List<Coroutine> coReelTurnLst;
    List<Coroutine> coReelToStopLst;

    List<int> curRollTimeLst;
    List<int> needRollTimeLst;

    List<List<int>> reelsResult;
    List<ReelState> reelStateLst;

    List<Action> reelStopCallbackLst;

    public event Action<EventData> onWinEvent;
    public event Action<EventData> onSlotDetailEvent;
    public event Action<EventData> onSlotEvent;
    public event Action<EventData> onPrepareTotalWinCreditEvent;
    public event Action<EventData> onTotalWinCreditEvent;
    public event Action<EventData> onContentEvent;



    public void Init()
    {
        if (isInit) return;
        isInit = true;


    }


    public void InitParam(IVSlotMachine v, SlotMachineConfig cf)
    {
        Init();

        if (view != null)
        {
            view.onSymbolPointerEnter -= OnSymbolPointerEnter;
            view.onSymbolPointerExit -= OnSymbolPointerExit;
        }

        view = v;
        config = cf;

        isReelPointeringLst = new List<bool>();
        reelStateLst = new List<ReelState>();

        coReelTurnLst = new List<Coroutine>();
        coReelToStopLst = new List<Coroutine>();
        curRollTimeLst = new List<int> ();
        needRollTimeLst = new List<int>();

        reelStopCallbackLst = new List<Action>();

        for (int i = 0; i < config.Column; i++)
        {
            isReelPointeringLst.Add(false);
            reelStateLst.Add(ReelState.Idle);
            coReelTurnLst.Add(null);
            coReelToStopLst.Add(null);
            reelStopCallbackLst.Add(null);

            curRollTimeLst.Add(0);
            needRollTimeLst.Add(0);
        }
        view.onSymbolPointerEnter += OnSymbolPointerEnter;
        view.onSymbolPointerExit += OnSymbolPointerExit;


        // 初始化，可见的界面

        view.CloseAllPlayLines();
    }


    void OnSymbolPointerEnter(int reelIndex)
    {
        isReelPointeringLst[reelIndex] = true;
        ReelToStop(reelIndex);
    }
    void OnSymbolPointerExit(int reelIndex)
    {
        isReelPointeringLst[reelIndex] = false;
    }


    /// <summary> 关闭遮罩层 </summary>
    public void CloseSlotCover() => view.SetSlotCover(false);

    /// <summary> 打开遮罩层 </summary>
    public void OpenSlotCover() => view.SetSlotCover(true);

    public void SetSlotCover(bool isShow) => view.SetSlotCover(isShow);




    #region 事件
    public void BeginTurn() => onContentEvent?.Invoke(new EventData(SlotMachineEvent.BeginTurn));

    public void BeginSpin()
    {
        onSlotEvent?.Invoke(new EventData(SlotMachineEvent.SpinSlotMachine));
        onContentEvent?.Invoke(new EventData(SlotMachineEvent.BeginSpin));
    }

    public void SendTotalBonusCreditEvent(long BonusCredit)
    {
        EventCenter.Instance.EventTrigger<EventData>(SlotMachineEvent.ON_WIN_EVENT,
            new EventData<long>(SlotMachineEvent.SingleWinBonus, BonusCredit));
    }

    /// <summary>
    /// 显示总赢(给控制面板)
    /// </summary>
    /// <param name="winList"></param>
    public void SendTotalWinCreditEvent(List<SymbolWin> winList)
    {
        long totalWinCredit = 0;
        foreach (SymbolWin win in winList)
        {
            totalWinCredit += win.earnCredit;
        }
        SendTotalWinCreditEvent(totalWinCredit);
    }
    public void SendTotalWinCreditEvent(long totalWinCredit)
    {
        onWinEvent?.Invoke(new EventData<long>(SlotMachineEvent.TotalWinCredit, totalWinCredit));
    }

    public long GetTotalWinCredit(List<SymbolWin> winList)
    {
        long totalWinCredit = 0;
        foreach (SymbolWin win in winList)
        {
            totalWinCredit += win.earnCredit;
        }
        return totalWinCredit;
    }

    public void SendPrepareTotalWinCreditEvent(long totalWinCredit, bool isAddToCredit = false)
    {
        PrepareTotalWinCredit res = new PrepareTotalWinCredit()
        {
            totalWinCredit = totalWinCredit,
            isAddToCredit = isAddToCredit,
        };
        onPrepareTotalWinCreditEvent?.Invoke(new EventData<PrepareTotalWinCredit>(SlotMachineEvent.PrepareTotalWinCredit, res));
    }


    #endregion






    #region 工具


    public SymbolWin GetTotalSymbolWin(List<SymbolWin> winList)
    {
        List<string> bsLst = new List<string>();

        long earnCredit = 0;
        List<Cell> cells = new List<Cell>();

        List<int> lineNumbers = new List<int>();

        foreach (SymbolWin sw in winList)
        {
            foreach (Cell cel in sw.cells)
            {
                int symbolNumber = deckColRowNumber[cel.column][cel.row + config.DeckUpStartIndex];
                string mark = $"{cel.column}-{cel.row}#{symbolNumber}";

                if (bsLst.Contains(mark))
                    continue;
                cells.Add(new Cell(cel.column, cel.row));
                bsLst.Add(mark);
            }

            // 获得所有赢线的线号
            if (!lineNumbers.Contains(sw.lineNumber))
                lineNumbers.Add(sw.lineNumber);

            earnCredit += sw.earnCredit;
        }

        TotalSymbolWin totalWin = new TotalSymbolWin()
        {
            lineNumbers = lineNumbers,
            earnCredit = earnCredit,
            cells = cells,
        };

        return totalWin;
    }

    public IEnumerator SlotWaitForSeconds(float waitS)
    {
        if (waitS <= 0f)
            yield break;

        float targetTimeS = Time.time + waitS;
        while (targetTimeS > Time.time)
        {
            yield return null;
        }
    }


    public int GetSymbolIndex(int symbolNumber) => config.SymbolNumbers.IndexOf(symbolNumber);

    public int GetSymbolNumber(int symbolIndex) => config.SymbolNumbers[symbolIndex];


    #endregion
}
