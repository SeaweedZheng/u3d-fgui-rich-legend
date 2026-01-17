using GameMaker;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SlotMachinePresenter 
{

    public void SkipWinLine(bool isIncludeTag)
    {

        // 打开基础图标
        for (int c = 0; c < config.Column; c++)
        {
            for (int r = config.DeckUpStartIndex; r <= config.DeckUpEndIndex; r++)
            {
                view.StopSymbolEffect(c, r);
                view.HideBaseSymbolIcon(c, r, false);
            }
        }

        // 去除层级功能
        view.ReturnSortingOrder();
        //FguiSortingOrderManager.Instance.ReturnSortingOrder(maskSortOrder);
        //FguiSortingOrderManager.Instance.ReturnAllSortingOrder();

        string[] exclude = isIncludeTag ? new string[] { } : new string[] { };
        for (int c = 0; c < config.Column; c++)
        {
            for (int r = config.DeckUpStartIndex; r <= config.DeckUpEndIndex; r++)
            {
                view.ReturnSymbolEffectToPool(c, r, exclude);
            }
        }

        view.CloseAllPlayLines();

        onWinEvent?.Invoke(new EventData(SlotMachineEvent.SkipWinLine));
    }

    public IEnumerator ShowSymbolWinBySetting(SymbolWin symbolWin, bool isUseMySelfSymbolNumber, SpinWinEvent eventType)
    {

        //停止特效显示
        SkipWinLine(false);


        // 立马停止时，不播放赢分环节？
        if (isStopImmediately && config.IsWESkipAtStopImmediately)
            yield break;

        //显示遮罩
        SetSlotCover(config.IsWEShowCover);

        Dictionary<string, string> symbolHitEffect = config.GetSymbolHitEffect();

        foreach (Cell cel in symbolWin.cells)
        {

            int symbolNumberSelf = view.GetVisibleSymbolNumberFromDeck(cel.column, cel.row);

            int symbolNumber = isUseMySelfSymbolNumber ? symbolNumberSelf : symbolWin.symbolNumber;

            string symbolName = symbolHitEffect[$"{symbolNumber}"];  // wild  or symbol;


            /*
            // 【替换】图标动画  
            GComponent goSymbolHit = fguiPoolHelper.GetObject(TagPoolObject.SymbolHit, symbolName).asCom;
            GComponent goSymbol = GetVisibleSymbolFromDeck(cel.column, cel.row);
            AddSymbolEffect(goSymbol, goSymbolHit, IsWESymbolAnim);

            int rowIndex = cel.row;
            // 设置层级
            FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goExpectation, maskSortOrder, null,
                (self) => rowIndex + DeckUpStartIndex);

            */

            // 图标动画
            view.AddSymbolEffect(cel.column, cel.row, symbolName, config.IsWESymbolAnim);



            // 边框
            if (config.IsWEFrame)
            {
                view.AddBorderEffect(cel.column, cel.row);
            }

            // 整体变大特效
            if (config.IsWETwinkle)
                view.ShowTwinkleEffect(cel.column, cel.row);
            else if (config.IsWEBigger)
                view.ShowBiggerEffect(cel.column, cel.row);

        }


        // 是否显示线
        if (config.IsWEShowLine)
        {
            view.ShowPayLines(symbolWin);
        }


        // 事件
        if (eventType == SpinWinEvent.TotalWinLine)
        {
            onWinEvent?.Invoke(new EventData<SymbolWin>(SlotMachineEvent.TotalWinLine, symbolWin));
        }
        else if (eventType == SpinWinEvent.SingleWinLine)
        {
            onWinEvent?.Invoke(new EventData<SymbolWin>(SlotMachineEvent.SingleWinLine, symbolWin));
        }

        yield return SlotWaitForSeconds(config.WETimeS);
    }



    public IEnumerator ShowWinListBySetting(List<SymbolWin> winList)
    {

        // 立马停止时，不播放赢分环节？
        if (isStopImmediately && config.IsWESkipAtStopImmediately)
            yield break;

        if (config.IsWETotalWinLine)
        {
            yield return ShowSymbolWinBySetting(GetTotalSymbolWin(winList), true, SpinWinEvent.TotalWinLine);
        }
        else
        {
            int idx = 0;
            while (idx < winList.Count)
            {
                yield return ShowSymbolWinBySetting(winList[idx], true, SpinWinEvent.SingleWinLine);

                ++idx;

                // 立马停止时，不播放赢分环节？
                if (isStopImmediately && config.IsWESkipAtStopImmediately)
                    break;
            }
        }

        //关闭遮罩
        CloseSlotCover();

        //停止特效显示
        SkipWinLine(false);
    }



    public void ShowSymbolWinDeck(List<BonusWin> symbolWin, bool isUseMySelfSymbolNumber)
    {
        //停止特效显示
        SkipWinLine(false);

        //显示遮罩
        SetSlotCover(config.IsWEShowCover);

        Dictionary<string, string> symbolHitEffect = config.GetSymbolHitEffect();

        foreach (BonusWin item in symbolWin)
        {
            Cell cel = item.cell;

            int symbolNumberSelf = view.GetVisibleSymbolNumberFromDeck(cel.column, cel.row);

            int symbolNumber = isUseMySelfSymbolNumber ? symbolNumberSelf : item.symbolNumber;

            string symbolName = symbolHitEffect[$"{symbolNumber}"];  // wild  or symbol;

            view.AddSymbolEffect(cel.column, cel.row, symbolName, config.IsWESymbolAnim);

            /*
            // 图标动画  
            GComponent goSymbolHit = fguiPoolHelper.GetObject(TagPoolObject.SymbolHit, symbolName).asCom;
            GComponent goSymbol = GetVisibleSymbolFromDeck(cel.column, cel.row);

            AddSymbolEffect(goSymbol, goSymbolHit, IsWESymbolAnim);


            int rowIndex = cel.row;
            // 设置层级
            FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goExpectation, maskSortOrder, null,
                (self) => rowIndex + DeckUpStartIndex);
            */

            // 边框
            if (config.IsWEFrame)
            {
                //GComponent goBorderEffect = fguiPoolHelper.GetObject(TagPoolObject.SymbolBorder, BorderEffect).asCom;
                //AddBorderEffect(goSymbol, goBorderEffect);
                view.AddBorderEffect(cel.column, cel.row);
            }

            // 整体变大特效
            if (config.IsWETwinkle)
                view.ShowTwinkleEffect(cel.column, cel.row);
            else if (config.IsWEBigger)
                view.ShowBiggerEffect(cel.column, cel.row);

        }
    }



}
