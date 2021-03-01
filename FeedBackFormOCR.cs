using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Amazon.Textract;
using Amazon.Textract.Model;
using ConsoleTables;
using Newtonsoft.Json;

namespace FeedbackOCR
{
    internal class FeedBackFormOCR
    {
        private readonly IAmazonTextract textract;

        public FeedBackFormOCR(IAmazonTextract textract)
        {
            this.textract = textract;
        }

        public DetectDocumentTextResponse DetectTextLocalFile(string path)
        {
            var request = new DetectDocumentTextRequest();
            var response = new DetectDocumentTextResponse();
            Console.WriteLine(path);
            if (File.Exists(path))
            {
                Console.WriteLine("Created");
                request.Document = new Document
                {
                    Bytes = new MemoryStream(File.ReadAllBytes(path))
                };
                response = textract.DetectDocumentText(request);
                return response;
            }

            Console.WriteLine("Not Created");
            return response;
        }

        public void PrintResponse(DetectDocumentTextResponse response)
        {
            var fullTextResponse = "";
            if (response != null)
            {
                Console.WriteLine("Response is not empty");
                response.Blocks.ForEach(block =>
                {
                    if (block.BlockType == "LINE")
                        //Console.WriteLine(block.Text);
                        fullTextResponse += block.Text;
                });
                //Console.WriteLine(fullTextResponse);
                var res = ExtractContentFromResponse(fullTextResponse);

                var table = new ConsoleTable("Field", "Value");
                table.AddRow("Provider Name", res["ProviderFullName"]);
                table.AddRow("Review", res["ReviewSection"]);
                table.Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write(Format.Alternative);
            }
            else
            {
                Console.WriteLine("Response is empty");
            }
        }


        public string CreateJson(DetectDocumentTextResponse response)
        {
            var fullTextResponse = "";
            if (response != null)
            {
                response.Blocks.ForEach(block =>
                {
                    if (block.BlockType == "LINE")
                        //Console.WriteLine(block.Text);
                        fullTextResponse += block.Text;
                });

                var res = ExtractContentFromResponse(fullTextResponse);
                Console.WriteLine(JsonConvert.SerializeObject(res, Formatting.Indented));
                return JsonConvert.SerializeObject(res, Formatting.Indented);
            }

            return JsonConvert.SerializeObject(null);
        }

        private Dictionary<string, string> ExtractContentFromResponse(string fullTextResponse)
        {
            var textReviewRegex = new Regex(@"(?<=about your visit)(?<Review>.*)(?=Would you recommend this)");
            var providerFullNameRegex =
                new Regex(@"(?<=Provider first and last name)(?<ProviderName>.*)(?=Patient)");
            var textReviewResults = textReviewRegex.Match(fullTextResponse);
            var providerFullNameResults = providerFullNameRegex.Match(fullTextResponse);

            var reviewText = textReviewResults.Success
                ? textReviewResults.Groups["Review"].Value
                : "Cannot Parse Review Section";
            var providerFullName = providerFullNameResults.Success
                ? providerFullNameResults.Groups["ProviderName"].Value
                : "Cannot Parse Provider Name ";
            var extractedContent = new Dictionary<string, string>();
            extractedContent.Add("ProviderFullName", providerFullName);
            extractedContent.Add("ReviewSection", reviewText);
            return extractedContent;
        }
    }
}
