﻿using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Debugs a novel and sets its values to the result.
/// </summary>
public class NovelDebugger : MonoBehaviour
{
    public StringBuilder ErrorList = new System.Text.StringBuilder();
    public int ScriptAmount = 0;
    public int ActionAmount = 0;

    public int WorkingScripts = 0;
    public int WorkingActions = 0;

    public int ErrorScripts = 0;
    public int ErrorActions = 0;

    #region actions
    private Alert Action_Alert;
    private Background Action_Background;
    private Textbox Action_Textbox;
    private Mood Action_Mood;
    private Character Action_Character;
    private DelCharacter Action_DelCharacter;
    private Question Action_Question;
    private LoadScript Action_LoadScript;
    private Wait Action_Wait;
    private PlayAudio Action_PlayAudio;
    private StopAudio Action_StopAudio;
    private bool ActionsSet = false;
    private void SetActions()
    {
        if (ActionsSet) return;
        ActionsSet = true;
        Action_Mood = gameObject.AddComponent<Mood>();
        Action_Alert = gameObject.AddComponent<Alert>();
        Action_Background = gameObject.AddComponent<Background>();
        Action_Textbox = gameObject.AddComponent<Textbox>();
        Action_Character = gameObject.AddComponent<Character>();
        Action_DelCharacter = gameObject.AddComponent<DelCharacter>();
        Action_Question = gameObject.AddComponent<Question>();
        Action_LoadScript = gameObject.AddComponent<LoadScript>();
        Action_Wait = gameObject.AddComponent<Wait>();
        Action_PlayAudio = gameObject.AddComponent<PlayAudio>();
        Action_StopAudio = gameObject.AddComponent<StopAudio>();
    }
    #endregion
    public NovelDebugger DebugNovel(string path)
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        SetActions();
        ResetVariables();
        UnityEngine.Debug.Log("NovelDebugger/Debug: Started debugging " + path);
        NovelDebugger ND = new NovelDebugger();
        if (!Directory.Exists(Path.Combine(path, "Dialogues"))) return ND;
        SetGlobalVariables(path);
        if (!File.Exists(Path.Combine(path, "Dialogues", "start.txt")))
        {
            ErrorActions++;
            ErrorScripts++;
            ErrorList.Append(string.Format(Translate.Get("missingstartscript"), Path.Combine(GameManager.shortStoryPath, "Dialogues", "start.txt")) + "\n-----------------------------------------------------");
        }
        foreach (string script in Directory.GetFiles(Path.Combine(path, "Dialogues"), "*.txt"))
        {
            bool scriptWorking = true;
            ScriptAmount++;
            GameManager.dialoguepos = 0;
            string filename = new FileInfo(script).Name;
            string scripttitle = string.Format(Translate.Get("script"), Path.Combine(GameManager.shortStoryPath, filename) + "\n-----------------------------------------------------");
            ErrorList.Append(scripttitle);
            foreach (string line in File.ReadAllLines(script))
            {
                ActionAmount++;
                NovelDebugInfo NDI = ProcessLine(line.Split('|'));
                if(NDI.Code == NovelDebugInfo.Status.OK) WorkingActions++;
                else
                {
                    ErrorActions++;
                    scriptWorking = false;
                    ErrorList.Append(string.Format(Translate.Get("debugitemtemplate"), NDI.Line, NDI.Action, NDI.Message));
                }
            }

            if (scriptWorking)
            {
                WorkingScripts++;
                ErrorList.Replace(scripttitle, "");
            }
            else
            {
                ErrorScripts++;
                ErrorList.Append("\n");
            }
        }
        st.Stop();
        UnityEngine.Debug.Log($"NovelDebugger/Debug: Finished debugging. Took {st.ElapsedMilliseconds}ms to execute.");
        return ND;
    }
    private void SetGlobalVariables(string path)
    {
        Helper.currentStoryPath = path;
        GameManager.shortStoryPath = new DirectoryInfo(path).Name;
        GameManager.people = Person.Assign();
    }
    private void ResetVariables()
    {
        ErrorList = new StringBuilder();
        ScriptAmount = 0;
        ActionAmount = 0;

        WorkingScripts = 0;
        WorkingActions = 0;

        ErrorScripts = 0;
        ErrorActions = 0;
    }
    private NovelDebugInfo ProcessLine(string[] line)
    {
        GameManager.dialoguepos++;
        if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
            return NovelDebugInfo.OK();

        else if (line[0] == "T" || line[0] == "T2") //textbox
            return Action_Textbox.Working(line);

        else if (line[0] == "ALERT") //general box
            return Action_Alert.Working(line);

        else if (line[0] == "BACKGROUND") //new background
            return Action_Background.Working(line);

        else if (line[0] == "CHAR") //move or create character
            return Action_Character.Working(line);

        else if (line[0] == "DELETE") //delete character
            return Action_DelCharacter.Working(line);

        else if (line[0] == "QUESTION") //question
            return Action_Question.Working(line);

        else if (line[0] == "LOADSTORY") //open new story (no question)
            return Action_LoadScript.Working(line);

        else if (line[0] == "WAIT") //delay
            return Action_Wait.Working(line);

        else if (line[0] == "MOOD") //delay
            return Action_Mood.Working(line);

        else if (line[0] == "PLAYMUSIC" || line[0] == "PLAYSFX")
            return Action_PlayAudio.Working(line);

        else if (line[0] == "STOPSOUNDS" || line[0] == "STOPMUSIC" || line[0] == "STOPSFX")
            return Action_StopAudio.Working(line);

        else if (line[0] == "FINISHGAME")
            return NovelDebugInfo.OK();

        else
        {
            return new NovelDebugInfo
            {
                Line = GameManager.dialoguepos,
                Code = NovelDebugInfo.Status.Error,
                Action = string.Join("|", line),
                Message = Translate.Get("invalidline")
            }; 
        }
    }
}
/// <summary>
/// Contains debug information about a single line.
/// </summary>
public class NovelDebugInfo
{
    public enum Status
    {
        OK,
        Error
    }
    /// <summary>Status code</summary>
    public Status Code;
    /// <summary>Status message</summary>
    public string Message;
    /// <summary>Analyzed line</summary>
    public int Line;
    /// <summary>Analyzed action</summary>
    public string Action;

    /// <summary>
    /// Debug info with custom status and message
    /// </summary>
    private NovelDebugInfo(Status s, string m)
    {
        Code = s;
        Message = m;
    }
    /// <summary>
    /// Debug info with status OK, custom story defined
    /// </summary>
    public NovelDebugInfo(string[] a, int l)
    {
        Action = string.Join("|", a);
        Line = l;
        Code = Status.OK;
        Message = "OK";
    }
    /// <summary>
    /// Debug info with status OK
    /// </summary>
    public NovelDebugInfo()
    {
        Code = Status.OK;
        Message = "OK";
    }
    public NovelDebugInfo Done(string message = "OK")
    {
        Message = message;
        if (Message != "OK") Code = Status.Error;
        return this;
    }
    public static NovelDebugInfo Error(string message)
    {
        return new NovelDebugInfo(Status.Error, message);
    }
    public static NovelDebugInfo OK()
    {
        return new NovelDebugInfo(Status.OK, "OK");
    }
}

