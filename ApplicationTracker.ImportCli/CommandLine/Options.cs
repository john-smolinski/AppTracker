﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.ImportCli.CommandLine
{
    public class Options
    {
        [Option('t', "titles", Required = false, HelpText = "Evaluate Job Titles")]
        public bool JobTitles { get; set; }

        [Option('l', "locations", Required = false, HelpText = "Evaluate Locations")]
        public bool Locations { get; set; }

        [Option('o', "organizations", Required = false, HelpText = "Evaluate Organizations")]
        public bool Organizations { get; set; }

        [Option('s', "sources", Required = false, HelpText = "Evaluate Sources")]
        public bool Sources { get; set; }

        [Option('e', "environments", Required = false, HelpText = "Evaluate WorkEnvironments")]
        public bool WorkEnvironments { get; set; }

        [Option('a', "all", Required = false, HelpText = "Evaluate all entities")]
        public bool All { get; set; }

        [Option('r', "report", Required = false, HelpText = "Report entity status")]
        public bool Report { get; set; }

        [Option('x', "execute", Required = false, HelpText = "Execute selected migrations")]
        public bool Execute { get; set; }
    }
}
