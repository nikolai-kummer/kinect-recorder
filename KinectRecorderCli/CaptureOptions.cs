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
        public int Frequency { get; set; } = 50; //Capture Frequency in ms
    }
}
