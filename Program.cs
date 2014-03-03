
namespace IndexAndSearchFileStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommandLine;
    using CommandLine.Parsing;
    using CommandLine.Text;

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();

            if (Parser.Default.ParseArguments(args, options))
            {
                if ((!string.IsNullOrEmpty(options.IndexFolderPath)) && (options.Create) && (!string.IsNullOrEmpty(options.ToIndexFolderPath)))
                {
                    IndexHelpers.CreateOrAppendIndex(options.IndexFolderPath, options.ToIndexFolderPath, options.Verbose);
                    return;
                }

                if ((!string.IsNullOrEmpty(options.IndexFolderPath)) && (!string.IsNullOrEmpty(options.SearchTerm)))
                {
                    IndexHelpers.SearchIndex(options.IndexFolderPath, options.SearchTerm);
                    return;
                }

                Console.WriteLine("Invalid combination of options entered.");

            }
        }
    }
}
