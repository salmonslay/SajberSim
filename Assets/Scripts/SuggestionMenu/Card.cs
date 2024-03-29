﻿using SajberSim.Steam;
using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using Steamworks.Ugc;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SajberSim.Suggestions
{
    public class Card : MonoBehaviour
    {
        public RawImage Thumbnail;
        public Text Description;
        public Text Title;
        public ulong id;
        private Download dl;
        private void Start()
        {
            dl = Download.Init();
        }
        private void Update()
        {
            transform.localPosition = new Vector3(150, transform.localPosition.y);
        }
        public async void FillData()
        {
            if (!Helper.Helper.loggedin) return;

            var itemInfo = await SteamUGC.QueryFileAsync(id);
            if (itemInfo?.PreviewImageUrl.Length == 0)
            {
                SetError();
                return;
            }
            Title.text = itemInfo?.Title;
            Description.text = itemInfo?.Description;
            dl.RawImage(Thumbnail.gameObject, itemInfo?.PreviewImageUrl);
        }
        public void OpenInSteam()
        {
            Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={id}");
        }
        private void SetError()
        {
            Title.text = "Error";
            Description.text = Translate.Get("recommendednovelerror");
        }
    }

}
