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
    public class TemplateUIGameRecordSlot001
    {
        public const string templateUrl = "ui://ConsoleSlot98000000/TemplateUIGameRecordSlot001";

        int totalPageCount;
        int curPageNumber;
        int gameId;
        public void InitParam(GLoader gldTemplate, TableGameRecordItem data, int totalPageCount, int curPageNumber)
        {
            this.gameId = (int)data.game_id;
            this.totalPageCount = totalPageCount;
            this.curPageNumber = curPageNumber;

            if (!string.IsNullOrEmpty(data.template_name))
            {
                gldTemplate.url = templateUrl;
                GComponent goTemplate = gldTemplate.component;


                switch (data.template_name)
                {
                    case nameof(TemplateDataGameRecordSlot1):
                        {
                            TemplateDataGameRecordSlot1 item = JsonConvert.DeserializeObject<TemplateDataGameRecordSlot1>(data.template_data);

                            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.creatAt);
                            DateTime localDateTime = dateTimeOffset.LocalDateTime;
                            string dateStr = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");


                            Dictionary<string, string> kvs = new Dictionary<string, string>()
                            {
                                [I18nMgr.T("Game Number:")] = item.gameUid,
                                [I18nMgr.T("Game Date:")] = dateStr,

                            };
                        }
                        break;
                    case nameof(TemplateDataGameRecordCoinPusher1):
                        {
                            TemplateDataGameRecordCoinPusher1 item = JsonConvert.DeserializeObject<TemplateDataGameRecordCoinPusher1>(data.template_data);

                            // 画面
                            SetRecordDeck(goTemplate.GetChild("deck").asList, item.deckRowCol);

                            SetRecordTitle(goTemplate.GetChild("title").asRichTextField);


                            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(item.creatAt);
                            DateTime localDateTime = dateTimeOffset.LocalDateTime;
                            string dateStr = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                            Dictionary<string, string> kvs = new Dictionary<string, string>();
                            kvs.Add(I18nMgr.T("Game Number:"), item.gameUid);
                            kvs.Add(I18nMgr.T("Game Date:"), dateStr);
                            kvs.Add(I18nMgr.T("Credit Per Coin:"), item.coinPerCredit.ToString());
                            kvs.Add(I18nMgr.T("Base Win Coins:"), item.baseGameWinCoins.ToString());
                            if (item.jackpotWinCoins > 0)
                                kvs.Add(string.Format(I18nMgr.T("{0} Win Coins:"), I18nMgr.T(item.jackpotType)), item.jackpotWinCoins.ToString());
                            if (item.bonusGameWinCoins > 0)
                                kvs.Add(string.Format(I18nMgr.T("{0} Win Coins:"), I18nMgr.T(item.bonusType)), item.bonusGameWinCoins.ToString());
                            if (item.isFreeSpin)
                            {
                                kvs.Add(I18nMgr.T("Free Spin Total Count:"), item.freeSpinTotalCount.ToString());
                                kvs.Add(I18nMgr.T("Free Spin Add Count:"), item.freeSpinAddCount.ToString());
                                kvs.Add(I18nMgr.T("Free Spin Current Number:"), item.freeSpinCurNumber.ToString());
                            }
                            SetRecordKVs(goTemplate.GetChild("kvs").asList, kvs);

                            SetRecordDetail(goTemplate.GetChild("detail").asRichTextField, item.detail, item.args);
                        }
                        break;
                    default:
                        //DebugUtils.LogError($"cant not find template of {clsName}");
                        break;
                }
            }

        }

        void SetRecordDeck(GList glstDeck, string deckRowCol)
        {
            //List<List<int>>   deckRowColDic = SlotTool.GetDeckRCdByRCs(deckRowCol);

            List<int> deckRowColLst = SlotTool.GetDeckRClByRCs(deckRowCol);

            string[] rows = deckRowCol.Split('#');
            int rowNum = rows.Length;
            int colNum = rows[0].Split(',').Length;

            glstDeck.columnCount = colNum;



            glstDeck.itemRenderer = (int index, GObject obj) =>
            {

                GLoader icon = obj.asCom.GetChild("icon").asLoader;
                icon.url = "ui://ConsoleSlot98000000/SymbolBroken";
                //icon.component;

                string assetPth = ConfigUtils.GetSlotSymbolAssetPth(gameId, deckRowColLst[index]);
                //string assetPth = $"Assets/AstBackup/Consoles/Game Info/G{gameId}/Symbols/symbol{deckRowColLst[index]}.png";

                string pth = "";

                if (Application.isEditor)
                {
                    pth = PathHelper.GetAssetBackupSAPTH(assetPth);
                }
                else
                {
                    string localPth = PathHelper.GetAssetBackupLOCPTH(assetPth);
                    if (File.Exists(localPth))
                    {
                        pth = localPth;
                    }
                    else
                    {
                        pth = PathHelper.GetAssetBackupWEBURL(assetPth);
                    }
                }

                //加载
                FileLoaderManager.Instance.LoadImageAsTexture(pth, (Texture2D texture) =>
                {
                    NTexture nTexture = new NTexture(texture);
                    icon.texture = nTexture;
                    //icon.fill = FillType.Scale;                                  
                    icon.fill = FillType.ScaleFree;      // 等比缩放，可能留白                                       
                                                         //icon.fill = FillType.ScaleNoBorder;  // 等比缩放，完全填充（可能裁剪）
                });

            };
            glstDeck.numItems = deckRowColLst.Count;
        }
        void SetRecordTitle(GRichTextField txtTitle)
        {
            string gameName = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId, "game_name");

            string result = string.Format(I18nMgr.T("{0} Game Record {1} of {2}"), I18nMgr.T(gameName), curPageNumber, totalPageCount);

            txtTitle.text = result;
        }

        void SetRecordKVs(GList glstKvs, Dictionary<string, string> kvs)
        {
            glstKvs.itemRenderer = (int index, GObject obj) =>
            {
                KeyValuePair<string, string> kv = kvs.ElementAt(index);
                obj.asCom.GetChild("key").asRichTextField.text = kv.Key;
                obj.asCom.GetChild("value").asRichTextField.text = kv.Value;
            };
            glstKvs.numItems = kvs.Count;
        }

        void SetRecordDetail(GRichTextField txtDetail, string detail, string args)
        {
            txtDetail.text = GetRecordDetail(detail, args);
        }

        string GetRecordDetail(string detail, string args)
        {
            string[] detailLst = detail.Split(new[] { "##" }, StringSplitOptions.None);
            string[] argsLst = args.Split(new[] { "##" }, StringSplitOptions.None);


            string result = "";

            for (int i = 0; i < detailLst.Length; i++)
            {
                if (i > 0)
                    result += "\n";

                string[] paramsLst02 = argsLst[i].Split(',');
                var datas = new List<object>();

                for (int j = 0; j < paramsLst02.Length; j++)
                {
                    //datas.Add(I18nMgr.T(paramsLst02[j]));
                    datas.Add(paramsLst02[j]);
                }
                try
                {
                    result += string.Format(I18nMgr.T(detailLst[i]), datas.ToArray());
                }
                catch
                {
                    result = $"detail: {detail} -- args: {args}";
                    break;
                }
            }


            return result;
        }
    }


}