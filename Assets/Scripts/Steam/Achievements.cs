﻿using Steamworks.Data;
using System;
using UnityEngine;

namespace SajberSim.Steam
{
    public class Achievements : MonoBehaviour
    {
        public enum List
        {
            ACHIEVEMENT_findblush,
            ACHIEVEMENT_play1,
            ACHIEVEMENT_finish1,
            ACHIEVEMENT_finish5,
            ACHIEVEMENT_finish15,
            ACHIEVEMENT_finish13,
            ACHIEVEMENT_finish100,
            ACHIEVEMENT_download,
            ACHIEVEMENT_create,
            ACHIEVEMENT_publish1,
            ACHIEVEMENT_publish10,
            ACHIEVEMENT_100questions,
            ACHIEVEMENT_500questions,
            ACHIEVEMENT_setname,
            ACHIEVEMENT_miohonda,
            ACHIEVEMENT_menuspin,
            ACHIEVEMENT_imfabina,
            ACHIEVEMENT_20piano
        }

        public static void Grant(List achievement)
        {
            if (!Helper.Helper.loggedin) return;
            try
            {
                Achievement ach = new Achievement(achievement.ToString());
                ach.Trigger(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Steam/Achievements/Grant: Could not grant achievement {achievement.ToString()}.\nError: {e}");
            }
        }

        public void GrantName(string name)
        {
            Achievement ach = new Achievement(name);
            ach.Trigger(true);
        }
    }
}