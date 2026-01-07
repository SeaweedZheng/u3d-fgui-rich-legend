using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
public interface IVTable 
{
    int curPageIndex
    {
        get;
    }

    int pageCount
    {
        get;
    }

    void OnClickPrev();
    void OnClickNext();

    event Action<int, int> onChangeNavBottomTitle;
}