using FairyGUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public class TabAdminDisplayPermissions : MonoBehaviour
{
    GComponent owner;

    GList glst;

    JArray curDisplayPermissions;

    GButton btnReset;
    public void InitParam(GComponent comp)
    {
        owner = comp;

        glst = owner.GetChild("lst").asList;
        btnReset = owner.GetChild("btnReset").asCom.GetChild("btn").asButton;
        btnReset.onClick.Clear();
        btnReset.onClick.Add(OnResetPermissions);

        SetUIContent();


    }

    public int pageCount => 1;
    public int curPageIndex => 0;


    #region 待实现
    public void OnClickNextPage()
    {

    }

    public void OnClickPrevPage()
    {

    }

    public event Action<int,int> onPageChange;

    #endregion


    void SetUIContent()
    {
        curDisplayPermissions = JArray.Parse(SBoxModel.Instance.consoleDisplayPermissions.ToString());

        foreach (JToken node1 in curDisplayPermissions)
        {
            JToken item = node1;
            string chldName = item["key"].Value<string>();
            GComponent targetCom = glst.GetChild(chldName)?.asCom;

            if (targetCom != null)
            {
                string nodeName = ApplicationSettings.Instance.isRelease ? "release_has" : "debug_has";
                if (!item[nodeName].Value<bool>())
                {
                    glst.RemoveChild(targetCom);
                }
                else
                {
                    GButton btnStaff = targetCom.GetChild("staff").asButton;
                    SetToggle(btnStaff, item["staff_display"].Value<bool>(), item["display_permissions_change"].Value<bool>());
                    btnStaff.onChanged.Clear();
                    btnStaff.onChanged.Add(() =>
                    {
                        item["staff_display"] = btnStaff.selected;
                    });

                    GButton btnManager = targetCom.GetChild("manager").asButton;
                    SetToggle(btnManager, item["manager_display"].Value<bool>(), item["display_permissions_change"].Value<bool>());
                    btnManager.onChanged.Clear();
                    btnManager.onChanged.Add(() =>
                    {
                        item["manager_display"] = btnManager.selected;
                    });

                    GButton btnAdmin = targetCom.GetChild("admin").asButton;
                    SetToggle(btnAdmin, item["admin_display"].Value<bool>(), item["display_permissions_change"].Value<bool>());
                    btnAdmin.onChanged.Clear();
                    btnAdmin.onChanged.Add(() =>
                    {
                        item["admin_display"] = btnAdmin.selected;
                    });
                }
            }
        }


        onPageChange?.Invoke(curPageIndex,pageCount);
    }

    void  OnResetPermissions(){

        CommonPopupHandler.Instance.OpenPopupSingle(new CommonPopupInfo()
        {
            text = I18nMgr.T("Reset Console Display Permissions?"),
            type = CommonPopupType.YesNo,
            buttonText1 = I18nMgr.T("OK"),
            buttonAutoClose1 = true,
            callback1 = () =>
            {
                SBoxModel.Instance.consoleDisplayPermissions =
                    JArray.Parse(SBoxModel.Instance.consoleDisplayPermissionsDefaultStr);
                SetUIContent();
            },
            buttonText2 = I18nMgr.T("Cancel"),
            buttonAutoClose2 = true,
            isUseXButton = false,
            //mark = MARK_POP_BILLER_NOT_LINK,
        });
    }


    public void Disable()
    {
        if (curDisplayPermissions != null)
        {
            string curStr = JsonConvert.SerializeObject(curDisplayPermissions);
            DebugUtils.Log(curStr);
            if (curStr != JsonConvert.SerializeObject(SBoxModel.Instance.consoleDisplayPermissions))
            {
                SBoxModel.Instance.consoleDisplayPermissions = JArray.Parse(curDisplayPermissions.ToString()); 
            }
        }
    }


    void SetToggle(GButton btn, bool isSelected, bool isActive)
    {
        btn.selected = isSelected;
        btn.touchable = isActive;
        btn.GetChild("untouchable").visible = !isActive;
    }

}
