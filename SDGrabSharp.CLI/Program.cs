﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SDGrabSharp.CLI.Resources;
using SchedulesDirect;
using SDGrabSharp.Common;

namespace SDGrabSharp.CLI
{
    class Program
    {
        private static Config config;
        private static DataCache cache;
        private static XmlTVBuilder builder;
        private static bool needcr;
        private static string version;

        private class localArgs
        {
            public string configFile;
            public List<string> xmlTVList;
            public bool quiet;

            public localArgs()
            {
                xmlTVList = new List<string>();
                quiet = false;
                configFile = "SDGrabSharp.xml";
            }
        }

        static void Main(string[] args)
        {
            version = typeof(SDGrabSharp.CLI.Program).Assembly.GetName().Version.ToString();

            // Read arguments
            var argData = processArgs(args);

            // Check specified config file exists
            if (!File.Exists(argData.configFile))
            {
                Console.WriteLine(string.Format(Strings.ConfigNotFound, argData.configFile));
                Environment.Exit(1);
            }

            // Check any specified XMLTV files exist
            bool badXmlTV = false;
            foreach(var xmlTvFile in argData.xmlTVList)
            {
                if (!File.Exists(xmlTvFile))
                {
                    Console.WriteLine(string.Format(Strings.XmlTVFileNotFound, xmlTvFile));
                    badXmlTV = true;
                }
            }
            if (badXmlTV)
                Environment.Exit(1);

            // Initialize objects
            config = new Config();
            cache = new DataCache();

            // Load configuration
            LoadConfig(argData.configFile);

            // Initialize XMLTV builder
            builder = new XmlTVBuilder(config, cache);

            // If not quiet mode, add feedback event
            if (!argData.quiet)
                builder.ActivityLogUpdate += updateActivityLog;
            needcr = false;

            // Initialize data
            builder.Init();

            // Load existing XML TV file
            builder.LoadXmlTV(config.XmlTVFileName);

            // Run process
            builder.RunProcess();

            // Import extra xml files
            foreach(var xmlTvFile in argData.xmlTVList)
            {
                if (!builder.mergeXmlTV(xmlTvFile))
                {
                    if (builder.GetXmlTVErrors().Count() > 0)
                    {
                        foreach(var error in builder.GetXmlTVErrors())
                        {
                            Console.WriteLine(string.Format("{0}: {1}", error.code, error.description));
                        }
                    }
                    Console.WriteLine(string.Format(Strings.XmlTVMergeFailed, xmlTvFile));
                    Environment.Exit(1);
                }
            }

            // Save final XMLTV
            builder.SaveXmlTV();

            if (needcr)
                Console.WriteLine("");
            Console.WriteLine(Strings.ProcessComplete);
            Environment.Exit(0);
        }

        private static localArgs processArgs(string[] args)
        {
            var argData = new localArgs();

            // Parse arguments
            bool argMode = false;
            bool invalidArg = false;
            string argString = "";
            foreach (string arg in args)
            {
                // If we were waiting for an argument parameter, process it now
                if (argMode)
                {
                    // Switch on previous argument name
                    switch (argString.ToUpper().Trim())
                    {
                        case "CONFIG":
                            argData.configFile = arg;
                            break;
                        case "IMPORTXMLTV":
                            argData.xmlTVList.Add(arg);
                            break;
                        default:
                            break;
                    }
                    argMode = false;
                }
                else if (arg.Substring(0, 2) == "--")
                {
                    // If argument begins with "--" process it as an argument
                    argString = arg.Substring(2, arg.Length - 2);

                    // Get argument text, and test against list
                    switch (argString.ToUpper().Trim())
                    {
                        case "CONFIG":
                        case "IMPORTXMLTV":
                            // We'll get the output mode from the next argument
                            argMode = true;
                            break;
                        case "QUIET":
                            // Set replace mode to true (replace original file)
                            argData.quiet = true;
                            argMode = false;
                            break;
                        case "HELP":
                            // Show help info
                            argMode = false;
                            DoUsage(true);
                            Environment.Exit(0);
                            break;
                        default:
                            invalidArg = true;
                            break;
                    }
                }
                else
                {
                    invalidArg = true;
                }
            }

            // Handle invalid arguments, by showing window with usage
            if (invalidArg)
            {
                DoUsage(true);
                Environment.Exit(1);
            }
            return argData;
        }

        private static void DoUsage(bool helpMode = false)
        {
            Console.WriteLine(string.Format(Strings.HelpText, version));
        }

        private static void updateActivityLog(object sender, XmlTVBuilder.ActivityLogEventArgs args)
        {
            if (needcr)
                Console.WriteLine(string.Empty);
            Console.WriteLine(args.ActivityText);
            needcr = false;
        }

        private static void LoadConfig(string filename = null)
        {
            string fileName = filename != null ? filename : "SDGrabSharp.xml";
            if (!config.Load(fileName))
            {
                Console.WriteLine(string.Format(Strings.TooManyArgs, fileName));
                Environment.Exit(1);
            }
        }
    }
}
