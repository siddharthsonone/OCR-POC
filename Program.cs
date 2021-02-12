using System;
using Amazon.Textract;
using Microsoft.Extensions.Configuration;
using DetectText;

namespace Textract {
	partial class Program {

		const string BucketName = "bucket-textxtract-sample";
		const string S3File = "sample-image-2-redacted-form.png";

		static void Main(string[] args) {

			var builder = new ConfigurationBuilder()
				.SetBasePath(Environment.CurrentDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
			var awsOptions = builder.GetAWSOptions();
			Console.WriteLine(awsOptions.Profile + ":" + awsOptions.ProfilesLocation + ": " + awsOptions.Region.DisplayName);

			var textractTextService = new TextractTextDetection(awsOptions.CreateServiceClient<IAmazonTextract>());
            
            var getResults = textractTextService.DetectTextS3(BucketName, S3File);
			getResults.Wait();
			textractTextService.Print(getResults.Result);
		}
    }
}
