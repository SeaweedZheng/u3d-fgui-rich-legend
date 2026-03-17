using FairyGUI;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineViewBase : IVSlotMachine
{

    GComponent ui;



    protected SlotMachineConfig smConfig;

    GComponent goSotCover, goReels, goPayLines;

    private List<List<GComponent>> goSymbolsColRow;
    List<GComponent> anchorSymbolsLst;


    List<GTweener> reelTweenLst;


    public List<List<int>>  DeckColRowNumber => deckColRowNumber;

    public event Action<int> onSymbolPointerEnter;
    public event Action<int> onSymbolPointerExit;


    FguiGameObjectPoolHelper fguiPoolHelper;


    /// <summary>
    /// 图标动画显示的层级
    /// </summary>
    protected GComponent goAnchorSymbolEffect;

    protected string maskSortOrder = null;




    public virtual void Enable()
    {

    }

    public virtual void Disable()
    {

    }


    public virtual void InitParam(GComponent u, GComponent gAnchorSymbolEffect, SlotMachineConfig cf)
    {
        if (string.IsNullOrEmpty(maskSortOrder))
            maskSortOrder = $"MASK_SORT_ORDER-{Time.unscaledTime}-{UnityEngine.Random.Range(0, 100)}";


        smConfig = cf;
        ui = u;
        this.goAnchorSymbolEffect = gAnchorSymbolEffect;

        goSotCover = ui.GetChild("slotCover").asCom;
        goPayLines = ui.GetChild("payLines").asCom;
        goReels = ui.GetChild("reels").asCom;




        reelTweenLst = new List<GTweener>();
        anchorSymbolsLst = new List<GComponent>();
        for (int i = 0; i < smConfig.Column; i++)
        {
            GComponent goReel = goReels.GetChild($"reel{i + 1}").asCom;
            anchorSymbolsLst.Add(goReel.GetChild("symbols").asCom);

            reelTweenLst.Add(null);

            int index = i;
            goReel.onRollOver.Clear();
            goReel.onRollOver.Add(() => { onSymbolPointerEnter?.Invoke(index); });
            goReel.onRollOut.Clear();
            goReel.onRollOut.Add(() => { onSymbolPointerExit?.Invoke(index); });
        }

        InitDeck();
    }



    public void InitDeck() {

        deckColRowNumber = new List<List<int>>();
        goSymbolsColRow = new List<List<GComponent>>();
        for (int c = 0; c < anchorSymbolsLst.Count; c++)
        {
            List<GComponent> reelSymbols = new List<GComponent>();
            List<int> reelDeck = new List<int>();
            for (int r = 0; r < anchorSymbolsLst[c].numChildren; r++)
            {
                GComponent goSymbol = anchorSymbolsLst[c].GetChildAt(r).asCom;
                reelSymbols.Add(goSymbol);
                int symbolNumber = smConfig.SymbolNumbers[UnityEngine.Random.Range(1, smConfig.SymbolCount)];

                SetSymbolIcon(goSymbol, symbolNumber);
                reelDeck.Add(symbolNumber);
            }
            goSymbolsColRow.Add(reelSymbols);
            deckColRowNumber.Add(reelDeck);
        }

    }
    /// <summary>
    /// 可见和不可见的图标面板
    /// </summary>
    /// <remarks>
    /// * 当前实时面板结果
    /// * 不一定是滚轮停止时的最终结果
    /// </remarks>
    List<List<int>> deckColRowNumber;

    /// <summary>
    /// 滚轮停止时的最终结果
    /// </summary>
    //List<List<int>> reelsResult;


    protected virtual void SetSymbolIcon(GComponent symbol, int symbolNumber)
    {
        string symbolUrl = smConfig.GetSymbolUrl(symbolNumber);
        //DebugUtils.Log($"symbolUrl = {symbolUrl}");
        symbol.GetChild("animator").asCom.GetChild("image").asLoader.url = symbolUrl;
    }

    public void CloseAllPayLines() {

        GObject[] chds = goPayLines.GetChildren();
        foreach (GObject ch in chds)
        {
            ch.visible = false;
        }
    }
    /*
    public void SetReelsDeck(string strDeckRowCol = "1,1,1,1,1#2,2,2,2,2#3,3,3,3,3") {

        //停止特效显示
        //SkipWinLine(false);

        reelsResult = SlotTool.GetDeckColRow02(strDeckRowCol);

        //这个还要判断特殊图标 如果有还需要改变滚轮滚的次数 还有特殊表现效果
        //模拟图标
        for (int col = 0; col < config.Column; col++)
        {
            SetReelDeck(col, reelsResult[col]);
        }
    }*/

    public void SetReelsDeck(List<List<int>> reelsResultColRowNumber)
    {
        //这个还要判断特殊图标 如果有还需要改变滚轮滚的次数 还有特殊表现效果
        //模拟图标
        for (int col = 0; col < smConfig.Column; col++)
        {
            SetReelDeck(col, reelsResultColRowNumber[col]);
        }
    }
    public void SetReelDeck(int reelIndex, List<int> reelResult)
    {
        for (int i = smConfig.DeckDownStartIndex; i <= smConfig.DeckDownEndIndex; i++)
        {
            int symbolNumber = deckColRowNumber[reelIndex][i - smConfig.Row];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
            // symbolList[i].SetBtnInteractableState(true);
        }
        //这里开始设置结果
        SetReelEndResult(reelIndex, reelResult);
        anchorSymbolsLst[reelIndex].y = 0;
    }


    /// <summary> 设置最终结果 </summary>
    public void SetReelEndResult(int reelIndex, List<int> reelResult)
    {
        for (int i = smConfig.DeckUpStartIndex; i <= smConfig.DeckUpEndIndex; i++)
        {
            //int symbolNumber = reelsResult[reelIndex][i - config.DeckUpStartIndex];
            int symbolNumber = reelResult[i - smConfig.DeckUpStartIndex];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
        }
    }

    // 去除层级
    public virtual void ReturnSortingOrder() {
        FguiSortingOrderManager.Instance.ReturnSortingOrder(maskSortOrder);
    }

    public virtual void ReturnSymbolEffectToPool(int colIndex, int rowIndex, string[] exclude) { 
    
        // Return fgui GameObject to pool
    }


    public  void HideBaseSymbolIcon(int colIndex, int rowIndex, bool isHide) {

        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        HideBaseSymbolIcon(goSymbol, isHide);
    }

    /// <summary>
    /// 子类重写
    /// </summary>
    /// <param name="goSymbol"></param>
    /// <param name="isHide"></param>
    public virtual void HideBaseSymbolIcon(GComponent goSymbol, bool isHide)
    {
        goSymbol.GetChild("animator").asCom.GetChild("image").visible = !isHide;
    }

    public virtual void ShowSymbolHitEffect(int colIndex, int rowIndex, string symbolName, bool isAmin = true)
    {
        GComponent goSymbolHit = fguiPoolHelper.GetObject(TagPoolObject.SymbolHit, symbolName).asCom;
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        AddSymbolEffect(goSymbol, goSymbolHit, isAmin);

        // 设置层级
        FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goAnchorSymbolEffect, maskSortOrder, null,
            (self) => rowIndex + smConfig.DeckUpStartIndex);
    }


    protected void AddSymbolEffect(GComponent goSymbol,   GComponent anchorSymbolEffect, bool isAmin = true)
    {
        
        //Animator animatorSpine = null;  //【待完成】  获取Spine的
        //if (animatorSpine != null)
        //{
        //    if (isAmin)
        //        animatorSpine.speed = 1f;  // 播放
        //    else
        //        animatorSpine.speed = 0f;  //暂停
        //}

        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(anchorSymbolEffect);
        anchorSymbolEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);

        // 是否隐藏原有图标
        if (smConfig.IsWEHideBaseSymbol)
        {
            HideBaseSymbolIcon(goSymbol, true);
        }

        // 播放动画
    }

  



    /// <summary> 特殊 Symbol Effect </summary>
    public void ShowSymbolAppearEffect(int reelIndex)
    {

        for (int i = smConfig.DeckUpStartIndex; i <= smConfig.DeckUpEndIndex; i++)
        {

            string symbolNumber = $"{deckColRowNumber[reelIndex][i]}";

            Dictionary<string, string> symbolAppearEffect = smConfig.GetSymbolAppearEffect();
            DebugUtils.LogError(symbolNumber);
            bool isHashSymbolAppearNumber = symbolAppearEffect.ContainsKey(symbolNumber);

            if (isHashSymbolAppearNumber)
            {
                string symbolName = symbolAppearEffect[symbolNumber];
                GComponent anchorSymbolEffect = fguiPoolHelper.GetObject(TagPoolObject.SymbolAppear, symbolName).asCom;
                GComponent goSymbol = goSymbolsColRow[reelIndex][i].asCom;
                AddSymbolEffect(goSymbol, anchorSymbolEffect);

                //FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goExpectation, maskSortOrder); 

                int rowIndex = i;
                // 设置层级
                FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goAnchorSymbolEffect, maskSortOrder, null,
                    (self) => rowIndex + smConfig.DeckUpStartIndex);
            }
        }
    }


    public virtual void ShowBorderEffect(int colIndex, int rowIndex)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        GComponent gAnchorBorderEffect = fguiPoolHelper.GetObject(TagPoolObject.SymbolBorder, smConfig.BorderEffect).asCom;
        //AddBorderEffect(goSymbol, gAnchorBorderEffect);

        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(gAnchorBorderEffect);  //边长为1的点
        gAnchorBorderEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);
    }
    /*
    protected void AddBorderEffect(GComponent goSymbol, GComponent gAnchorBorderEffect)
    {
        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(gAnchorBorderEffect);  //边长为1的点
        gAnchorBorderEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);
        // 播放动画
    }*/

    public virtual void RemoveBorderEffect(int colIndex, int rowIndex)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        // 待完成
    }


    public void SetSlotCover(bool isShow) => goSotCover.visible = isShow;


    /// <summary> 回弹效果（这里方向相反）</summary>
    public IEnumerator Rebound(int reelIndex, float yTo = 80, float durationS = 0.05f)
    {
        bool isNext = false;

        reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], yTo, durationS, EaseType.Linear, () =>
        {
            reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], 0, durationS, EaseType.Linear, () =>
            {
                isNext = true;
            });
        });

        yield return new WaitUntil(() => isNext == true);
        isNext = false;
        reelTweenLst[reelIndex] = null;
    }



    public void ClearTween(int reelIndex)
    {
        if (reelTweenLst[reelIndex] != null)
            reelTweenLst[reelIndex].Kill();  //GTween.Kill(reelTweenLst[reelIndex]);
        reelTweenLst[reelIndex] = null;
    }


    /// <summary> 修改滚轮图标 </summary>
    public void ResetIconData(int reelIndex)
    {
        for (int i = smConfig.DeckDownStartIndex; i <= smConfig.DeckDownEndIndex; i++)
        {
            int symbolNumber = deckColRowNumber[reelIndex][i - smConfig.Row];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
            //symbolList[i].SetBtnInteractableState(true);
        }

        anchorSymbolsLst[reelIndex].y = - smConfig.ReelMaxOffsetY;  // 拉上去 (这里的方向和ugui是相反的)

        for (int i = smConfig.DeckUpStartIndex; i <= smConfig.DeckUpEndIndex; i++)
        {
            int symbolNumber = smConfig.SymbolNumbers[UnityEngine.Random.Range(0, smConfig.SymbolCount)];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
            //symbolList[i].SetBtnInteractableState(true);
        }
    }





    public int GetVisibleSymbolNumberFromDeck(int colIndex, int rowIndex)
    {
        return deckColRowNumber[colIndex][rowIndex + smConfig.DeckUpStartIndex];
    }

    public void MoveY(int reelIndex,float yTo  , float duration, Action onFinish)
    {
        reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], yTo,
                duration, EaseType.Linear, () => { onFinish?.Invoke(); });
    }




    private Dictionary<GComponent, Transition> transitionBiggerLst = new Dictionary<GComponent, Transition>();
    private Dictionary<GComponent, Transition> transitionTwinkleLst = new Dictionary<GComponent, Transition>();





    public virtual void ShowBiggerEffect(int colIndex, int rowIndex)
    {

        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        if (!transitionBiggerLst.ContainsKey(goSymbol))
            transitionBiggerLst.Add(goSymbol, null);
        transitionBiggerLst[goSymbol] = goSymbol.GetTransition("animBigger");
        transitionBiggerLst[goSymbol].Play();
    }



    public virtual void ShowTwinkleEffect(int colIndex, int rowIndex)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);

        if (!transitionTwinkleLst.ContainsKey(goSymbol))
            transitionTwinkleLst.Add(goSymbol, null);
        transitionTwinkleLst[goSymbol] = goSymbol.GetTransition("animTwinkle");
        transitionTwinkleLst[goSymbol].Play();
    }



    public void ShowPayLines(SymbolWin symbolWin)
    {
        if (symbolWin is TotalSymbolWin)
        {
            TotalSymbolWin totalSymbolWin = symbolWin as TotalSymbolWin;

            foreach (int payLineNumber in totalSymbolWin.lineNumbers)
            {
                int payLineIndex = GetPayLineIndex(payLineNumber);
                if (payLineIndex >= 0 && payLineIndex < goPayLines.numChildren)
                {
                    goPayLines.GetChildAt(payLineIndex).visible = true;
                }
            }
        }
        else
        {
            int paylineIndex = GetPayLineIndex(symbolWin.lineNumber);
            if (paylineIndex >= 0 && paylineIndex < goPayLines.numChildren)
            {
                goPayLines.GetChildAt(paylineIndex).visible = true;
            }
        }
    }

    public void ShowPayLines(List<int> lineNumbers )
    {
        foreach (int number in lineNumbers)
        {
            int paylineIndex = GetPayLineIndex(number);
            if (paylineIndex >= 0 && paylineIndex < goPayLines.numChildren)
            {
                goPayLines.GetChildAt(paylineIndex).visible = true;
            }
        }
    }



    public int GetPayLineIndex(int payLineNumber) => payLineNumber - 1;

    protected GComponent GetVisibleSymbolFromDeck(int colIndex, int rowIndex)
    {
        return goSymbolsColRow[colIndex][rowIndex + smConfig.DeckUpStartIndex];
    }


    public void RemoveSymbolTwinkleEffect(int colIndex, int rowIndex)
    {

        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);

        if (transitionTwinkleLst.ContainsKey(goSymbol) && transitionTwinkleLst[goSymbol] != null)
            transitionTwinkleLst[goSymbol].Stop();
        transitionTwinkleLst[goSymbol] = null;

    }



    public void RemoveSymbolBiggerEffect(int colIndex, int rowIndex)
    {

        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);

        if (transitionBiggerLst.ContainsKey(goSymbol) && transitionBiggerLst[goSymbol] != null)
            transitionBiggerLst[goSymbol].Stop();
        transitionBiggerLst[goSymbol] = null;
    }




    public void ShowSymbolEffects(List<Cell> cells, SymbolEffectType effectType, bool isAnim, int? toSymbolNumber = null)
    {
        Dictionary<string, string> symbolEffect = null;

        switch (effectType)
        {
            case SymbolEffectType.SymbolHit:
                symbolEffect = smConfig.GetSymbolHitEffect();
                break;
            case SymbolEffectType.SymbolAppear:
                symbolEffect = smConfig.GetSymbolAppearEffect();
                break;
        }

        foreach (Cell cel in  cells)
        {
            int symbolNumber = deckColRowNumber[cel.columnIndex][cel.rowIndex + smConfig.DeckUpStartIndex];

            int targetSymbolNumber = toSymbolNumber != null ? (int)toSymbolNumber : symbolNumber;
            string effectName = symbolEffect[$"{targetSymbolNumber}"];  // wild  or symbol;
            ShowSymbolHitEffect(cel.columnIndex, cel.rowIndex, effectName, isAnim);
        }
    }

    public void ShowSymbolEffects(List<int> symbolNumbers, SymbolEffectType effectType, bool isAnim, int? toSymbolNumber = null)
    {

        Dictionary<string, string> symbolEffect = null;

        switch (effectType)
        {
            case SymbolEffectType.SymbolHit:
                symbolEffect = smConfig.GetSymbolHitEffect();
                break;
            case SymbolEffectType.SymbolAppear:
                symbolEffect = smConfig.GetSymbolAppearEffect();
                break;
        }



        for (int cIdx = 0; cIdx < smConfig.Column; cIdx++)
        {
            for (int rIdx = 0; rIdx < smConfig.Row; rIdx++)
            {
                //GComponent goSymbol = GetVisibleSymbolFromDeck(cIdx, rIdx);
                int symbolNumber = deckColRowNumber[cIdx][rIdx + smConfig.DeckUpStartIndex];
                if (symbolNumbers.Contains(symbolNumber))
                {
                    int targetSymbolNumber = toSymbolNumber != null ? (int)toSymbolNumber : symbolNumber;
                    string effectName = symbolEffect[$"{targetSymbolNumber}"];  // wild  or symbol;
                    ShowSymbolHitEffect(cIdx, rIdx, effectName, isAnim);
                }
            }
        }
    }


    public void ShowBiggerEffects(List<Cell> cells)
    {
        foreach (Cell cel in  cells)
        {
            ShowBiggerEffect(cel.columnIndex, cel.rowIndex);
        }

    }
    public void ShowTwinkleEffects(List<Cell> cells)
    {
        foreach (Cell cel in cells)
        {
            ShowTwinkleEffect(cel.columnIndex, cel.rowIndex);
        }
    }
    public void ShowBorderEffects(List<Cell> cells)
    {
        foreach (Cell cel in cells)
        {
            ShowBorderEffect(cel.columnIndex, cel.rowIndex);
        }
    }





    public List<Cell> GetSymbolCells(List<int> symbolNumbers)
    {
        List <Cell> cells = new List <Cell>();
        for (int cIdx = 0; cIdx < smConfig.Column; cIdx++)
        {
            for (int rIdx = 0; rIdx < smConfig.Row; rIdx++)
            {
                int symbolNumber = deckColRowNumber[cIdx][rIdx + smConfig.DeckUpStartIndex];
                if (symbolNumbers.Contains(symbolNumber))
                {
                    cells.Add(new Cell()
                    {
                        rowIndex = rIdx,
                        columnIndex = cIdx,
                    });
                }
            }
        }
        return cells;
    }


    public Vector2 GetVisibleSymbolCenterWordPos(int colIndex, int rowIndex)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);

        Vector2 centerlocalPos = new Vector2(goSymbol.width / 2, goSymbol.height / 2);

        Vector2 worldPos = goSymbol.LocalToGlobal(centerlocalPos);

        // Vector2 worldPos02 = new Vector2(worldPos.x - goSymbol.pivotX * goSymbol.width,  worldPos.y - goSymbol.pivotY * goSymbol.height); 

        return worldPos; // worldPos02
    }

    public Vector2 SymbolCenterToNodeLocalPos(int colIndex, int rowIndex, GComponent toNode)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);

        Vector2 centerlocalPos = new Vector2(goSymbol.width / 2, goSymbol.height / 2);

        Vector2 worldPos = goSymbol.LocalToGlobal(centerlocalPos);

        Vector2 localPos = toNode.GlobalToLocal(worldPos);

        return new Vector2(localPos.x - goSymbol.pivotX * goSymbol.width,
            localPos.y - goSymbol.pivotY * goSymbol.height);
    }



}
