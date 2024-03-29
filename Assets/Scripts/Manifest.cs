﻿using Newtonsoft.Json;
using SajberSim.CardMenu;
using SajberSim.Colors;
using SajberSim.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq.Expressions;

/// <summary>
/// Visual Novel manifest containing all metadata
/// </summary>
public class Manifest
{
#pragma warning disable CS0649
    //Metadata
    public string name = "NAME_MISSING";
    public string description = "DESCRIPTION_MISSING";
    public string language = "UNKNOWN";
    public bool nsfw = false;
    public int playtime = 0;
    public DateTime uploaddate = new DateTime(1970, 1, 1, 0, 0, 0);
    public DateTime lastEdit = new DateTime(1970, 1, 1, 0, 0, 0);
    public string[] tags = new string[0];
    public string genre = "other";
    public string rating = "everyone";

    //Steam
    public string author = "Unknown";
    public string authorid = "-1";
    public string id = "0";

    //Designs
    public string overlaycolor = "FFFFFF";
    public string textcolor = "323232";
    public bool customname = false;



    public static Manifest Get(string path)
    {
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogWarning($"Manifest/Get: Tried getting manifest for path \"{path}\" which does not exist");
            return null;
        }
        try
        {
            return JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(path));
        }
        catch
        {
            UnityEngine.Debug.LogError($"Manifest/Get: Something went wrong when converting manifest \"{path}\". Is it setup correctly?");
            return null;
        }
    }
    /// <summary>
    /// Returns paths to all story manifest files if they exist
    /// </summary>
    public static string[] GetAll(Helper.StorySearchArgs args = Helper.StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", Helper.StorySearchPaths where = Helper.StorySearchPaths.All)
    {
        if (!Helper.loggedin && where != Helper.StorySearchPaths.Own) where = Helper.StorySearchPaths.NoWorkshop;
        List<string> manifestPaths = new List<string>();
        foreach (string story in Stories.GetAllStoryPaths(args, nsfw, searchTerm, where))
        {
            if (!File.Exists(Path.Combine(story, "manifest.json")))
                Debug.LogWarning($"Manifest/GetAll: Tried getting manifest for {story} which does not exist.");
            else if (Get(Path.Combine(story, "manifest.json")) != null)
                manifestPaths.Add(Path.Combine(story, "manifest.json"));
        }
        return manifestPaths.ToArray();
    }
    public static void FixSteamID()
    {
        if (!Helper.loggedin) return;
        string[] stories = Stories.GetAllStoryPaths(Helper.StorySearchArgs.Alphabetical, true, "", Helper.StorySearchPaths.Workshop, true);
        foreach (string path in stories)
        {
            string manifestPath = Path.Combine(path, "manifest.json");
            if (File.Exists(manifestPath))
            {
                Manifest data = Get(manifestPath);
                if (data.id != Path.GetFileName(Path.GetDirectoryName(manifestPath)))
                {
                    try
                    {
                        data.id = Path.GetFileName(Path.GetDirectoryName(manifestPath));
                        Write(manifestPath, data);
                        Debug.Log($"Fixed Steam ID for visual novel at path {manifestPath} successfully.");
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"Something went wrong when trying to fix Steam ID for story {manifestPath}.\nError: {e}");
                    }
                }
            }
        }
    }
    /// <summary>
    /// Replace a manifest
    /// </summary>
    /// <param name="path">Path to manifest</param>
    /// <param name="data">Data to replace with</param>
    /// <returns>Status</returns>
    public static bool Write(string path, Manifest data)
    {
        try
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, data);
            }
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Manifest/Write: Could not modify manifest {path}, error: \n{e}");
            return false;
        }
    }
}
/// <summary>
/// Visual Novel design file
/// </summary>
public class StoryDesign
{
    public string namecolor = ColorUtility.ToHtmlStringRGB(Colors.UnityGray);
    public string textcolor = ColorUtility.ToHtmlStringRGB(Colors.DarkPurple); 
    public string questioncolor = ColorUtility.ToHtmlStringRGB(Colors.IngameBlue); 
    public string questiontextcolor = ColorUtility.ToHtmlStringRGB(Colors.UnityGray);
    public static StoryDesign Get()
    {
        string path = Path.Combine(Helper.currentStoryPath, "design.json");
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogWarning($"StoryLayout/Get: {Helper.currentStoryPath} does not have a design manifest, continuing with default.");
            return new StoryDesign();
        }
        try
        {
            return JsonConvert.DeserializeObject<StoryDesign>(File.ReadAllText(path));
        }
        catch
        {
            UnityEngine.Debug.LogError($"StoryLayout/Get: Something went wrong when converting manifest \"{path}/design.json\". Is it setup correctly?");
            return null;
        }
    }
}

