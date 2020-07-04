﻿using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateStory : MonoBehaviour
{
    public static string currentlyEditingPath = @"H:\School code stuff\CyberSim\CyberSim\Assets\Story\OpenHouse"; //Base path, should be root/SajberSim_Data/MyStories/X
    public static string currentlyEditingName = "Cybergymnasiet Öppet hus";

    public InputField B_inputName;
    public InputField B_inputDescription;
    public InputField B_inputTags;
    public Dropdown B_inputGenre;
    public Dropdown B_inputAudience;
    public Dropdown B_inputLanguage;

    public Text E_StatsTitle;
    public Text E_Stats;
    public GameObject E_Thumbnail;

    public GameObject ButtonGroup;
    public GameObject BasicsMenu;
    public GameObject EditsMenu;
    public Text Title;
    public Text Description;

    public Button ButtonDetails;
    public Button ButtonEdit;
    public Button ButtonVerify;
    public Button ButtonPublish;
    public Button ButtonQuit;
    public Button ButtonCreate;

    private Language lang;
    Download dl;
    StartStory storyMenu;
    public enum CreateWindows
    {
        Basics,
        Details,
        Edit,
        Verify,
        Publish
    }

    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        storyMenu = GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>();
        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;

        B_inputGenre.AddOptions(Helper.genres.ToList());
        B_inputAudience.AddOptions(Helper.audience.ToList());

        List<Dropdown.OptionData> dropdownList = new List<Dropdown.OptionData>();
        foreach (Language lang in Language.list)
            dropdownList.Add(new Dropdown.OptionData(lang.formal_name, dl.Flag(lang.flag_code)));
        B_inputLanguage.AddOptions(dropdownList);

        if (Helper.loggedin)
            B_inputName.placeholder.GetComponent<Text>().text = string.Format(Translate.Get("defaultnameuser"), SteamClient.Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleMenu(bool open)
    {
        transform.localScale = open ? Vector3.one : Vector3.zero;
    }
    public void SetWindow(int window)
    {
        ButtonDetails.interactable = true;
        ButtonEdit.interactable = true;
        ButtonVerify.interactable = true;
        ButtonPublish.interactable = true;
        BasicsMenu.transform.localScale = Vector3.zero;
        EditsMenu.transform.localScale = Vector3.zero;
        switch (window)
        {
            case 0: // Create story and set basics
                ButtonDetails.interactable = false;
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("createnewnovel");
                Description.text = Translate.Get("createnewdesc");
                break;
            case 1: // Story created, fill fields with predefined info
                ButtonDetails.interactable = false;
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("details");
                Description.text = Translate.Get("detailsdescription");
                SetBasics();
                break;
            case 2: // Edit story 
                ButtonEdit.interactable = false;
                EditsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("edit");
                Description.text = Translate.Get("editsdescription");
                SetEdits();
                break;
        }
    }
    /// <summary>
    /// Create a basics menu for when a novel already exists and set the data
    /// </summary>
    private void SetBasics()
    {
        Manifest data = Helper.GetManifest(currentlyEditingPath + "/manifest.json");
        B_inputName.SetTextWithoutNotify(data.name);
        B_inputDescription.SetTextWithoutNotify(data.description);
        B_inputTags.SetTextWithoutNotify(string.Join(", ", data.tags));
        B_inputAudience.SetValueWithoutNotify(Array.IndexOf(Helper.audience, data.rating));
        B_inputGenre.SetValueWithoutNotify(Array.IndexOf(Helper.genresid, data.genre));

        int langIndex = Array.IndexOf(Language.ListFlagCode().ToArray(), data.language);
        if (langIndex == -1)
            B_inputLanguage.SetValueWithoutNotify(0);
        else
            B_inputLanguage.SetValueWithoutNotify(langIndex);
    }
    /// <summary>
    /// Create an edit menu and set the data
    /// </summary>
    private void SetEdits()
    {
        UpdateStats();
    }
    public void UpdateStats()
    {
        GameObject card = storyMenu.CreateCard(currentlyEditingPath, Helper.GetManifest(currentlyEditingPath + "/manifest.json"), new Vector3(680.6f, -45.3f, 0), -1, "Canvas/CreateMenu/EditMenu");
        card.transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
        int dialogues = 0;
        int alerts = 0;
        int words = 0;
        int backgroundchanges = 0;
        int decisions = 0;
        int participants = 0;
        int filesize = 0;
        bool hasstart = false;
        bool hascredits = false;
        bool hasthumbnail = false;
        bool hassteam = false;

        E_StatsTitle.text = string.Format(Translate.Get("statsabout"), currentlyEditingName);
        string[] scriptPaths = Helper.GetStoryAssetPaths("dialogues", currentlyEditingPath);
        List<string> scriptLines = new List<string>();
        foreach(string path in scriptPaths)
        {
            if (File.Exists(path))
            {
                scriptLines.AddRange(File.ReadAllLines(path));
            }
        }
        int scripts = scriptPaths.Length;
        int lines = scriptLines.Count();
        int audio = Helper.GetStoryAssetPaths("audio", currentlyEditingPath).Length;
        int backgrounds = Helper.GetStoryAssetPaths("backgrounds", currentlyEditingPath).Length;
        int characters = Helper.GetStoryAssetPaths("characters", currentlyEditingPath).Length;
        try
        {
            foreach (string line in scriptLines)
            {
                string action = line.Split('|')[0];
                switch (action)
                {
                    case "T":
                        dialogues++;
                        words += line.Split('|')[2].Count(f => f == ' ') + 1;
                        break;
                    case "BG": backgroundchanges++; break;
                    case "ALERT": alerts++; break;
                    case "QUESTION": 
                        decisions++;
                        words += line.Split('|')[1].Count(f => f == ' ') + 1;
                        break;
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
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n" +
                $"";
        }
        catch(Exception e)
        {
            E_Stats.text = $"Something went wrong: \n{e}";
            Debug.LogError(e);
        }
    }

}
