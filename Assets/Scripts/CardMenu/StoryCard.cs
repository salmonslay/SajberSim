﻿#pragma warning disable CS0162 // Unreachable code detected

using SajberSim.CardMenu;
using SajberSim.Chararcter;
using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Web;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryCard : MonoBehaviour
{
    public bool myNovel = false;
    private Manifest data;
    public Text Title;
    public Image Paper;
    public Text Playtime;
    public Image Overlay;
    public RawImage Thumbnail;
    public Image Flag;
    public Text NSFW;
    public string storyPath;
    private StartStory storyManager;
    public Download dl;
    public DetailsCard detailsCard;
    public GameObject detailsTemplate;
    public StoryStats stats;
    public GameObject ColorOverlayObject;
    public GameObject DemoNotice;

    private void Start()
    {
        storyManager = GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void CheckOwnerStatus()
    {
        if (data == null) return;
        myNovel = storyPath.Contains($"SajberSim_Data{Path.DirectorySeparatorChar}MyNovels") || (Application.isEditor && storyPath.Contains("MyNovels"));
        if (data.authorid == Helper.SteamIDCache()) myNovel = true;

        if (Demo.isDemo) myNovel = false;
    }

    public void SetData(Manifest storyData, string path)
    {
        stats = StoryStats.Get(path);
        data = storyData;
        storyPath = path;
        CheckOwnerStatus();
        if (File.Exists(Path.Combine(storyPath, "thumbnail.png")))
            dl.CardThumbnail(Thumbnail, Path.Combine(storyPath, "thumbnail.png"));
        else
            Thumbnail.color = Color.white;

        Color textColor = Colors.FromRGB(data.textcolor);
        Overlay.GetComponent<Image>().color = Colors.FromRGB(data.overlaycolor);
        Title.GetComponent<Text>().color = textColor;
        Paper.color = textColor;
        Playtime.color = textColor;

        Playtime.text = stats.wordsK;

        if (!data.nsfw)
        {
            NSFW.color = new Color(0, 0, 0, 0); //hide
            Paper.transform.localPosition = new Vector3(Paper.transform.localPosition.x, 47, 0);
        }
        else if (data.nsfw) // easier to read than just an else
        {
            NSFW.color = textColor; //show
            Paper.transform.localPosition = new Vector3(Paper.transform.localPosition.x, 57, 0);
        }

        Title.GetComponent<Text>().text = data.name;
        try
        {
            Flag.sprite = dl.Flag(Language.Languages[data.language].iso_code);
        }
        catch { }
        UpdateDemoStatus();
    }

    private void UpdateDemoStatus()
    {
        if (Demo.isDemo)
        {
            bool included = false;
            foreach (Demo.DemoNovel n in Demo.allowedNovels)
            {
                if (data.id == n.id && data.authorid == n.by)
                {
                    included = true;
                    break;
                }
            }
            if (!included)
            {
                Destroy(ColorOverlayObject.GetComponent<CardOverlay>());
                Destroy(ColorOverlayObject.GetComponent<EventTrigger>());
                ColorOverlayObject.GetComponent<Image>().color = Colors.AlmostSolid;
                DemoNotice.SetActive(true);
            }
        }
    }

    public void Play()
    {
        Helper.currentStoryPath = storyPath;
        Helper.currentStoryName = data.name;
        Debug.Log($"Attempting to start the novel \"{data.name}\" with path {storyPath}");
        ButtonCtrl main = GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>();
        Person.Assign();
        StartStory.storymenuOpen = false;
        GameManager.storyAuthor = data.author;
        GameManager.storyName = data.name;
        Achievements.Grant(Achievements.List.ACHIEVEMENT_play1);
        Stats.Add(Stats.List.novelsstarted);
        StartCoroutine(main.FadeToScene("game"));
    }

    public void Details()
    {
        GameObject details = Instantiate(detailsTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject;
        details.transform.localPosition = new Vector3(0, 0, 1);
        details.name = $"Details card for {data.name}";
        details.GetComponent<DetailsCard>().card = this;
        details.GetComponent<DetailsCard>().data = data;
        details.GetComponent<DetailsCard>().UpdateDetails(Thumbnail);
    }

    public void Edit()
    {
        CreateStory createManager = GameObject.Find("Canvas/CreateMenu").GetComponent<CreateStory>();
        CreateStory.editName = data.name;
        CreateStory.editPath = storyPath;
        createManager.SetWindow(1);
        createManager.ToggleMenu(true);
    }
}