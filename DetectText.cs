using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.Textract;
using Amazon.Textract.Model;

namespace DetectText{
	public class TextractTextDetection{

		private IAmazonTextract textract;
		public TextractTextDetection(IAmazonTextract textract) {
			this.textract = textract;
		}

		public async Task<DetectDocumentTextResponse> DetectTextS3(string bucketName, string key) {
			var result = new DetectDocumentTextResponse();
			var s3Object = new S3Object {
				Bucket = bucketName,
				Name = key
			};
			var request = new DetectDocumentTextRequest();
			request.Document = new Document {
				S3Object = s3Object
			};
			return await this.textract.DetectDocumentTextAsync(request);
		}

		private void Print(List<Block> blocks) {
			blocks.ForEach(x => {
				if(x.BlockType.Equals("LINE")) {
					Console.WriteLine(x.Text);
				}
			});
		}

		public void Print(DetectDocumentTextResponse response) {
			if(response != null) {
				this.Print(response.Blocks);
			}
		}

		public void Print(List<GetDocumentTextDetectionResponse> response) {
			if(response != null && response.Count > 0) {
				response.ForEach(r => this.Print(r.Blocks));
			}
		}

	}
}