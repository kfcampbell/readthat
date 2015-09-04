using System;
using System.Net;
using System.IO;
using System.Text;
using UIKit;
using System.Threading.Tasks;
using System.Threading;
using CoreGraphics;
using CoreImage;
using BigTed;
using FFImageLoading;
using Foundation;
using MessageUI;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace MasterDetail
{
	partial class BookViewController : UITableViewController
	{
		public Book DetailItem { get; set; }
		public WebClient webClient;

		public BookViewController (IntPtr handle) : base (handle)
		{
		}

		partial void editButton_TouchUpInside (UIButton sender)
		{
			Console.Out.WriteLine("Book View Controller Edit Button Pressed");
		}

		partial void priceButton_TouchUpInside (UIButton sender)
		{
			Console.Out.WriteLine("price button hit!");

			// load the amazon site if tapped
			if(!string.IsNullOrEmpty(DetailItem.isbn))
			{

				string url = "http://amazon.com/s?index=books&field-isbn=" + DetailItem.isbn;

				UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
			}
			else
			{
				Console.Out.WriteLine("Price button hit, but no pricing data available!");
			}
		}

		partial void summaryButton_TouchUpInside (UIButton sender)
		{
			string summaryString;
			string acceptString;

			if(DetailItem.getSummary() != String.Empty)
			{
				summaryString = DetailItem.getSummary();
				acceptString = "Looks Interesting";
			}
			else
			{
				summaryString = "The book's summary looks empty. Sorry about that.";
				acceptString = "Aww man";
			}
			var alert = UIAlertController.Create("Book Summary", summaryString, UIAlertControllerStyle.Alert);

			// add buttons
			alert.AddAction(UIAlertAction.Create(acceptString, UIAlertActionStyle.Default, null));

			// actually show the thing
			PresentViewController(alert, true, null);
		}


		// called every time the class is instantiated.
		public void SetDetailItem (Book newDetailItem)
		{

			if (DetailItem != newDetailItem) 
			{
				DetailItem = newDetailItem;

				this.Title = DetailItem.getTitle ();

				// set the photo until otherwise determined
				//coverImage.Image = UIImage.FromBundle ("unavailable.png");

				// Update the view
				ConfigureView ();
			}
		}

		// get all the data loaded into correct spots.
		async void ConfigureView ()
		{
			// short delay to make sure view is loaded. experiment with number of milliseconds
			await Task.Delay (100);

			// instantiate the share button
			var actionButton = new UIBarButtonItem (UIBarButtonSystemItem.Action, shareItem);
			//actionButton.AccessibilityLabel = "actionButton";
			NavigationItem.RightBarButtonItem = actionButton;

			// Update the user interface for the detail item
			if (IsViewLoaded && DetailItem != null)
			{
				this.Title = DetailItem.getTitle ();
				titleLabel.Text = DetailItem.getTitle ();
				authorLabel.Text = DetailItem.getAuthor ();
				dateAddedLabel.Text = "Added " + DetailItem.getDateAdded ().ToString ();
				publisherLabel.Text = DetailItem.getPublisher ();

				Console.Out.WriteLine ("detailitem : " + DetailItem.userimagepath);
				Console.Out.WriteLine ("detailitem: " + DetailItem.usesuserphoto);


				// attempt to download the cover photo
				downloadAsync ();

				Console.Out.WriteLine ("View updates done.");
			}

			// put some filler text in if the publisher isn't filled out
			if(DetailItem.getPublisher() == string.Empty)
			{
				publisherLabel.Text = "No publisher information available.";
			}

			//getPrice ();

			if(string.IsNullOrEmpty(DetailItem.isbn))
			{
				Console.Out.WriteLine ("test book. no pricing data available.");
				priceButton.SetTitle ("Sorry, no pricing available", UIControlState.Normal);
			}
			else
			{
				priceButton.SetTitle ("Tap to open Safari for Amazon pricing", UIControlState.Normal);
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "editBook") 
			{
				Console.Out.WriteLine ("Edit book segue entered.");

				// attempt to set new book
				//ManualController.newBook = DetailItem;
				((ManualController)segue.DestinationViewController).SetBookItem (DetailItem);
				Console.Out.WriteLine ("edit book detail set: " + DetailItem.getTitle ());
			}
		}

		public void shareItem(object sender, EventArgs e)
		{
			Console.Out.WriteLine ("Share button pressed!");

			// Create a new Alert Controller
			UIAlertController actionSheetAlert = UIAlertController
				.Create("Share via Email or SMS", "Note that I'm not allowed to populate the text message for you :(", UIAlertControllerStyle.ActionSheet);

			// Add Actions
			actionSheetAlert.AddAction(UIAlertAction
				.Create("Share via Email",UIAlertActionStyle.Default, (action) => shareEmail()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Share via text/iMessage",UIAlertActionStyle.Default, (action) => shareText()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Cancel",UIAlertActionStyle.Cancel, (action) => Console.WriteLine ("Cancel button pressed.")));


			// Display the alert
			this.PresentViewController(actionSheetAlert,true,null);
		}

		public void shareEmail()
		{
			MFMailComposeViewController mailController;
			string messageBody = "Hey, you should consider reading " + DetailItem.title + " by " + DetailItem.author + ".";

			if (MFMailComposeViewController.CanSendMail) 
			{
				mailController = new MFMailComposeViewController ();

				mailController.SetSubject ("Interesting Book");
				mailController.SetMessageBody (messageBody, false);

				mailController.Finished += ( object s, MFComposeResultEventArgs args) => 
				{
					Console.WriteLine (args.Result.ToString ());
					args.Controller.DismissViewController (true, null);
				};

				this.PresentViewController (mailController, true, null);
			}
		}

		public void shareText()
		{
			var smsTo = NSUrl.FromString("sms:");
			UIApplication.SharedApplication.OpenUrl(smsTo);
		}

		// framework for downloading cover image. eventually store it and don't do it every time.
		async void downloadAsync()
		{
			if (!DetailItem.usesuserphoto) 
			{
				try
				{
					BTProgressHUD.Show ("Retrieving Cover...");
					webClient = new WebClient ();
					//An large image url
					var url = new Uri (DetailItem.getCoverString ());
					byte[] bytes = null;

					webClient.DownloadProgressChanged += HandleDownloadProgressChanged;

					//Start download data using DownloadDataTaskAsync
					try {
						bytes = await webClient.DownloadDataTaskAsync (url);
					} catch (OperationCanceledException) {
						Console.WriteLine ("Task Canceled!");
						return;
					} catch (Exception e) {
						Console.WriteLine (e.ToString ());
						return;
					}
					string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
					string localFilename = "downloaded.png";
					string localPath = Path.Combine (documentsPath, localFilename);

					//Save the image using writeAsync
					FileStream fs = new FileStream (localPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
					await fs.WriteAsync (bytes, 0, bytes.Length);
					fs.Close ();

					Console.WriteLine ("localPath:" + localPath);

					//Resizing image is time costing, using async to avoid blocking the UI thread
					UIImage image = null;
					CGSize imageViewSize = coverImage.Frame.Size;

					await Task.Run (() => {
						image = UIImage.FromFile (localPath).Scale (imageViewSize);
					});
					Console.WriteLine ("Loaded!");

					BTProgressHUD.Dismiss ();
					coverImage.Image = image;
				}
				catch(Exception ex)
				{
					Console.Out.WriteLine ("Error in loading image from online: " + ex.ToString ());
					waitForToast ("Error in loading image from online.", 2000);
				}

			} else
			{
				Console.Out.WriteLine ("usesuserphoto == true; displaying image from file");

				coverImage.TranslatesAutoresizingMaskIntoConstraints = false;

				string path = DetailItem.userimagepath;
				Console.Out.WriteLine ("pngfilename = " + path);

				try
				{
					await ImageService.LoadFile(path)
						.Success(() =>
							{
								Console.Out.WriteLine("Image loaded success");
							})
						.Error(exception =>
							{
								Console.Out.WriteLine("Image loaded failure");
							})
						.IntoAsync(coverImage);

					Console.Out.WriteLine ("setting image from file completed");
				}
				catch(Exception ex)
				{
					Console.Out.WriteLine ("image loaded exception \n" + ex.ToString ());
				}

				/*try
				{
					coverImage.Image = UIImage.FromFile (path);

					coverImage.Transform = CGAffineTransform.MakeRotation ((float)Math.PI/2);
				}
				catch(Exception ex)
				{
					Console.Out.WriteLine ("Error trying to open custom photo. " + ex);
				}*/
			}
		}

		async void waitForToast(string content, int millis)
		{
			BTProgressHUD.ShowToast (content, true);
			await Task.Delay (millis);
			BTProgressHUD.Dismiss();
		}

		void HandleDownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
			//this.downloadProgress.Progress = e.ProgressPercentage / 100.0f;
			Console.Out.WriteLine ("download progress changed: " + e.ProgressPercentage.ToString ());
		}

		void cancelDownload(object sender, System.EventArgs ea)
		{
			Console.WriteLine ("Cancel clicked!");
			if(webClient!=null)
				webClient.CancelAsync ();

			webClient.DownloadProgressChanged -= HandleDownloadProgressChanged;
		}

		public void getPrice()
		{
			if(string.IsNullOrEmpty(DetailItem.isbn))
			{
				Console.Out.WriteLine ("test book. no pricing data.");
				priceButton.SetTitle ("Sorry, pricing data is only available via ISBN", UIControlState.Disabled);
			}
			else
			{
				// attempt to get pricing data via ISBNdb
				// make the url
				// http://isbndb.com/api/v2/json/[your-api-key]/prices/9780849303159 
				string url = "http://isbndb.com/api/v2/json/2DEB0A3O/prices/" + DetailItem.isbn;
				Console.Out.WriteLine ("url used: " + url);

				// restSharp attempt here
				var client = new RestClient (url);

				var request = new RestRequest (Method.GET);
				client.ExecuteAsync (request, response => 
				{
					Console.WriteLine ("RestSharp response:" + response.Content);
					parseJSON(response.Content);
				});
			}
		}

		public void parseJSON(string content)
		{
			Console.Out.WriteLine ("Parse pricing JSON entered.");
			//priceButton.SetTitle (priceString, UIControlState.Disabled);

			JObject wholeThing = JObject.Parse (content);
			JToken data = wholeThing ["data"];

			// because they have a stupid array format
			JToken bookInfo = data [0];
			//Console.Out.WriteLine ("Book info: " + bookInfo.ToString ());

			foreach(var obj in data)
			{
				//Console.Out.WriteLine ("obj to string: " + obj.ToString ());
				if(obj["store_id"].ToString() == "amazon")
				{
					Console.Out.WriteLine ("Amazon price: $" + obj ["price"].ToString ());
					//priceButton.SetTitle ("Amazon price: $" + obj ["price"].ToString (), UIControlState.Normal);
					//priceString = "Amazon price: $" + obj ["price"];
				}
				else
				{
					//priceButton.SetTitle ("No Amazon price found!", UIControlState.Disabled);
					Console.Out.WriteLine ("No Amazon price found!");
					//priceString = "No Amazon price found!";
				}
			}
		}
	}
}
