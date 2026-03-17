using ConsoleSlot98000000;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleSlot98000000
{
    public static class TemplateUIGameRecordUtils
    {

        public static string GetUITemplateName(string clsName)
        {
            switch (clsName)
            {
                case nameof(TemplateDataGameRecordSlot1):
                case nameof(TemplateDataGameRecordCoinPusher1):
                    return nameof(TemplateUIGameRecordSlot001);
                default:
                    DebugUtils.LogError($"cant not find template of {clsName}");
                    return clsName;
            }
        }
        /*
        public static string GetUITemplateUrl(string clsName)
        {
            switch (clsName)
            {
                case nameof(UITemplateGameRecordSlot1):
                case nameof(UITemplateGameRecordCoinPusher1):
                    return "ui://ConsoleSlot98000000/UITemplateGameRecord001";
                default:
                    DebugUtils.LogError($"cant not find template of {clsName}");
                    return clsName;
            }
        }
        */
    }
}