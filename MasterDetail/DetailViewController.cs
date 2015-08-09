using System;
using System.Net;
using System.IO;
using System.Text;
using UIKit;
using System.Threading.Tasks;
using System.Threading;
using CoreGraphics;
using CoreImage;

namespace MasterDetail
{
	public partial class DetailViewController : UIViewController
	{
		public Book DetailItem { get; set; }
		public WebClient webClient;

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		public void SetDetailItem (Book newDetailItem)
		{
			if (DetailItem != newDetailItem) {
				DetailItem = newDetailItem;
				
				// Update the view
				ConfigureView ();
			}
		}

		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (IsViewLoaded && DetailItem != null)
			{
				Console.Out.WriteLine ("author detail: " + DetailItem.getAuthor ());

				downloadAsync ();
				titleLabel.Text = DetailItem.getTitle ();
				authorLabel.Text = DetailItem.getAuthor ();
				summaryButton.Hidden = false;

				summaryButton.TouchUpInside += (object sender, EventArgs e) => 
				{
					var alert = UIAlertController.Create("Book Summary", DetailItem.getSummary(), UIAlertControllerStyle.Alert);

					// add buttons
					alert.AddAction(UIAlertAction.Create("Looks Interesting", UIAlertActionStyle.Default, null));

					// actually show the thing
					PresentViewController(alert, true, null);
				};
			}
		}

		async void downloadAsync(/*object sender, System.EventArgs ea*/)
		{
			webClient = new WebClient ();
			//An large image url
			var url = new Uri (DetailItem.getCoverString());
			byte[] bytes = null;

			webClient.DownloadProgressChanged += HandleDownloadProgressChanged;

			//Start download data using DownloadDataTaskAsync
			try{
				bytes = await webClient.DownloadDataTaskAsync(url);
			}
			catch(OperationCanceledException){
				Console.WriteLine ("Task Canceled!");
				return;
			}
			catch(Exception e) {
				Console.WriteLine (e.ToString());
				return;
			}
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string localFilename = "downloaded.png";
			string localPath = Path.Combine (documentsPath, localFilename);

			//Save the image using writeAsync
			FileStream fs = new FileStream (localPath, FileMode.OpenOrCreate);
			await fs.WriteAsync (bytes, 0, bytes.Length);

			Console.WriteLine("localPath:"+localPath);

			//Resizing image is time costing, using async to avoid blocking the UI thread
			UIImage image = null;
			CGSize imageViewSize = coverImage.Frame.Size;

			await Task.Run( () => { image = UIImage.FromFile(localPath).Scale(imageViewSize); } );
			Console.WriteLine ("Loaded!");

			coverImage.Image = image;

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
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			summaryButton.Hidden = true;
			// Perform any additional setup after loading the view, typically from a nib.
			ConfigureView ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


