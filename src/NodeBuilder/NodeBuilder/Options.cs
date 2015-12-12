using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace NodeBuilder
{
    class Options
    {
        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints verbose logging to standard output.")]
        public bool Verbose { get; set; }

        [OptionList('d', "definition", Required = true, HelpText="Node definition file - a json file defining the node image to be created")]
        public string NodeDefinition { get; set; }

        [Option('o', "outputdir", Required = true, HelpText = "Directory where the generated project will be written. This directory be created if it does not exist, and emptied if it does.")]
        public string OutputDirectory { get; set; }

        [Option('b', "build", HelpText = "Build the solution")]
        public bool DoBuild { get; set; }

        [Option('f', "flavor", HelpText = "Debug or Release?")]
        public string Flavor { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        //[HelpOption]
        //public string GetUsage()
        //{
        //    return HelpText.AutoBuild(this,
        //      (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        //}
    }
}
