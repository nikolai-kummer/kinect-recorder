using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectRecorder.Cli
{
    public class CaptureOptions
    {
        public string? Filename { get; set; } = null; 
        public int ImuFrequency { get; set; } = 50; //Capture Frequency in ms
        public int ImuDuration { get; set; } = 3000; //Capture Frequency in ms
    }
}
