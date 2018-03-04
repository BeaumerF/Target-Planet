using System;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stuffinder
{
    class Analyze
    {
        private VisionServiceClient visionClient;
        public AnalysisResult result { get; private set; }

        public Analyze()
        {
            visionClient = new VisionServiceClient("", @"https://westeurope.api.cognitive.microsoft.com/vision/v1.0");
        }

        public async Task<Description> AnalyzePictureAsync(Stream inputFile)
        {
            if (inputFile.Length == 0)
                return null;
            var _res = await visionClient.DescribeAsync(inputFile);
            return (_res.Description);
        }
    }
}