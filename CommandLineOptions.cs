namespace IndexAndSearchFileStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CommandLine;
    using CommandLine.Parsing;
    using CommandLine.Text;

    public class CommandLineOptions
    {
        [Option('i', "index", HelpText = "The path to store or access the index")]
        public string IndexFolderPath { get; set; }

        [Option('f', "folder", HelpText = "The folder to index (recursively)")]
        public string ToIndexFolderPath { get; set; }

        [Option('s', "search", HelpText = "Search an existing index")]
        public string SearchTerm { get; set; }

        [Option('c', "create", DefaultValue = false, HelpText = "Create a new index")]
        public bool Create { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Verbose")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
