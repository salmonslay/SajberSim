﻿using Newtonsoft.Json;
using SajberSim.Chararcter;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SajberSim.SaveSystem
{
    public class Save
    {
        public string novelname;
        public string path;

        public string script;
        public int line;
        public string username;
        public PersonSave[] characters;
        public Person[] charconfig;
        public string background;
        public string music;
        public DateTime date;
        public string textcolor;
        public string splashcolor;

        public Save(string npath, string nnovelname, string nscript, int nline, string nusername, PersonSave[] ncharacters, string nbackground, string nmusic)
        {
            path = npath;
            novelname = nnovelname;
            script = nscript;
            line = nline;
            username = nusername;
            characters = ncharacters;
            background = nbackground;
            music = nmusic;
        }

        public Save()
        {
        }

        public static Save Get(string path)
        {
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Saves/Get: Tried getting save file for path \"{path}\" which does not exist");
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<Save>(File.ReadAllText(path));
            }
            catch
            {
                UnityEngine.Debug.LogError($"Saves/Get: Something went wrong when converting save file \"{path}\". There is a possibility it have been modified.");
                return null;
            }
        }

        /// <summary>
        /// Returns paths to all save files
        /// </summary>
        public static string[] GetAllPaths()
        {
            return Directory.GetFiles(Path.Combine(Application.dataPath, "Saves"), "*.save");
        }

        public static Save[] GetAll()
        {
            List<Save> saves = new List<Save>();
            foreach (string path in GetAllPaths())
            {
                if (File.Exists(path))
                {
                    Save s = Get(path);
                    if (s != null)
                        saves.Add(s);
                }
            }
            return saves.ToArray();
        }

        public static void Create(Save savefile, int id, bool isNew)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(Path.Combine(Helper.Helper.savesPath, id + ".save")))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, savefile);
                }
                string thumbnailPath = Path.Combine(Helper.Helper.savesPath, id + ".png");
                if (File.Exists(thumbnailPath)) File.Delete(thumbnailPath);
                File.Copy(Path.Combine(Application.temporaryCachePath, "lastGame.png"), thumbnailPath);
                Debug.Log($"Saves/Create: Game saved successfully with ID {id}.");
                if (isNew) GameObject.Find("Canvas/SaveLoadMenu").GetComponent<SaveMenu>().UpdateMenu();
            }
            catch (Exception e)
            {
                Debug.LogError($"Saves/Create: Something went wrong when trying to save the game.\n{e}");
            }
        }
    }
}