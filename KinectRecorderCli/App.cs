using LightInject;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectRecorder.Cli.Commands;


namespace KinectRecorder.Cli
{
    class App
    {
        public class OptionsParser
        {
            [Option('c', "command", Required = false, HelpText = $"Command indicating which capture. Supported: {Constants.Commands.ImageCommand}, {Constants.Commands.ImuCommand}")]
            public string Command { get; set; } = Constants.Commands.ImageCommand;

            [Option('f', "filename", Required = false, HelpText = "Full path to filename where output will be saved. Default same folder with date")]
            public string? Filename { get; set; } = null;

            [Option('t', "freq", Required = false, HelpText = "Imu polling frequencu in ms")]
            public int? Frequency { get; set; } = 50;

            [Option('d', "duration", Required = false, HelpText = "Imu capture duration in ms")]
            public int? Duration { get; set; } = 2000;
        }

        public App()
        {
            //Constructor
        }

        internal async Task<int> ExecuteAsync(string[] args)
        {
            int returnVal = -1;
            Parser.Default.ParseArguments<OptionsParser>(args)
                   .WithParsed(async o =>
                   {
                       var cliOptions = new CaptureOptions()
                       {
                           Filename = o.Filename,
                           ImuDuration = (int)o.Duration,
                           ImuFrequency = (int)o.Frequency
                       };

                       switch (o.Command)
                       {
                           case Constants.Commands.ImageCommand:
                               returnVal = await Image.ImageCapture(cliOptions);
                               break;
                           case Constants.Commands.ImuCommand:
                               returnVal = await Imu.ImuCapture(cliOptions);
                               break;
                       }
                   });
            return 1;
        }
    }
}
