﻿using SajberSim.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Language
{
    public static NumberFormatInfo Format = new NumberFormatInfo();
    public static CultureInfo Culture = new CultureInfo("en-us");
    static Language()
    {
        Format.NumberDecimalSeparator = ".";
        Culture = new CultureInfo("en-us");
    }
    public Language(string code, string name, string formal, string localized, string lcid)
    {
        iso_code = code;
        language_code = name;
        formal_name = formal;
        localized_name = localized;
        LCID_code = lcid;
    }

    public string iso_code; //DK, the code used for flags
    public string language_code; //danish, the code used for steam
    public string formal_name; //Danish, formal name in english
    public string localized_name; //Dansk, formal name in original language
    public string LCID_code; //da
    public static Language[] list =
        {
        new Language("US", "english", "English", "English", "en-us"),
        new Language("ARAB_LEAGUE", "arabic", "Arabic", "عربى", "ar-ae"),
        new Language("BG", "bulgarian", "Bulgarian", "Български", "bg"),
        new Language("CN", "schinese", "Chinese", "中文", "zh-cn"),
        new Language("CZ", "czech", "Czech", "čeština", "cs"),
        new Language("DK", "danish", "Danish", "Dansk", "da"),
        new Language("NL", "dutch", "Dutch", "Nederlands", "nl-nl"),
        new Language("FI", "finnish", "Finnish", "Soumi", "fi"),
        new Language("FR", "french", "French", "Français", "fr-fr"),
        new Language("DE", "german", "German", "Deutsch", "de-de"),
        new Language("GR", "greek", "Greek", "Ελληνικά", "el"),
        new Language("HU", "hungarian", "Hungarian", "Magyar", "hu"),
        new Language("IT", "italian", "Italian", "Italiano", "it"),
        new Language("JP", "japanese", "Japanese", "日本語", "ja"),
        new Language("KR", "koreana", "Korean", "한국어", "ko"),
        new Language("NO", "norwegian", "Norwegian", "Norsk", "no-no"),
        new Language("PL", "polish", "Polish", "Polski", "pl"),
        new Language("PT", "portuguese", "Portuguese", "Português", "pt-pt"),
        new Language("BR", "brazilian", "Portuguese (Brazil)", "Português-Brasil", "pt-br"),
        new Language("RO", "romanian", "Romanian", "Limba română", "ro"),
        new Language("RU", "russian", "Russian", "русский", "ru"),
        new Language("ES", "spanish", "Spanish", "Español", "es-es"),
        new Language("SE", "swedish", "Swedish", "Svenska", "sv-se"),
        new Language("TH", "thai", "Thai", "ไทย", "th"),
        new Language("TR", "turkish", "Turkish", "Türkçe", "tr"),
        new Language("UA", "ukrainian", "Ukrainian", "українська мова", "uk"),
        new Language("VN", "vietnamese", "Vietnamese", "Tiếng Việt", "vi")
        };
    public static List<string> ListEnglishName()
    {
        List<string> lang = new List<string>();
        foreach (Language language in list)
            lang.Add(language.formal_name);
        return lang;
    }
    public static List<string> ListLocalizedName()
    {
        List<string> lang = new List<string>();
        foreach (Language language in list)
            lang.Add(language.localized_name);
        return lang;
    }
    public static List<string> ListFlagCode()
    {
        List<string> lang = new List<string>();
        foreach (Language language in list)
            lang.Add(language.iso_code);
        return lang;
    }
    public static List<string> ListSteamName()
    {
        List<string> lang = new List<string>();
        foreach (Language language in list)
            lang.Add(language.language_code);
        return lang;
    }
}

