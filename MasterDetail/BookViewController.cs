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

			// Update the user interface for the detail item
			if (IsViewLoaded && DetailItem != null)
			{
				this.Title = DetailItem.getTitle ();
				titleLabel.Text = DetailItem.getTitle ();
				authorLabel.Text = DetailItem.getAuthor ();
				dateAddedLabel.Text = "Added " + DetailItem.getDateAdded ().ToString ();
				publisherLabel.Text = DetailItem.getPublisher ();

				// attempt to download the cover photo
				downloadAsync ();

				Console.Out.WriteLine ("View updates done.");
			}

			// put some filler text in if the publisher isn't filled out
			if(DetailItem.getPublisher() == string.Empty)
			{
				publisherLabel.Text = "No publisher information available.";
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

		// framework for downloading cover image. eventually store it and don't do it every time.
		async void downloadAsync()
		{
			if (DetailItem.getCoverString () != null) {
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
			} else
			{
				Console.Out.WriteLine ("Can't get cover. Cover string == null");
				Console.Out.WriteLine ("Going to attempt to display image from file.");

				//coverImage.TranslatesAutoresizingMaskIntoConstraints = false;
				//coverImage.Image = UIImage.FromBundle ("/var/mobile/Containers/Data/Application/828D8F48-D213-47A5-BE5C-3E05B32F1E80/Documents/Photo.jpg");

				string path = "/var/mobile/Containers/Data/Application/828D8F48-D213-47A5-BE5C-3E05B32F1E80/Documents/Photo.png";

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
			}
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
	}
}
