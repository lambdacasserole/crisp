﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Crisp.Configuration;
using Crisp.Core;
using Crisp.Core.Evaluation;
using Crisp.Core.Parsing;
using Crisp.Core.Preprocessing;
using Crisp.Core.Tokenizing;
using Crisp.Core.Types;
using Crisp.Visualization;

using SimpleInjector;

using CommandLine.Text;
using Crisp.Core.Runtime;

namespace Crisp
{
    class Program
    {
        /// <summary>
        /// Prints the help card for the application.
        /// </summary>
        /// <param name="options">The command-line options passed to the program.</param>
        private static void PrintHelp(Options options)
        {
            // Write ASCII art.
            var titleCard = new[]
            {
                "      __                         ",
                "    /    )         ,             ",
                "   /        )__       __      __ ",
                "  /        /   ) /   (_ `   /   )",
                " (____/___/_____/___(__)___/___/ ",
                "                          /      ",
                "                         /       ",
            };
            foreach (var line in titleCard)
            {
                Console.WriteLine(line);
            }

            var version = Assembly.GetEntryAssembly().GetName().Version;

            // Automatically build help text based on options class.
            var helpText = HelpText.AutoBuild(options);
            helpText.AdditionalNewLineAfterOption = true;
            helpText.Heading = $"Interpreter v{version}";
            helpText.Copyright = "Copyright © Saul Johnson 2015";

            Console.Write(helpText);
        }
        
        static void Main(string[] args)
        {
            // Parse command-line options.
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                PrintHelp(options);
                return;
            }

            // Check file exists.
            if (!File.Exists(options.InputFile))
            {
                PrintHelp(options);
                Console.WriteLine("Could not find input file.");
                return;
            }

            // Get runtime and run program.
            var runtime = CrispRuntimeFactory.GetCrispRuntime(options.InputFile);
            var output = runtime.EvaluateSourceFile(options.InputFile, options.Args);
            
            // Write result to output.
            Console.Write(new LispSerializer().Serialize(output));
        }
    }
}




























































