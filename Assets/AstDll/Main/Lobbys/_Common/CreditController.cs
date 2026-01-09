using FairyGUI;
using SlotMaker;
using GameMaker;


public class CreditController  
{

    public void Enable()
    {
        EventCenter.Instance.AddEventListener<EventData>(MetaUIEvent.ON_CREDIT_EVENT,OnCreditEvent);

    }
    public void Disable()
    {
        EventCenter.Instance.AddEventListener<EventData>(MetaUIEvent.ON_CREDIT_EVENT,OnCreditEvent);
    }

    GTextField gtxtCredit;
    public void InitParam(GTextField v)
    {
        gtxtCredit = v;
        gtxtCredit.text = SBoxModel.Instance.myCredit.ToString();
    }

    void OnCreditEvent(EventData res)
    {
        if (res.name == MetaUIEvent.UpdateNaviCredit)
        {
            UpdateNaviCredit data = res.value as UpdateNaviCredit;

            if (data.isAnim)
            {
                DoAddToCredit(data.toCredit, data.fromCredit);
            }
            else {
                DoSetCredit(data.toCredit);
            }

        }
    }



    enum StepAddCredit
    {
        Init = 0,
        Adding = 1,
    }

    long fromCredit;
    long toCredit;
    long curCredit;

    const long NONE = -99999;

    StepAddCredit stepAddCredit;
    void AddToCreditTask(object pam)
    {
        switch (stepAddCredit)
        {
            case StepAddCredit.Init:
                {
                    if(fromCredit != NONE)
                        curCredit = fromCredit;
                    stepAddCredit = StepAddCredit.Init;

                    AddToCreditTask(null);

                    Timers.inst.Add(2f, 1, ToFinishCredtiTask); // 加钱动画最多2秒
                }
                break;
            case StepAddCredit.Adding:
                {
                    curCredit += 5;
                    if (curCredit >= fromCredit)
                    {
                        curCredit = fromCredit;
                        Timers.inst.Remove(ToFinishCredtiTask); // 加钱动画最多2秒 
                    }
                    else
                    {
                        Timers.inst.Add(0.1f, 1, AddToCreditTask);
                    }
                    gtxtCredit.text = $"{curCredit}";
                }
                break;
        }
    }

    void ToFinishCredtiTask(object pam)
    {
        Timers.inst.Remove(AddToCreditTask);
        this.fromCredit = NONE;
        this.curCredit = toCredit;
        gtxtCredit.text = $"{curCredit}";
    }


    void DoAddToCredit(long toCredit, long fromCredit)
    {
        Timers.inst.Remove(AddToCreditTask);
        this.fromCredit = fromCredit;
        this.toCredit = toCredit;
        stepAddCredit = StepAddCredit.Init;
        AddToCreditTask(null);
    }

    void DoSetCredit(long toCredit)
    {
        Timers.inst.Remove(ToFinishCredtiTask);
        Timers.inst.Remove(AddToCreditTask);
        this.fromCredit = NONE;
        this.toCredit = toCredit;
        this.curCredit = toCredit;
        gtxtCredit.text = $"{curCredit}";
    }

}
