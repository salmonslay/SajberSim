﻿using SajberSim.CardMenu;
using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EditStats : MonoBehaviour
{
    public CreateStory Main;
    public Text E_StatsTitle;
    public Text E_Stats;

    public void UpdateStats()
    {
        GameObject card = Main.storyMenu.CreateCard(CreateStory.currentlyEditingPath, Manifest.Get(CreateStory.currentlyEditingPath + "/manifest.json"), new Vector3(680.6f, -45.3f, 0), "Canvas/CreateMenu/EditMenu");
        card.transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
        card.name = "Preview card";
        card.tag = "Untagged";
        int dialogues = 0;
        int alerts = 0;
        int words = 0;
        int backgroundchanges = 0;
        int decisions = 0;
        int participants = 0;
        string filesize = $"{Math.Round(Helper.BytesTo(Helper.DirSize(new DirectoryInfo(CreateStory.currentlyEditingPath)), Helper.DataSize.Megabyte)),1} Mb";
        bool hasstart = false;
        bool hascredits = false;
        bool hasthumbnail = false;
        bool hassteam = false;

        E_StatsTitle.text = string.Format(Translate.Get("statsabout"), CreateStory.currentlyEditingName);
        string[] scriptPaths = Stories.GetStoryAssetPaths("dialogues", CreateStory.currentlyEditingPath);
        List<string> scriptLines = new List<string>();
        foreach (string path in scriptPaths)
        {
            if (File.Exists(path))
            {
                scriptLines.AddRange(File.ReadAllLines(path));
            }
        }
        int scripts = scriptPaths.Length;
        int lines = scriptLines.Count();
        int audio = Stories.GetStoryAssetPaths("audio", CreateStory.currentlyEditingPath).Length;
        int backgrounds = Stories.GetStoryAssetPaths("backgrounds", CreateStory.currentlyEditingPath).Length;
        int characters = Stories.GetStoryAssetPaths("characters", CreateStory.currentlyEditingPath).Length;
        try
        {
            int i = 0;
            foreach (string line in scriptLines)
            {
                string action = line.Split('|')[0];
                switch (action)
                {
                    case "T":
                        dialogues++;
                        if (line.Split('|').Length == 3)
                            words += line.Split('|')[2].Count(f => f == ' ') + 1;
                        break;
                    case "BG": backgroundchanges++; break;
                    case "ALERT": alerts++; break;
                    case "QUESTION":
                        decisions++;
                        if (line.Split('|').Length > 1)
                            words += line.Split('|')[1].Count(f => f == ' ') + 1;
                        break;
                }
                i++;
            }
            if (File.Exists(CreateStory.currentlyEditingPath + "/credits.txt"))
            {
                foreach (string line in File.ReadAllLines(CreateStory.currentlyEditingPath + "/credits.txt"))
                {
                    if (!line.Contains('|') && !line.StartsWith("-") && line != "") participants++;
                }
            }
            E_Stats.text =
                $"{string.Format(Translate.Get("totalscripts"), scripts)}\n" +
                $"{string.Format(Translate.Get("totallines"), lines)}\n" +
                $"{string.Format(Translate.Get("totalaudio"), audio)}\n" +
                $"{string.Format(Translate.Get("totalbackgrounds"), backgrounds)}\n" +
                $"{string.Format(Translate.Get("totalcharacters"), characters)}\n" +
                $"{string.Format(Translate.Get("totaldialogues"), dialogues)}\n" +
                $"{string.Format(Translate.Get("totalalerts"), alerts)}\n" +
                $"{string.Format(Translate.Get("totalwords"), words)}\n" +
                $"{string.Format(Translate.Get("totalbgchanges"), backgroundchanges)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n\n" +
                $"{string.Format(Translate.Get("totalparticipants"), participants)}\n" +
                $"{string.Format(Translate.Get("filesize"), filesize)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n" +
                $"";
        }
        catch (Exception e)
        {
            E_Stats.text = $"Something went wrong when trying to show stats: \n{e}";
            Debug.LogError($"Something went wrong when trying to show stats: \n{e}");
        }
    }
}

