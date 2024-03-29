﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajberSim.Steam
{

    public class WorkshopData
    {
        public string title;
        public string description;
        public string genre;
        public string dataPath;
        public string lang;
        public string changenotes;
        public Workshop.Privacy privacy;
        public string rating;
        public Stopwatch st;
        public string originalPath;
        public long id = -1;
    }
}