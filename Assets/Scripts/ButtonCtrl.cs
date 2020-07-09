﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SajberSim.Chararcter;
using SajberSim.Web;
using SajberSim.StoryDebug;
using SajberSim.Story;
using SajberSim.Helper;
using SajberSim.Translation;
using Steamworks;
using Steamworks.Data;
using SajberSim.Steam;

/// <summary>
/// Controls buttons on the main menu
/// </summary>
public class ButtonCtrl : MonoBehaviour
{
    public GameObject CreationMenu;
    public GameObject OverwriteAlert;
    public Text OverwriteTitle;
    private string overwritetext;
    public GameObject Logo;
    public GameObject Modding;
    public GameObject CreditsButton;
    public GameObject SettingsMenu;
    public Button DebugButton;
    public GameObject BehindSettings;
    public static Character[] people = new Character[4];
    public GameObject fadeimage;
    public AudioSource music;
    public static string charpath;


    private Download dl;

    
    public GameObject PauseMenuGame;
    public GameObject SettingsMenuGame;
    private void OnEnable()
    {
        if (!Helper.loggedin)
        {
            try
            {
                SteamClient.Init(Helper.AppID);
                Helper.loggedin = true;
                UnityEngine.Debug.Log($"Steam: Connected to {SteamClient.Name} (ID: {SteamClient.SteamId})");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Steam: Could not connect to steam. Is it open?\n{e}");
                Helper.Alert(Translate.Get("noconnection"));
                Helper.loggedin = false;
            }
        }
    }
    public void Start()
    {
        if (GameObject.Find("Helper"))
        {
            dl = GameObject.Find("Helper").GetComponent<Download>();
        }
        else
        {
            dl = new GameObject("downloadobj").AddComponent<Download>();
        }

        Cursor.visible = true;
        UpdateUI();
        string storyid = PlayerPrefs.GetString("story", "none");
        //if (File.Exists($"{Application.dataPath}/Story/{storyid}/manifest.json"))
        //    GameObject.Find("Canvas/Overwrite warning/title").GetComponent<Text>().text = string.Format(Translate.Get("overwritewarning"), Helper.GetManifest(storyid).name);
        //else
        //    PlayerPrefs.SetString("story", "none");

        Text loginstatus = GameObject.Find("Canvas/Username").GetComponent<Text>();
        //Set login if you are logged in
        if (Helper.loggedin)
        {
            loginstatus.text = string.Format(Translate.Get("welcomeuser"), SteamClient.Name);
            dl.CardThumbnail(GameObject.Find("Canvas/ProfilePictureFrame/ProfilePicture").transform, SteamAPI.GetProfile($"{SteamClient.SteamId}").Avatarfull);
        }
        else
        {
            loginstatus.text = Translate.Get("offline");
            loginstatus.transform.localPosition = new Vector3(374.16f, 318.84f, 0);
            GameObject.Find("Canvas/ProfilePictureFrame").SetActive(false);

        }
        GameObject.Find("Canvas/Version").GetComponent<Text>().text = 'v' + Application.version;
    }
   public void OpenSteamProfile()
    {
        if (Helper.loggedin)
        {
            if (Application.isEditor)
                Process.Start($@"steam://openurl/{SteamAPI.GetProfile($"{SteamClient.SteamId}").Profileurl}");
            else
                SteamFriends.OpenUserOverlay(SteamClient.SteamId, "steamid");
        }
    }
    private void UpdateUI()
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            List<string> charpaths = new List<string>();
            foreach (string path in Helper.GetAllStoryAssetPaths("characters"))
            {
                if (path.Contains("happy")) charpaths.Add(path);
            }
            if (charpaths.Count() == 0) return;
            charpath = charpaths[UnityEngine.Random.Range(0,charpaths.Count)];
            while (!File.Exists(charpath.Replace("happy", "blush"))) 
            {
                charpath = charpaths[UnityEngine.Random.Range(0, charpaths.Count)];
            }

            //ladda in filen som texture
            dl.Image(GameObject.Find("Character"), $"file://{charpath}");
        }
    }
    
    public void StartNew() //Just checks if a new story should be started
    {
        if (PlayerPrefs.GetString("story", "none") != "none")
        {
            //story found
            OverwriteAlert.transform.localScale = Vector3.one;
        }
        else
            StartNewConfirmed();
    }

    public void StartNewConfirmed() //confirmed that user wants to start a new
    {
        OverwriteAlert.transform.localScale = Vector3.zero;
        GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().OpenMenu(false);
    }
    public void OpenWorkshop()
    {
        if (!Helper.loggedin)
        {
            Helper.Alert(Translate.Get("noconnection"));
            return;
        }
        Helper.Alert(Translate.Get("openedworkshop"));
        Process.Start($@"steam://openurl/https://steamcommunity.com/app/1353530/workshop/");
    }
    public void CreateNovel()
    {
        //WorkshopItemUpdate createNewItemUsingGivenFolder = new WorkshopItemUpdate();
        //createNewItemUsingGivenFolder.ContentPath = @"H:\School code stuff\CyberSim\CyberSim\Assets\Story\OpenHouse";
        //((SteamWorkshopPopupUpload)uMyGUI_PopupManager.Instance.ShowPopup("steam_ugc_upload")).UploadUI.SetItemData(createNewItemUsingGivenFolder);
    }

    public void CreateCharacters()
    {
        System.Random rnd = new System.Random();
        string configPath = $"{Helper.currentStoryPath}/Characters/characterconfig.txt";
        if (!File.Exists(configPath)) return;
        string[] config = File.ReadAllLines(configPath);

        people = new Character[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Character(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array

        for (int i = 0; i < people.Length; i++) //sparar ID i playerpref
            PlayerPrefs.SetInt($"character{i}",people[i].ID);
    }
    private void LoadCharacters() //Loads characters from playerprefs
    {
        string path = $"{Helper.currentStoryPath}/Characters/characterconfig.txt";
        if (!File.Exists(path)) return;
        string[] config = File.ReadAllLines(path);
        people = new Character[PlayerPrefs.GetInt("characters", 1)];

        for (int i = 0; i < people.Length; i++) //fill array from save
        {
            int tempID = PlayerPrefs.GetInt($"character{i}", 0);
            people[i] = new Character(config[tempID].Split(',')[0], config[tempID].Split(',')[1], i);
        }
    }
    public void Continue() //just opens everything SAVED
    {
        Manifest data = Manifest.Get($"{Application.dataPath}/Story/{PlayerPrefs.GetString("story")}/manifest.json");
        StartStory.storymenuOpen = false;
        GameManager.storyAuthor = data.author;
        GameManager.storyName = data.name;
        LoadCharacters();
        StartCoroutine(FadeToScene("game"));
    }

    public void OpenMenu(GameObject menu) //opens a menu, like settings or modding
    {
        menu.SetActive(true);
        BehindSettings.SetActive(true);
    }
    public void GoBack()
    {
        SettingsMenuGame.SetActive(false);
    }

    public void CloseSettings() //closes all menus
    {
        BehindSettings.SetActive(false);
        Modding.SetActive(false);
        PauseMenuGame.SetActive(false);
        SettingsMenuGame.SetActive(false);
        OverwriteAlert.transform.localScale = Vector3.zero;
        GameManager.paused = false;
        
    }
    public void QuitGame() //nuff' said
    {
        Application.Quit();
    }
    public void ResetAll() //fucking nukes all the stats like the US during 1945
    {
        PlayerPrefs.DeleteAll();
    }
    
    public void OpenFolder(string path)
    {
        if (!Directory.Exists($@"{Application.dataPath}/{path}"))
        {
            UnityEngine.Debug.LogError($"Tried to open folder with argument \"{path}\" which does not exist (full path: {Application.dataPath}/{path}");
            return;
        }
        if(path == "Logs") Helper.CreateLogfile();
        Process.Start("explorer.exe", $@"{Application.dataPath}/{path}");
    }
    public void FindAndCloseSettings()
    { 
        if(GameObject.Find("Canvas/Settings(Clone)"))
        GameObject.Find("Canvas/Settings(Clone)").GetComponent<Settings>().CloseMenu();
    }
    
    public void OpenLink(string link)
    {
        Process.Start(link);
    }
    public void OpenLogfile()
    {
        Process.Start($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/LocalLow/Te18B/SajberSim/Player.log".Replace("/", "\\"));
    }
    /*public void Debug()
    {
        StoryDebugger.CreateLog();
        StartCoroutine(ToggleDebug());
        
    }*/
    public void OpenSettings()
    {
        GameObject x = Instantiate(SettingsMenu, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        x.transform.localPosition = Vector3.zero;
    }
    public void GAMEOpenSettings()
    {
        PauseMenuGame.SetActive(false);
        SettingsMenuGame.SetActive(true);
        GameManager.paused = true;
    }
    IEnumerator ToggleDebug() //disables the button for 2 seconds to avoid doubleclicks
    {
        DebugButton.interactable = false;
        yield return new WaitForSeconds(2f);
        DebugButton.interactable = true;
    }
    public void StartScene(string scene) //seems like i couldn't start coroutines with buttons
    {
        StartCoroutine(FadeToScene(scene));
    }
    public IEnumerator FadeToScene(string scene)
    {
        StartCoroutine(AudioFadeOut.FadeOut(music, 0.4f));
        //if(MainPopup.singlecharClicked >= 5) GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popdown"); //fade away easter egg if active
        MainPopup.singlecharClicked = 0;
        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime/0.5f) //Starts fade, load scene when done
        {
            fadeimage.GetComponent<UnityEngine.UI.Image>().color = new UnityEngine.Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }
    private void OnApplicationQuit()
    {
        DiscordRpc.Shutdown(); //Stänger Discord RPC
        SteamClient.Shutdown();
        Helper.CreateLogfile();

    }
}