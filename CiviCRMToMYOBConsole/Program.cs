﻿using CiviCRMToMYOB;
using Fclp;

namespace CiviCRMToMYOBConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileName = "";
            var outputFileName = "";

            var parser = new FluentCommandLineParser();
            parser.Setup<string>('i').Callback(val => inputFileName = val);
            parser.Setup<string>('o').Callback(val => outputFileName = val);

            parser.Parse(args);

            var text = System.IO.File.ReadAllText(inputFileName);

            var outfile = new System.IO.StreamWriter(outputFileName);
            outfile.Write(new FormatConverter().Convert(text));

            outfile.Flush();
        }
    }
}
