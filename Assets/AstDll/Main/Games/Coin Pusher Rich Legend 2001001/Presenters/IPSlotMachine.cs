using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using GameMaker;
public interface IPSlotMachine 
{
    event Action<EventData> onWinEvent;
    event Action<EventData> onSlotDetailEvent;
    event Action<EventData> onSlotEvent;
    event Action<EventData> onPrepareTotalWinCreditEvent;
    event Action<EventData> onTotalWinCreditEvent;
}
