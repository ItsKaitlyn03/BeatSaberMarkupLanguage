﻿using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LevelPackTableCell = AnnotatedBeatmapLevelCollectionTableCell;//This got renamed at a point, but old name is more clear so I'm using that

namespace BeatSaberMarkupLanguage.Components
{
    public class CustomListTableData : MonoBehaviour, TableView.IDataSource
    {
        public enum ListStyle
        {
            List, Box, Simple
        }

        private ListStyle listStyle = ListStyle.List;

        private LevelListTableCell songListTableCellInstance;
        private LevelPackTableCell levelPackTableCellInstance;
        private SimpleTextTableCell simpleTextTableCellInstance;

        public List<CustomCellInfo> data = new List<CustomCellInfo>();
        public float cellSize = 8.5f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;

        public bool expandCell = false;

        public ListStyle Style
        {
            get => listStyle;
            set
            {
                //Sets the default cell size for certain styles
                switch (value)
                {
                    case ListStyle.List:
                        cellSize = 8.5f;
                        break;
                    case ListStyle.Box:
                        cellSize = tableView.tableType == TableView.TableType.Horizontal ? 30f : 35f;
                        break;
                    case ListStyle.Simple:
                        cellSize = 8f;
                        break;
                }

                listStyle = value;
            }
        }

        public LevelListTableCell GetTableCell()
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (songListTableCellInstance == null)
                    songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                tableCell = Instantiate(songListTableCellInstance);

            }

            //tableCell.SetField("_beatmapCharacteristicImages", new Image[0]);
            tableCell.SetField("_notOwned", false);

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public LevelPackTableCell GetLevelPackTableCell()
        {
            LevelPackTableCell tableCell = (LevelPackTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (levelPackTableCellInstance == null)
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<LevelPackTableCell>().First(x => x.name == "AnnotatedBeatmapLevelCollectionTableCell");

                tableCell = Instantiate(levelPackTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public SimpleTextTableCell GetSimpleTextTableCell()
        {
            SimpleTextTableCell tableCell = (SimpleTextTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (simpleTextTableCellInstance == null)
                    simpleTextTableCellInstance = Resources.FindObjectsOfTypeAll<SimpleTextTableCell>().First(x => x.name == "SimpleTextTableCell");

                tableCell = Instantiate(simpleTextTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell();

                    TextMeshProUGUI nameText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songNameText");
                    TextMeshProUGUI authorText = tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songAuthorText");
                    tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songBpmText").gameObject.SetActive(false);
                    tableCell.GetField<TextMeshProUGUI, LevelListTableCell>("_songDurationText").gameObject.SetActive(false);
                    tableCell.GetField<Image, LevelListTableCell>("_favoritesBadgeImage").gameObject.SetActive(false);
                    tableCell.transform.Find("BpmIcon").gameObject.SetActive(false);
                    if (expandCell)
                    {
                        nameText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                        authorText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                    }

                    nameText.text = data[idx].text;
                    authorText.text = data[idx].subtext;
                    tableCell.GetField<Image, LevelListTableCell>("_coverImage").sprite = data[idx].icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : data[idx].icon;

                    return tableCell;
                case ListStyle.Box:
                    LevelPackTableCell cell = GetLevelPackTableCell();
                    cell.showNewRibbon = false;
                    cell.GetField<TextMeshProUGUI, LevelPackTableCell>("_infoText").text = $"{data[idx].text}\n{data[idx].subtext}";
                    Image packCoverImage = cell.GetField<Image, LevelPackTableCell>("_coverImage");

                    packCoverImage.sprite = data[idx].icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : data[idx].icon;

                    return cell;
                case ListStyle.Simple:
                    SimpleTextTableCell simpleCell = GetSimpleTextTableCell();
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").richText = true;
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").enableWordWrapping = true;
                    simpleCell.text = data[idx].text;

                    return simpleCell;
            }

            return null;
        }

        public float CellSize()
        {
            return cellSize;
        }

        public int NumberOfCells()
        {
            return data.Count();
        }

        public class CustomCellInfo
        {
            public string text;
            public string subtext;
            public Sprite icon;

            public CustomCellInfo(string text, string subtext = null, Sprite icon = null)
            {
                this.text = text;
                this.subtext = subtext;
                this.icon = icon;
            }
        };
    }
}
