using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class FguiSettingsController :Singleton<FguiSettingsController>
{
 
    public void Enable()
    {
        // ==== 核心：最高优先级保证这个监听最先执行，就是「点击前」的逻辑
        // Stage.inst.onTouchEnd.Add(OnGlobalBtnClickBefore);


        // ==== 设置全局按钮声音
        ResourceManager02.Instance.LoadAsset<AudioClip>("Assets/AstBundle/_Commons/Game Maker/Sounds/UI_Button_Normal.wav", (res) =>
        {
            UIConfig.buttonSound = new NAudioClip(res);
        });

    }


    public void Disable()
    {
        Stage.inst.onTouchEnd.Remove(OnGlobalBtnClickBefore);

    }


    void OnGlobalBtnClickBefore(EventContext evt)
    {
        GObject clickTarget = evt.sender as GObject;
        if (clickTarget != null)
        {
            DebugUtils.Log($"点击了： {clickTarget.name} ");
        }
        /*
        // 1. 获取当前点击的目标UI组件（精准定位点击的是谁）
        GObject clickTarget = evt.sender as GObject;
        if (clickTarget == null) return;

        // 2. 核心判断：当前点击的是不是【按钮组件(Button)】
        // is Button 精准匹配，只处理按钮，不处理其他组件(图片、文本、列表等)
        if (clickTarget is Button clickBtn)
        {
            // ================ 这里写你所有按钮的【点击前逻辑】 ================
            Debug.Log($"按钮[{clickBtn.name}] 点击前执行，按钮地址：{clickBtn}");

            // ✅ 常用场景1：按钮防重复点击（比如1秒内只能点一次）
            if (clickBtn.enabled == false)
            {
                evt.StopPropagation(); // 阻止事件继续传递，按钮的onClick不会触发
                return;
            }

            // ✅ 常用场景2：权限校验，无权限则拦截点击
            bool hasPermission = CheckBtnPermission(clickBtn);
            if (!hasPermission)
            {
                evt.StopPropagation(); // 拦截事件，点击失效
                Debug.Log($"按钮[{clickBtn.name}] 无权限，点击被拦截");
                return;
            }

            // ✅ 常用场景3：全局埋点，统计所有按钮的点击行为（点击前上报）
            UploadBtnClickLog(clickBtn.name, clickBtn.parent.name);

            // ✅ 常用场景4：按钮置灰/冷却（执行完前置逻辑后）
            clickBtn.enabled = false;
            Invoke(() => { clickBtn.enabled = true; }, 1f);

            // 如果需要放行，什么都不用做，事件会自动传递到按钮的onClick回调
        }
        */
    }
}
