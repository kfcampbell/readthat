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

// amazon link: http://smile.amazon.com/s/ref=nb_ss_gw?url=search-alias%3Daps&field-keywords=9780152049409+&x=0&y=0 sub isbn number

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

				this.Title = DetailItem.getTitle ();
				
				// Update the view
				ConfigureView ();
			}
		}

		/*public void ViewWillDisappear()
		{
			base.ViewDidDisappear ();
			try
			{
				BTProgressHUD.Dismiss();
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("Error attempting to hide BTProgressHud");
			}
		}*/

		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (IsViewLoaded && DetailItem != null)
			{
				this.Title = DetailItem.getTitle ();
				Console.Out.WriteLine ("author detail: " + DetailItem.getAuthor ());

				downloadAsync ();
				titleLabel.Text = DetailItem.getTitle ();
				authorLabel.Text = DetailItem.getAuthor ();
				dateAddedLabel.Text = "Added " + DetailItem.getDateAdded ().ToString ();
				summaryButton.Hidden = false;

				/*summaryButton.TouchUpInside += (object sender, EventArgs e) => 
				{
					// hide Cover Downloading pop-up.
					BTProgressHUD.Dismiss();

					if(DetailItem.getSummary() != String.Empty)
					{
						var alert = UIAlertController.Create("Book Summary", DetailItem.getSummary(), UIAlertControllerStyle.Alert);

						// add buttons
						alert.AddAction(UIAlertAction.Create("Looks Interesting", UIAlertActionStyle.Default, null));

						// actually show the thing
						PresentViewController(alert, true, null);
					}
					else
					{
						var alert = UIAlertController.Create("Book Summary", "The summary looks empty. Sorry about that.", UIAlertControllerStyle.Alert);

						// add buttons
						alert.AddAction(UIAlertAction.Create("Aww man.", UIAlertActionStyle.Default, null));

						// actually show the thing
						PresentViewController(alert, true, null);
					}
				};*/
			}
		}

		async void downloadAsync()
		{
			if (DetailItem.getCoverString () != null) {
				BTProgressHUD.Show ("Retrieving Cover...");

				// attempt to hide image downloading thing
				summaryButton.TouchUpInside += (object sender, EventArgs e) => 
				{
					// hide Cover Downloading pop-up.
					BTProgressHUD.Dismiss();

					if(DetailItem.getSummary() != String.Empty)
					{
						var alert = UIAlertController.Create("Book Summary", DetailItem.getSummary(), UIAlertControllerStyle.Alert);

						// add buttons
						alert.AddAction(UIAlertAction.Create("Looks Interesting", UIAlertActionStyle.Default, null));

						// actually show the thing
						PresentViewController(alert, true, null);
					}
					else
					{
						var alert = UIAlertController.Create("Book Summary", "The summary looks empty. Sorry about that.", UIAlertControllerStyle.Alert);

						// add buttons
						alert.AddAction(UIAlertAction.Create("Aww man.", UIAlertActionStyle.Default, null));

						// actually show the thing
						PresentViewController(alert, true, null);
					}
				};

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
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//summaryButton.Hidden = true;
			// Perform any additional setup after loading the view, typically from a nib.
			ConfigureView ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public void OnResignActivation(UIApplication application)
		{
			Console.WriteLine("OnResignActivation called, App moving to inactive state.");
		}
	}
}


