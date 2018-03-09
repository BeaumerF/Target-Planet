using System;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text;

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

        public async Task<String> AnalyzeTextAsync(Stream inputFile)
        {
            if (inputFile.Length == 0)
                return null;
            var _res = await visionClient.RecognizeTextAsync(inputFile);
            var txt = GetRetrieveText(_res);

            return (txt);
        }

        public string GetRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (results != null && results.Regions != null)
            {
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}