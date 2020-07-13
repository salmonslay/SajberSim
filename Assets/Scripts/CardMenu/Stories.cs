﻿using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SajberSim.Helper.Helper;

namespace SajberSim.CardMenu
{
    public class Stories
    {
        private static string[] storyPaths;
        //if the path list above needs to be updated or if it is up-to-date already
        public static bool pathUpdateNeeded = true;

        /// <summary>
        /// Returns paths to all story folders, eg app/Story/OpenHouse. Main method for most stuff here
        /// </summary>
        /// <param name="args">Search arguments</param>
        /// <param name="nsfw">Include NSFW</param>
        /// <returns>Array with paths to all local story folders</returns>
        public static string[] GetAllStoryPaths(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!pathUpdateNeeded) return Stories.storyPaths;
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;

            List<string> storyPaths = new List<string>();
            //This is what I call "The tired" ~
            //update, apparently the 5 line method i had here before wasn't the problem. oh well goodnight
            if (where == StorySearchPaths.Workshop)
            {
                storyPaths = Directory.GetDirectories(steamPath).ToList();
            }
            else if (where == StorySearchPaths.Local)
            {
                storyPaths = Directory.GetDirectories(localPath).ToList();
            }
            else if (where == StorySearchPaths.NoWorkshop)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
            }
            else if (where == StorySearchPaths.All)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(steamPath).ToList());
            }
            else if (where == StorySearchPaths.Own)
            {
                storyPaths.AddRange(Directory.GetDirectories(customPath).ToList());
                storyPaths.AddRange(Directory.GetDirectories(localPath).ToList());
                if (loggedin)
                    storyPaths.AddRange(Directory.GetDirectories(steamPath).ToList());
            }

            //Sort the list
            string[] fixedPaths = SortArrayBy(storyPaths, args);

            if (!nsfw) //remove nsfw if needed
                fixedPaths = FilterNSFWFromCardPaths(fixedPaths.ToList());
            if (searchTerm != "")
                fixedPaths = FilterSearchFromCardPaths(fixedPaths.ToList(), searchTerm);
            if (where == StorySearchPaths.Own)
                fixedPaths = FilterNonOwnedFromCardPaths(fixedPaths.ToList());
            Stories.pathUpdateNeeded = false;
            Stories.storyPaths = fixedPaths;
            return fixedPaths;
        }
        /// <summary>
        /// Takes array of story paths and sorts it by data in manifest
        /// </summary>
        private static string[] SortArrayBy(List<string> storyPaths, StorySearchArgs args)
        {
            if (args == StorySearchArgs.ID) return storyPaths.ToArray();

            bool reverse = false;
            if (args == StorySearchArgs.ReverseAlphabetical || args == StorySearchArgs.Newest || args == StorySearchArgs.LongestFirst || args == StorySearchArgs.LastModified) reverse = true;
            List<StorySort> itemList = new List<StorySort>();

            //Add everything to a list
            foreach (string path in storyPaths)
            {
                if (File.Exists($"{path}/manifest.json"))
                {
                    Manifest storydata = Manifest.Get($"{path}/manifest.json");
                    if (storydata != null)
                    {
                        if (args == StorySearchArgs.Alphabetical || args == StorySearchArgs.ReverseAlphabetical)
                            itemList.Add(new StorySort(path, storydata.name));
                        else if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.ShortestFirst)
                            itemList.Add(new StorySort(path, storydata.playtime));
                        else if (args == StorySearchArgs.Author)
                            itemList.Add(new StorySort(path, storydata.author));
                        else if (args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest)
                            itemList.Add(new StorySort(path, storydata.publishdate));
                        else if (args == StorySearchArgs.LastModified)
                            itemList.Add(new StorySort(path));
                    }
                }
            }

            //Start sorting
            if (args == StorySearchArgs.LongestFirst || args == StorySearchArgs.Newest || args == StorySearchArgs.Oldest || args == StorySearchArgs.ShortestFirst || args == StorySearchArgs.LastModified) //playtime
                itemList = itemList.OrderBy(c => c.argint).ToList();
            if (args == StorySearchArgs.Alphabetical || args == StorySearchArgs.Author || args == StorySearchArgs.ReverseAlphabetical) //name || author
                itemList = itemList.OrderBy(c => c.argstring).ToList();

            //Done, now add paths
            List<string> sortedList = new List<string>();
            foreach (StorySort story in itemList)
            {
                sortedList.Add(story.thepath);
            }
            if (reverse) return ReverseArray(sortedList.ToArray());
            else return sortedList.ToArray();

        }
        private static string[] FilterNSFWFromCardPaths(List<string> storyPaths, bool remove = true) // https://i.imgur.com/Dw1l9YI.png
        {
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = Manifest.Get($"{path}/manifest.json");
                if (storydata.nsfw && remove) storyPaths.Remove(path);
                else if (!storydata.nsfw && !remove) storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        private static string[] FilterSearchFromCardPaths(List<string> storyPaths, string searchTerm)
        {
            searchTerm.ToLower();
            if (searchTerm == "nsfw") return FilterNSFWFromCardPaths(storyPaths, false);
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = Manifest.Get($"{path}/manifest.json");

                if (!storydata.name.ToLower().Contains(searchTerm) && !storydata.tags.Contains(searchTerm) && !storydata.description.ToLower().Contains(searchTerm) && !storydata.author.ToLower().Contains(searchTerm) && !storydata.genre.ToLower().Contains(searchTerm))
                    storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        /// <summary>
        /// Removes all story paths where the logged in owner not is the author
        /// </summary>
        public static string[] FilterNonOwnedFromCardPaths(List<string> storyPaths)
        {
            foreach (string path in storyPaths.ToList())
            {
                Manifest storydata = Manifest.Get($"{path}/manifest.json");
                if (storydata != null)
                {
                    if (storydata.author != SteamClient.Name && storydata.authorid != $"{SteamClient.SteamId}") storyPaths.Remove(path);
                }
                else
                    storyPaths.Remove(path);
            }
            return storyPaths.ToArray();
        }
        /// <summary>
        /// Returns names of all story folders
        /// </summary>
        public static string[] GetAllStoryNames(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            List<string> nameList = new List<string>();
            foreach (string path in GetAllStoryPaths(args, nsfw, searchTerm, where))
                nameList.Add(new DirectoryInfo(path).Name);

            return nameList.ToArray();
        }
        /// <summary>
        /// Returns amount of pages needed for the preview card menu
        /// </summary>
        public static int GetCardPages(StorySearchArgs args = StorySearchArgs.ID, bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            int length = Manifest.GetAll(args, nsfw, searchTerm, where).Length;
            if (length <= 6) return 0;
            else if (length % 6 == 0) return length / 6 - 1;
            return (length - (length % 6)) / 6;
        }
        /// <summary>
        /// Returns amount of cards that should be on the last page (indexed at 0)
        /// </summary>
        public static int GetLeftoverCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            if (!loggedin && where != StorySearchPaths.Own) where = StorySearchPaths.NoWorkshop;
            int n = Manifest.GetAll(StorySearchArgs.ID, nsfw, searchTerm, where).Length % 6;
            if (n == 0) return 6; //there shouldn't be 0 cards on the last page
            else return n;
        }
        /// <summary>
        /// Returns amount of cards in total
        /// </summary>
        public static int GetTotalCardAmount(bool nsfw = true, string searchTerm = "", StorySearchPaths where = StorySearchPaths.All)
        {
            return Manifest.GetAll(StorySearchArgs.ID, nsfw, searchTerm, where).Length;
        }
        /// <summary>
        /// Returns paths to all assets of specified type collected from all stories
        /// </summary>
        /// <param name="folder">Foldertype, eg. Characters</param>
        /// <returns>Array with all specified assets</returns>
        public static string[] GetAllStoryAssetPaths(string folder)
        {
            string[] validPaths = { "audio", "backgrounds", "characters", "dialogues" };
            List<string> assetPaths = new List<string>();
            folder = folder.ToLower();

            if (!validPaths.Contains(folder)) return new string[0];

            string extension = "*.png";

            switch (folder)
            {
                case "audio":
                    extension = "*.ogg";
                    break;
                case "dialogues":
                    extension = "*.txt";
                    break;
            }
            foreach (string story in GetAllStoryPaths())
            {
                string path = $"{story}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}";
                if (Directory.Exists(path))
                    assetPaths.AddRange(Directory.GetFiles(path, extension));
            }
            return assetPaths.ToArray();
        }
        public static string[] GetStoryAssetPaths(string folder, string path)
        {
            string[] validPaths = { "audio", "backgrounds", "characters", "dialogues" };
            List<string> assetPaths = new List<string>();
            folder = folder.ToLower();

            if (!validPaths.Contains(folder)) return new string[0];

            string extension = "*.png";

            switch (folder)
            {
                case "audio":
                    extension = "*.ogg";
                    break;
                case "dialogues":
                    extension = "*.txt";
                    break;
            }
            if (Directory.Exists($"{path}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}"))
                assetPaths.AddRange(Directory.GetFiles($"{path}/{Char.ToUpper(folder[0]) + folder.Remove(0, 1)}", extension));
            return assetPaths.ToArray();
        }
    }
}