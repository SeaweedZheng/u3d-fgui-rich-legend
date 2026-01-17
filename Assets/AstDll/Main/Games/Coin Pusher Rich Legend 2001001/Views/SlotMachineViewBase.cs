using FairyGUI;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineViewBase : IVSlotMachine
{

    GComponent ui;



    SlotMachineConfig config;

    GComponent goSotCover, goReels, goPayLines;

    private List<List<GComponent>> goSymbolsColRow;
    List<GComponent> anchorSymbolsLst;


    List<GTweener> reelTweenLst;



    public event Action<int> onSymbolPointerEnter;
    public event Action<int> onSymbolPointerExit;


    FguiPoolHelper fguiPoolHelper;

    GComponent goExpectation;

    string maskSortOrder = null;
    public virtual void InitParam(GComponent u, GComponent goExpectation, SlotMachineConfig cf)
    {
        if (string.IsNullOrEmpty(maskSortOrder))
            maskSortOrder = $"MASK_SORT_ORDER-{Time.unscaledTime}-{UnityEngine.Random.Range(0, 100)}";


        config = cf;
        ui = u;
        this.goExpectation = goExpectation;

        goSotCover = ui.GetChild("slotCover").asCom;
        goPayLines = ui.GetChild("playLines").asCom;
        goReels = ui.GetChild("reels").asCom;




        reelTweenLst = new List<GTweener>();
        anchorSymbolsLst = new List<GComponent>();
        for (int i = 0; i < config.Column; i++)
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
                int symbolNumber = config.SymbolNumbers[UnityEngine.Random.Range(1, config.SymbolCount)];

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
        string symbolUrl = config.GetSymbolUrl(symbolNumber);
        //DebugUtils.Log($"symbolUrl = {symbolUrl}");
        symbol.GetChild("animator").asCom.GetChild("image").asLoader.url = symbolUrl;
    }

    public void CloseAllPlayLines() {

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
        for (int col = 0; col < config.Column; col++)
        {
            SetReelDeck(col, reelsResultColRowNumber[col]);
        }
    }
    public void SetReelDeck(int reelIndex, List<int> reelResult)
    {
        for (int i = config.DeckDownStartIndex; i <= config.DeckDownEndIndex; i++)
        {
            int symbolNumber = deckColRowNumber[reelIndex][i - config.Row];
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
        for (int i = config.DeckUpStartIndex; i <= config.DeckUpEndIndex; i++)
        {
            //int symbolNumber = reelsResult[reelIndex][i - config.DeckUpStartIndex];
            int symbolNumber = reelResult[i - config.DeckUpStartIndex];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
        }
    }

    // 去除层级
    public void ReturnSortingOrder() {
        FguiSortingOrderManager.Instance.ReturnSortingOrder(maskSortOrder);
    }

    public void ReturnSymbolEffectToPool(int colIndex, int rowIndex, string[] exclude) { }


    public void HideBaseSymbolIcon(int colIndex, int rowIndex, bool isHide) {

        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        HideBaseSymbolIcon(goSymbol, isHide);
    }
    protected virtual void HideBaseSymbolIcon(GComponent goSymbol, bool isHide)
    {
        goSymbol.GetChild("animator").asCom.GetChild("image").visible = !isHide;
    }
    public void AddSymbolEffect(int colIndex, int rowIndex, string symbolName, bool isAmin = true)
    {
        GComponent goSymbolHit = fguiPoolHelper.GetObject(TagPoolObject.SymbolHit, symbolName).asCom;
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        AddSymbolEffect(goSymbol, goSymbolHit, isAmin);


        // 设置层级
        FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goExpectation, maskSortOrder, null,
            (self) => rowIndex + config.DeckUpStartIndex);
    }
     void AddSymbolEffect(GComponent goSymbol,   GComponent anchorSymbolEffect, bool isAmin = true)
    {
        
        /*
        Animator animatorSpine = null;  //【待完成】  获取Spine的
        if (animatorSpine != null)
        {
            if (isAmin)
                animatorSpine.speed = 1f;  // 播放
            else
                animatorSpine.speed = 0f;  //暂停
        }*/

        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(anchorSymbolEffect);
        anchorSymbolEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);

        // 是否隐藏原有图标
        if (config.IsWEHideBaseSymbol)
        {
            HideBaseSymbolIcon(goSymbol, true);
        }

        // 播放动画
    }


    public void AddBorderEffect(int colIndex, int rowIndex)
    {
        GComponent goSymbol = GetVisibleSymbolFromDeck(colIndex, rowIndex);
        GComponent goBorderEffect = fguiPoolHelper.GetObject(TagPoolObject.SymbolBorder, config.BorderEffect).asCom;
        _AddBorderEffect(goSymbol, goBorderEffect);
    }
    void _AddBorderEffect(GComponent goSymbol, GComponent anchorBorderEffect)
    {
        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(anchorBorderEffect);  //边长为1的点
        anchorBorderEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);
        // 播放动画
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
        for (int i = config.DeckDownStartIndex; i <= config.DeckDownEndIndex; i++)
        {
            int symbolNumber = deckColRowNumber[reelIndex][i - config.Row];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
            //symbolList[i].SetBtnInteractableState(true);
        }

        anchorSymbolsLst[reelIndex].y = - config.ReelMaxOffsetY;  // 拉上去 (这里的方向和ugui是相反的)

        for (int i = config.DeckUpStartIndex; i <= config.DeckUpEndIndex; i++)
        {
            int symbolNumber = config.SymbolNumbers[UnityEngine.Random.Range(0, config.SymbolCount)];
            SetSymbolIcon(goSymbolsColRow[reelIndex][i].asCom, symbolNumber);
            deckColRowNumber[reelIndex][i] = symbolNumber;
            //symbolList[i].SetBtnInteractableState(true);
        }
    }





    public int GetVisibleSymbolNumberFromDeck(int colIndex, int rowIndex)
    {
        return deckColRowNumber[colIndex][rowIndex + config.DeckUpStartIndex];
    }

    public void MoveY(int reelIndex,float yTo  , float duration, Action onFinish)
    {
        reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], yTo,
                duration, EaseType.Linear, () => { onFinish?.Invoke(); });
    }




    private Dictionary<GComponent, Transition> transitionBiggerLst = new Dictionary<GComponent, Transition>();
    private Dictionary<GComponent, Transition> transitionTwinkleLst = new Dictionary<GComponent, Transition>();


    /// <summary> 特殊 Symbol Effect </summary>
    public void SymbolAppearEffect(int reelIndex)
    {

        for (int i = config.DeckUpStartIndex; i <= config.DeckUpEndIndex; i++)
        {

            string symbolNumber = $"{deckColRowNumber[reelIndex][i]}";

            Dictionary<string, string> symbolAppearEffect = config.GetSymbolAppearEffect();
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
                FguiSortingOrderManager.Instance.ChangeSortingOrder(goSymbol, goExpectation, maskSortOrder, null,
                    (self) => rowIndex + config.DeckUpStartIndex);
            }
        }
    }


    public virtual GComponent AddBorderEffect(GComponent goSymbol, GComponent anchorBorderEffect)
    {
        GComponent goAnimator = goSymbol.GetChild("animator").asCom;
        goAnimator.AddChild(anchorBorderEffect);  //边长为1的点
        anchorBorderEffect.xy = new Vector2(goAnimator.width / 2, goAnimator.height / 2);
        // 播放动画
        return anchorBorderEffect;
    }

    public void ShowBiggerEffect(int colIndex, int rowIndex)
        => ShowBiggerEffect(GetVisibleSymbolFromDeck(colIndex, rowIndex));

    public void ShowBiggerEffect(GComponent goSymbol)
    {
        if (!transitionBiggerLst.ContainsKey(goSymbol))
            transitionBiggerLst.Add(goSymbol, null);
        transitionBiggerLst[goSymbol] = goSymbol.GetTransition("animBigger");
        transitionBiggerLst[goSymbol].Play();
    }


    public void ShowTwinkleEffect(int colIndex, int rowIndex)
        => ShowTwinkleEffect(GetVisibleSymbolFromDeck(colIndex, rowIndex));

    void ShowTwinkleEffect(GComponent goSymbol)
    {
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

    public int GetPayLineIndex(int payLineNumber) => payLineNumber - 1;

    GComponent GetVisibleSymbolFromDeck(int colIndex, int rowIndex)
    {
        return goSymbolsColRow[colIndex][rowIndex + config.DeckUpStartIndex];
    }


    public void StopSymbolEffect(int colIndex, int rowIndex) => 
        StopSymbolEffect(GetVisibleSymbolFromDeck(colIndex, rowIndex));

    void StopSymbolEffect(GComponent goSymbol)
    {
        if (transitionBiggerLst.ContainsKey(goSymbol) && transitionBiggerLst[goSymbol] != null)
            transitionBiggerLst[goSymbol].Stop();
        transitionBiggerLst[goSymbol] = null;

        if (transitionTwinkleLst.ContainsKey(goSymbol) && transitionTwinkleLst[goSymbol] != null)
            transitionTwinkleLst[goSymbol].Stop();
        transitionTwinkleLst[goSymbol] = null;
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
