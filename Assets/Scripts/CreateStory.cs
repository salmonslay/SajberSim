﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SajberSim.CardMenu;
using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateStory : MonoBehaviour
{
    public static string editPath = "";
    public static string editName = "";

    public CreateNew Menu_Create;
    public EditStats Menu_Edit;
    public DebugNovel Menu_Debug;
    public PublishMenu Menu_Publish;


    public GameObject ButtonGroup;
    public GameObject BasicsMenu;
    public GameObject EditsMenu;
    public GameObject DebugMenu;
    public GameObject PublishMenu;
    public Text Title;
    public Text Description;

    public Button ButtonDetails;
    public Button ButtonEdit;
    public Button ButtonVerify;
    public Button ButtonPublish;
    public Button ButtonQuit;
    public Button ButtonCreate;
    public Text OfflineNotice;

    private Language lang;
    private Download dl;
    public StartStory storyMenu;
    public CreateWindows currentWindow;
    public enum CreateWindows
    {
        Basics,
        Details,
        Edit,
        Debug,
        Publish
    }

    void Start()
    {
        StartStory.creatingStory = false;
        dl = Download.Init();
        storyMenu = GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>();
        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ToggleMenu(bool open)
    {
        transform.localScale = open ? Vector3.one : Vector3.zero;
        storyMenu.UpdatePreviewCards();
        StartStory.creatingStory = open;
    }
    /// <summary>
    /// Toggles between all windows in the create menu
    /// </summary>
    public void SetWindow(int window)
    {
        if (Workshop.publishProgress >= 1) Workshop.publishProgress = 0;
        Menu_Publish.P_Loadingbar.UpdateBar(0);
        ButtonDetails.interactable = true;
        ButtonEdit.interactable = true;
        ButtonVerify.interactable = true;
        ButtonPublish.interactable = Helper.loggedin;
        OfflineNotice.gameObject.SetActive(!Helper.loggedin);
        ButtonPublish.gameObject.SetActive(true);
        ButtonCreate.gameObject.SetActive(false);
        BasicsMenu.transform.localScale = Vector3.zero;
        EditsMenu.transform.localScale = Vector3.zero;
        DebugMenu.transform.localScale = Vector3.zero;
        PublishMenu.transform.localScale = Vector3.zero;
        Menu_Create.ResetFields();
        Menu_Edit.SaveColors();
        switch (window)
        {
            case 0: // Create story and set basics
                currentWindow = CreateWindows.Basics;
                Menu_Create.ResetFields();
                Menu_Create.ButtonSave.interactable = false;
                Menu_Create.ButtonRevert.interactable = false;
                ButtonDetails.interactable = false;
                ButtonEdit.interactable = false;
                ButtonPublish.interactable = false;
                ButtonVerify.interactable = false;
                ButtonPublish.gameObject.SetActive(false);
                ButtonCreate.gameObject.SetActive(true);
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("createnewnovel");
                Description.text = Translate.Get("createnewdesc");
                editPath = "NEW";
                editName = "New novel";
                break;
            case 1: // Story created, fill fields with predefined info
                currentWindow = CreateWindows.Details;
                ButtonDetails.interactable = false;
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("details");
                Description.text = string.Format(Translate.Get("detailsdescription"), editName);
                Menu_Create.SetDetails();
                break;
            case 2: // Edit story 
                currentWindow = CreateWindows.Edit;
                EditsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("editstats");
                Description.text = string.Format(Translate.Get("editsdescription"), editName);
                ButtonEdit.interactable = false;
                Menu_Edit.UpdateStats();
                break;
            case 3: // Debug story
                currentWindow = CreateWindows.Debug;
                DebugMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("debugtitle");
                Description.text = string.Format(Translate.Get("debugdescription"), editName);
                ButtonVerify.interactable = false;
                Menu_Debug.UpdateList();
                break;
            case 4: // Publish story
                currentWindow = CreateWindows.Publish;
                PublishMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("publishtitle");
                Description.text = string.Format(Translate.Get("publishdescription"), Translate.Get("details"));
                ButtonPublish.interactable = false;
                Menu_Publish.FillData();
                break;

        }
    }

    public void PlayCredits()
    {
        Credits.storypath = editPath;
        SceneManager.LoadScene("credits");
    }
    public void OpenDirectory()
    {
        if (Directory.Exists(editPath))
            Process.Start(editPath);
    }
    public void OpenScripts()
    {
        string path = Path.Combine(editPath, "Dialogues");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Process.Start(path);
    }
}
