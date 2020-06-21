﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Language
{
    public Language(string code, string name, string formal, string localized)
    {
        flag_code = code;
        language_code = name;
        formal_name = formal;
        localized_name = localized;
    }

    public string flag_code;
    public string language_code;
    public string formal_name;
    public string localized_name;
    public static Language[] list =
        {
        new Language("US", "english", "English", "English"),
        new Language("AE", "arabic", "Arabic", "عربى"),
        new Language("BG", "bulgarian", "Bulgarian", "Български"),
        new Language("CN", "schinese", "Chinese (Simplified)", "中文"),
        new Language("CN", "tchinese", "Chinese (Traditional)", "中文"),
        new Language("CZ", "czech", "Czech", "čeština"),
        new Language("DK", "danish", "Danish", "Dansk"),
        new Language("NL", "dutch", "Dutch", "Nederlands"),
        new Language("FI", "finnish", "Finnish", "Soumi"),
        new Language("FR", "french", "French", "Français"),
        new Language("DE", "german", "German", "Deutsch"),
        new Language("GR", "greek", "Greek", "Ελληνικά"),
        new Language("HU", "hungarian", "Hungarian", "Magyar"),
        new Language("IT", "italian", "Italian", "Italiano"),
        new Language("JP", "japanese", "Japanese", "日本語"),
        new Language("KR", "koreana", "Korean", "한국어"),
        new Language("NO", "norwegian", "Norwegian", "Norsk"),
        new Language("PL", "polish", "Polish", "Polski"),
        new Language("PT", "portuguese", "Portuguese", "Português"),
        new Language("BR", "brazilian", "Portuguese (Brazil)", "Português-Brasil"),
        new Language("RO", "romanian", "Romanian", "Limba română"),
        new Language("RU", "russian", "Russian", "русский"),
        new Language("ES", "spanish", "Spanish", "Español"),
        new Language("SE", "swedish", "Swedish", "Svenska"),
        new Language("TH", "thai", "Thai", "ไทย"),
        new Language("TR", "turkish", "Turkish", "Türkçe"),
        new Language("UA", "ukrainian", "Ukrainian", "	українська мова "),
        new Language("VN", "vietnamese", "Vietnamese", "Tiếng Việt")
        };
}

