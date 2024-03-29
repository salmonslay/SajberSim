﻿using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prefs = SajberSim.Helper.Helper.Prefs;

/// <summary>
/// Placed on settings menu
/// </summary>
public class Settings : MonoBehaviour
{
    private Helper shelper;
    private Download dl;

    //pause stuff ingame
    public static bool paused = false;

    // Start is called before the first frame update
    private void Start()
    {
        shelper = GameObject.Find("Helper").GetComponent<Helper>();
        if (PlayerPrefs.GetString(Prefs.language.ToString(), "none") != "none")
            transform.Find("Language/Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(Array.IndexOf(Translate.languages, PlayerPrefs.GetString(Prefs.language.ToString())));
        dl = Download.Init();

        transform.Find("WritingSpeed/Value").GetComponent<Text>().text = $"{Math.Round(PlayerPrefs.GetFloat("delay", 0.04f) * 1000)}ms";
        transform.Find("Volume/Value").GetComponent<Text>().text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 0.5f) * 100)}%";
        transform.Find("CreditsSpeed/Value").GetComponent<Text>().text = $"{PlayerPrefs.GetFloat("creditspeed", 50)}";

        transform.Find("WritingSpeed/Slider").GetComponent<Slider>().SetValueWithoutNotify(PlayerPrefs.GetFloat("delay", 0.04f));
        transform.Find("Volume/Slider").GetComponent<Slider>().SetValueWithoutNotify(PlayerPrefs.GetFloat("volume", 0.5f));
        transform.Find("CreditsSpeed/Slider").GetComponent<Slider>().SetValueWithoutNotify(PlayerPrefs.GetFloat("creditspeed", 50f));

        if (PlayerPrefs.GetInt("uwu", 0) == 1)
        {
            transform.Find("UwU/Toggle").GetComponent<Toggle>().SetIsOnWithoutNotify(true);
            transform.Find("UwU/Value").GetComponent<Text>().text = "(◠‿◠✿)";
        }
    }

    public void ChangeSpeed(float value) //runs when the speed slider is changed
    {
        PlayerPrefs.SetFloat("delay", value);
        transform.Find("WritingSpeed/Value").GetComponent<Text>().text = $"{Math.Round(PlayerPrefs.GetFloat("delay", 0.04f) * 1000)}ms";
    }

    public void ChangeVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 0.4f);
        transform.Find("Volume/Value").GetComponent<Text>().text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 0.4f) * 100)}%";
    }

    public void UwUToggle(bool uwu)
    {
        if (uwu)
        {
            transform.Find("UwU/Value").GetComponent<Text>().text = "(◠‿◠✿)";
            PlayerPrefs.SetInt("uwu", 1);
        }
        else //disable / false
        {
            transform.Find("UwU/Value").GetComponent<Text>().text = "";
            PlayerPrefs.SetInt("uwu", 0);
        }
    }

    public void SetLanguage(int n)
    {
        Translate.lang = Translate.languages[n];
        PlayerPrefs.SetString("language", Translate.languages[n]);
        transform.Find("Language/UpdateNotif").GetComponent<Text>().text = "Language change will take effect when you restart the game.";
    }

    public void ChangeCreditsSpeed(float value)
    {
        PlayerPrefs.SetFloat("creditspeed", value);
        transform.Find("CreditsSpeed/Value").GetComponent<Text>().text = $"{PlayerPrefs.GetFloat("creditspeed", 50)}";
        Debug.Log(value);
        PlayerPrefs.Save();
    }

    public void ReturnToMain()
    {
        Time.timeScale = 1;
        StartCoroutine(FadeToScene("menu"));
    }

    public IEnumerator FadeToScene(string scene)
    {
        StartCoroutine(AudioFadeOut.FadeOut(GameObject.Find("MusicPlayer").GetComponent<AudioSource>(), 1.55f));
        GameObject.Find("Canvas/Fade").GetComponent<Image>().transform.position = new Vector3(162, 160, 0); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime / 1.5f) //Starts fade, load scene when done
        {
            GameObject.Find("").GetComponent<Image>().color = new Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    public void CloseMenu()
    {
        if (GameObject.Find("ButtonCtrl") != null)
        {
            GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>().BehindSettings.SetActive(false);
        }
        GetComponent<Animator>().Play("closeStorymenu");
        Destroy(this.gameObject, 0.5f);
    }

    public void CreateAndOpenLog()
    {
        Helper.CreateLogfile();
        Helper.OpenFolderFromGame("Logs");
    }

    public void OpenVault()
    {
        GameObject vault = Instantiate(Resources.Load($"Prefabs/Vault", typeof(GameObject)), Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        vault.transform.localPosition = Vector3.zero;
    }

    public void OpenAbout()
    {
        GameObject about = Instantiate(Resources.Load($"Prefabs/About", typeof(GameObject)), Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        about.transform.localPosition = Vector3.zero;
    }
}