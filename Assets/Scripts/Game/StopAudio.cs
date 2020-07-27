﻿using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopAudio : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "syntax"));
            return;
        }
        StopSound(line[0].ToLower().Replace("stop", ""));
        Game.RunNext();
    }
    public string Working(string[] line)
    {
        return "";
    }
    private void StopSound(string source)
    {
        if (source == "music")
        {
            Game.music.GetComponent<AudioSource>().Stop();
            Game.musicplaying = "none";
        }

        else if (source == "sfx")
            Game.SFX.GetComponent<AudioSource>().Stop();

        else if (source == "sounds")
        {
            Game.music.GetComponent<AudioSource>().Stop();
            Game.SFX.GetComponent<AudioSource>().Stop();
            Game.musicplaying = "none";
        }
    }
}