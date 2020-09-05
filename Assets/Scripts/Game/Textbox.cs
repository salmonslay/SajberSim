﻿using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : MonoBehaviour, INovelAction
{
    private Text textobj;
    private Text nameobj;
    public GameManager Game;

    public void Run(string[] line)
    {
        GameManager.textdone = false;
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "T|person|text|(showportrait)"));
            GameManager.textdone = true;
            return;
        }
        string name;
        if (int.TryParse(line[1], out int x))
            name = GameManager.people[int.Parse(line[1])].name;
        else
            name = line[1];
        string text = Game.FillVars(line[2]);

        bool port = true;
        Debug.Log($"{name} says: {text}");
        if (line.Length == 4) if (line[3] == "false") port = false;
        if (line[0] == "T2") port = false;
        StartCoroutine(SpawnTextBox(name, Helper.UwUTranslator(text), port));
    }

    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        ///Check length
        if (line.Length > 4 || line.Length < 3) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, "3-4")); //Incorrect length, found LENGTH arguments but the action expects 3-4.

        ///Check character config & assign name
        string name;
        int customCharacters = 0;
        string configPath = Path.Combine(Helper.currentStoryPath, "Characters", "characterconfig.txt");
        if (File.Exists(configPath)) customCharacters = File.ReadAllLines(configPath).Length;
        if (Helper.IsNum(line[1])) if (int.Parse(line[1]) >= customCharacters) return NDI.Done(string.Format(Translate.Get("invalidcharacterconfig"), line[1], customCharacters, Path.Combine("Characters", "characterconfig.txt")));
        if (Helper.IsNum(line[1])) name = GameManager.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];

        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name.ToLower() + "port.png")) && !File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name.ToLower(), "port.png")) && (line.Length == 3))
            return NDI.Done(string.Format(Translate.Get("missingcharacterport"), Path.Combine(GameManager.shortStoryPath, "Characters", name.ToLower(), "port.png")));

        return NDI;
    }
    private IEnumerator SpawnTextBox(string name, string target, bool port) //ID 0
    {
        ChangeTextboxType(port);
        Download dl = GameObject.Find("Helper").GetComponent<Download>();
        Game.textbox.SetActive(true);
        if (port && GameManager.currentPortrait != name)
        {
            string path = Path.Combine(Helper.currentStoryPath, "Characters", name.ToLower() + "port.png");
            if (File.Exists(path))
                dl.Image(Game.portrait, path);
            else
                dl.Image(Game.portrait, Path.Combine(Helper.currentStoryPath, "Characters", name.ToLower(), "port.png"));
            GameManager.currentPortrait = name;
        }
        nameobj.text = name;

        if (PlayerPrefs.GetFloat("delay", 0.04f) > 0.001f) //ifall man stängt av typing speed är denna onödig
        {
            string written = target[0].ToString(); //written = det som står hittills

            for (int i = 1; i < target.Length; i++)
            {
                written = written + target[i];
                yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
                if (GameManager.textdone) //avbryt och skriv hela
                {
                    textobj.text = target;
                    GameManager.textdone = true;
                    break;
                }
                textobj.text = written;
            }
        }
        textobj.text = target;
        GameManager.textdone = true;
    }
    private void ChangeTextboxType(bool portrait)
    {
        if (portrait)
        {
            textobj = Game.commentPort;
            nameobj = Game.nametagPort;
            Game.portrait.transform.localScale = Vector3.one;
        }
        else
        {
            textobj = Game.comment;
            nameobj = Game.nametag;
            Game.portrait.transform.localScale = Vector3.zero;
        }
    }
}