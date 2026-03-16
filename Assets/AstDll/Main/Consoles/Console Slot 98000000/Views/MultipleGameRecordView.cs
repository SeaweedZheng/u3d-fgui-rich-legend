using FairyGUI;
using Newtonsoft.Json;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

namespace ConsoleSlot98000000
{
    public partial class MultipleGameRecordView : IVMultipleGameRecord
    {
        GComponent ui;

        GComponent goSearch;

        GLoader gldGameRecord;

        GRichTextField txtSearchValue;


        GTextField txtTest;
        public void InitParam(GComponent u)
        {
            ui = u;


            txtTest = ui.GetChild("test").asTextField;

            goSearch = ui.GetChild("search").asCom;
            txtSearchValue = goSearch.GetChild("value").asRichTextField;
            txtSearchValue.onClick.Clear();
            txtSearchValue.onClick.Add(OnShearchGameRecorf);
            txtSearchValue.text = $"#{I18nMgr.T("All")}";

            GComponent goDelete = goSearch.GetChild("delete").asCom;
            goDelete.onClick.Clear();
            goDelete.onClick.Add((context) =>
            {
                context.StopPropagation(); // 停止事件冒泡(不起作用)
                OnClickButtonDelete();
            });


            gldGameRecord = ui.GetChild("gameRecordTemplate").asLoader;
        }

        void OnClickButtonDelete()
        {
            DebugUtils.LogError($"【Test】:清除");
            SelectGameRecordFilterInfo newFilterInfo = new SelectGameRecordFilterInfo();

            if (JsonConvert.SerializeObject(newFilterInfo) !=
                JsonConvert.SerializeObject(_curSelectFilterInfo))
            {
                txtSearchValue.text = $"#{I18nMgr.T("All")}";
                _curSelectFilterInfo = newFilterInfo;
                SelectGameRecordPageInfo pageInfo = new SelectGameRecordPageInfo()
                {
                    totalCountPerPage = totalCountPerPage,
                    selectNumberPage = 1,
                };

                onSelectGameRecord(_curSelectFilterInfo, pageInfo);
            }
        }


        async void OnShearchGameRecorf()
        {
            DebugUtils.LogError($"【Test】:点击查找");
            List<InParamItemSelectOption> options = new List<InParamItemSelectOption>();
            InParamItemSelectOption op;

            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.gameTypes);
            op.selectName = I18nMgr.T("Game Type:");
            op.selectKey = _curSelectFilterInfo.selectedIndexGameType.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i = 0; i < totalGameFilterOptions.gameTypes.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T(totalGameFilterOptions.gameTypes[i])); //"id:200"
            }
            options.Add(op);


            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.gameIds);
            op.selectName = I18nMgr.T("Game ID:");
            op.selectKey = _curSelectFilterInfo.selectedIndexGameId.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i=0;i< totalGameFilterOptions.gameIds.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T($"{totalGameFilterOptions.gameIds[i]}") ); //"id:200"
            }
            options.Add(op);

            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.turnTypes);
            op.selectName = I18nMgr.T("Turn Type:");
            op.selectKey = _curSelectFilterInfo.selectedIndexTurnType.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i = 0; i < totalGameFilterOptions.turnTypes.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T(totalGameFilterOptions.turnTypes[i])); //"id:200"
            }
            options.Add(op);

            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.hitJackpotTypes);
            op.selectName = I18nMgr.T("Hit Jackpot Types:");
            op.selectKey = _curSelectFilterInfo.selectedIndexHitJackpotTypes.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i = 0; i < totalGameFilterOptions.hitJackpotTypes.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T(totalGameFilterOptions.hitJackpotTypes[i])); //"id:200"
            }
            options.Add(op);

            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.hitBonusTypes);
            op.selectName = I18nMgr.T("Hit Bonus Types:");
            op.selectKey = _curSelectFilterInfo.selectedIndexHitBonusTypes.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i = 0; i < totalGameFilterOptions.hitBonusTypes.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T(totalGameFilterOptions.hitBonusTypes[i])); //"id:200"
            }
            options.Add(op);

            op = new InParamItemSelectOption();
            op.selectType = nameof(totalGameFilterOptions.fullDates);
            op.selectName = I18nMgr.T("Date:");
            op.selectKey = _curSelectFilterInfo.selectedIndexDate.ToString();
            op.selectContent.Add("-1", I18nMgr.T("All"));
            for (int i = 0; i < totalGameFilterOptions.fullDates.Count; i++)
            {
                op.selectContent.Add(i.ToString(), I18nMgr.T(totalGameFilterOptions.fullDates[i])); //"id:200"
            }


            OutParamsBase res = await PageManager.Instance.OpenPageAsync(PageName.ConsoleSlot98000000PopupConsoleSearch,
                new InParamsPopupConsoleSearch()
                {
                    title = I18nMgr.T("Search Record"),
                    options = options,
                });
            if (res != null && res.code == 0)
            {
                string showFilterName = "";

                var result = res as OutParamsPopupConsoleSearch;

                SelectGameRecordFilterInfo filterInfo = new SelectGameRecordFilterInfo();

                foreach(var item in result.selectResult)
                {
                    switch (item.Key)
                    {
                        case nameof(totalGameFilterOptions.gameTypes):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexGameType = selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += I18nMgr.T(totalGameFilterOptions.gameTypes[selIndex]); 
                                }
                            }
                            break;
                        case nameof(totalGameFilterOptions.gameIds):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexGameId =  selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += "/";
                                    showFilterName += totalGameFilterOptions.gameIds[selIndex].ToString();
                                }
                            }
                            break;
                        case nameof(totalGameFilterOptions.turnTypes):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexTurnType = selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += "/";
                                    showFilterName += I18nMgr.T(totalGameFilterOptions.turnTypes[selIndex]);
                                }
                            }
                            break;
                        case nameof(totalGameFilterOptions.hitJackpotTypes):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexHitJackpotTypes = selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += "/";
                                    showFilterName += I18nMgr.T(totalGameFilterOptions.hitJackpotTypes[selIndex]);
                                }
                            }
 
                            break;
                        case nameof(totalGameFilterOptions.hitBonusTypes):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexHitBonusTypes = selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += "/";
                                    showFilterName += I18nMgr.T(totalGameFilterOptions.hitBonusTypes[selIndex]);
                                }
                            }

                            break;
                        case nameof(totalGameFilterOptions.fullDates):
                            {
                                int selIndex = int.Parse(item.Value);
                                filterInfo.selectedIndexDate = selIndex;
                                if (selIndex != -1)
                                {
                                    showFilterName += "/";
                                    showFilterName += totalGameFilterOptions.fullDates[selIndex];
                                }
                            }
                            break;
                    }
                }
                _curSelectFilterInfo = filterInfo;

                txtSearchValue.text = string.IsNullOrEmpty(showFilterName)? $"#{I18nMgr.T("All")}" : "#"+ showFilterName;

                SelectGameRecordPageInfo pageInfo = new SelectGameRecordPageInfo()
                {
                    totalCountPerPage = totalCountPerPage,
                    selectNumberPage = 1,
                };
                onSelectGameRecord(_curSelectFilterInfo, pageInfo);
            }

        }


        public event Action<SelectGameRecordFilterInfo, SelectGameRecordPageInfo> onSelectGameRecord;

        public event Action onClickNext;
        public event Action onClickPrev;

        public void ClearAll()
        {
            totalGameFilterOptions = null;
        }




        public SelectGameRecordFilterInfo curSelectFilterInfo { get => _curSelectFilterInfo; }
        SelectGameRecordFilterInfo _curSelectFilterInfo;

        TotalGameFilterOptions totalGameFilterOptions;
        public const int totalCountPerPage = 1;

        public SelectGameRecordPageInfo SetDefaultSelect()
        {
            _curSelectFilterInfo = new SelectGameRecordFilterInfo();
            return new SelectGameRecordPageInfo()
            {
                totalCountPerPage = totalCountPerPage,
                selectNumberPage = 1,
            };
        }


        public void SetTotalGameFilterOptions(TotalGameFilterOptions Filter)
        {
            totalGameFilterOptions = Filter;
        }


        SelectGameRecordPageResult curGameRecordPageResult;
        public void SetContent(SelectGameRecordPageResult content)
        {
            txtTest.text = "";
            if (content.pageItems.Count > 0)
            {
                TableGameRecordItem data = content.pageItems[0];

                txtTest.text = JsonConvert.SerializeObject(data); //测试数据显示

                if (!string.IsNullOrEmpty(data.template_name))
                {
                    switch (TemplateUIGameRecordUtils.GetUITemplateName(data.template_name))
                    {
                        case nameof(TemplateUIGameRecord001):
                            {
                                TemplateUIGameRecord001 templateCtrl =  new TemplateUIGameRecord001();
                                templateCtrl.InitParam(gldGameRecord, data, content.totalPageCount, content.selectNumberPage);
                            }
                            break;
                    }
                 }

            }


            curGameRecordPageResult = content;
            onChangeNavBottomTitle?.Invoke(curPageIndex, pageCount);
        }

    }




    public partial class MultipleGameRecordView : IVTable
    {

        public int curPageIndex
        {
            get => curGameRecordPageResult.selectNumberPage -1;
        }


        public int pageCount
        {
            get => curGameRecordPageResult.totalPageCount;
        }

        public void OnClickPrev()
        {
            onClickPrev?.Invoke();
        }
        public void OnClickNext()
        {
            onClickNext?.Invoke();
        }

        public event Action<int, int> onChangeNavBottomTitle;
    }







}