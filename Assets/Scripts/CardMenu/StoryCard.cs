﻿using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryCard : MonoBehaviour
{
    private Manifest data;
    public Text Title;
    public Image Clock;
    public Text Playtime;
    public Image Overlay;
    public Image Thumbnail;
    public Image Flag;
    public Text NSFW;
    public string storyPath;
    private StartStory storyManager;
    public Download dl;

    void Start()
    {
        storyManager = GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetData(Manifest storyData, string path)
    {
        data = storyData;
        storyPath = path;
        if (File.Exists($"{storyPath}/thumbnail.png"))
            dl.CardThumbnail(Thumbnail, $"{storyPath}/thumbnail.png");
        else
            Thumbnail.color = Color.white;

        Color splashColor = Color.white;
        ColorUtility.TryParseHtmlString($"#{data.overlaycolor.Replace("#", "")}", out splashColor);
        Overlay.GetComponent<Image>().color = splashColor;

        Color textColor = Colors.UnityGray;
        ColorUtility.TryParseHtmlString($"#{data.textcolor.Replace("#", "")}", out textColor);
        Title.GetComponent<Text>().color = textColor;

        Clock.color = textColor;
        Playtime.color = textColor;
        Playtime.text = TimeSpan.FromMinutes(data.playtime).ToString(@"h\hmm\m");

        if (!data.nsfw)
        {
            NSFW.color = new Color(0, 0, 0, 0); //hide
            Clock.transform.localPosition = new Vector3(Clock.transform.localPosition.x, 47, 0);
        }
        else if (data.nsfw) // easier to read than just an else 
        {
            NSFW.color = textColor; //show
            Clock.transform.localPosition = new Vector3(Clock.transform.localPosition.x, 57, 0);
        }

        Title.GetComponent<Text>().text = data.name;

        Flag.sprite = dl.Flag(data.language);
    }
    public void Play() 
    { 
        Helper.currentStoryPath = storyPath;
        Helper.currentStoryName = data.name;
        Debug.Log($"Attempting to start the novel \"{data.name}\" with path {storyPath}");
        ButtonCtrl main = GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>();
        main.CreateCharacters();
        StartStory.storymenuOpen = false;
        GameManager.storyAuthor = data.author;
        GameManager.storyName = data.name;
        Achievements.Grant(Achievements.List.ACHIEVEMENT_play1);
        Stats.Add(Stats.List.novelsstarted);
        StartCoroutine(main.FadeToScene("game"));
    }
}