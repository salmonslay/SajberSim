﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SajberSim.Web
{
    class Webhook
    {
        private static void Send(string url, string msg, string nameext, string avatar, string msgbase = "")
        {
            using (dWebHook dcWeb = new dWebHook())
            {
                dcWeb.profilepic = avatar;
                dcWeb.displayname = $"SajberSim {nameext}";
                dcWeb.url = url;
                dcWeb.SendMessage(msgbase + msg);
            }
        }
        public static void Log(string msg, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["log"], msg, "Log", avatar);
        }
        public static void Support(string msg, string email, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["support"], msg, "Support", avatar, $"**:triangular_flag_on_post: NEW SUPPORT REQUEST**\nSender: {email}\n\n ");
        }
        public static void Stats(string msg, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["stats"], msg, "Stats", avatar);
        }
    }
    public class dWebHook : IDisposable
    {
        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        public string url { get; set; }
        public string displayname { get; set; }
        public string profilepic { get; set; }

        public dWebHook()
        {
            dWebClient = new WebClient();
        }


        public void SendMessage(string msgSend)
        {
            discordValues.Set("username", displayname);
            discordValues.Set("avatar_url", profilepic);
            discordValues.Set("content", msgSend);

            try
            {
                dWebClient.UploadValues(url, discordValues);
            }
            catch (WebException e)
            {
                UnityEngine.Debug.LogError($"Webhook: Tried to send message with text {msgSend} failed, error message:\n\n{e}");
            }
            discordValues = new NameValueCollection();
        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }
    }
}
