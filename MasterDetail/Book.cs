using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using ZXing;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MasterDetail
{
	public class Book
	{
		// member variables
		public string title { get; set; }
		public string publisher { get; set; }
		public string isbn { get; set; }
		public string summary { get; set; }
		public string author { get; set; }
		public string coverstring { get; set; }
		public DateTime dateadded { get; set; }

		public Book()
		{
			// null constructor just for testing
		}

		public Book (string isbn)
		{
			viewInformation (isbn);
			this.coverstring = "http://covers.openlibrary.org/b/isbn/" + this.isbn+ "-L.jpg";

			// get the date/time the book was added
			var thisdate = new DateTime ();
			thisdate = DateTime.Now.ToLocalTime ();
			this.dateadded = thisdate;
		}

		private void viewInformation(string resultText)
		{
			// make the url
			string url = "http://isbndb.com/api/v2/json/2DEB0A3O/book/" + resultText;

			// httpwebrequest example altered to fit with isbndb
			var request = HttpWebRequest.Create(string.Format(@url));
			request.ContentType = "application/json";
			request.Method = "GET";

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				if (response.StatusCode != HttpStatusCode.OK)
					Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					if(string.IsNullOrWhiteSpace(content)) 
					{
						Console.Out.WriteLine("Response contained empty body...");
					}
					else 
					{
						Console.Out.WriteLine("Response Body: \r\n {0}", content);

						// call function to parse data
						Console.Out.WriteLine ("JSON: " + content);
						parseJSON (content);
					}
				}
			}
		}

		private void parseJSON(string content)
		{
			try
			{
				JObject wholeThing = JObject.Parse (content);
				JToken data = wholeThing ["data"];
				Console.Out.WriteLine ("data: \n" + data.ToString ());

				// because they have a stupid array format
				JToken bookInfo = data [0];
				JToken bookSummary = bookInfo ["summary"];
				JToken authorData = bookInfo["author_data"];

				// assign the member
				this.isbn = bookInfo["isbn13"].ToString();
				this.publisher = bookInfo["publisher_text"].ToString();
				this.summary = bookInfo["summary"].ToString();
				JToken authorName = authorData[0];
				this.author = authorName["name"].ToString();

				// process the title for correct capitalization
				//this.title = bookInfo["title_latin"].ToString();
				string booktitle = bookInfo["title_latin"].ToString();
				booktitle = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(booktitle.ToLower());
				this.title = booktitle;

				// print statements just to test:
				Console.Out.WriteLine("Title: " + this.title);
				Console.Out.WriteLine("Author: " + this.author);
				Console.Out.WriteLine("Summary: " + this.summary);
				Console.Out.WriteLine("Publisher: " + this.publisher);
				Console.Out.WriteLine("ISBN: " + this.isbn);
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("parseJson error: " + ex.ToString ());
			}

		}

		// getter functions
		public string getTitle()
		{
			return this.title;
		}

		public string getPublisher()
		{
			return this.publisher;
		}

		public string getIsbn()
		{
			return this.isbn;
		}

		public string getSummary()
		{
			return this.summary;
		}

		public string getAuthor()
		{
			return this.author;
		}

		public string getCoverString()
		{
			return this.coverstring;
		}

		public DateTime getDateAdded()
		{
			return this.dateadded;
		}
	}
}

