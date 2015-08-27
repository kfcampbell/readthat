using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using SQLite;
using System.IO;

namespace MasterDetail
{
	partial class ManualController : UITableViewController
	{
		public Book newBook { get; set; }
		private string _pathToDatabase;
		private const string _databaseName = "book_database.db";
		private DateTime timeAdded;
		private string photoString;
		string pngFilename;

		public ManualController (IntPtr handle) : base (handle)
		{
		}

		public void SetBookItem (Book newDetailItem)
		{

			if (newBook != newDetailItem) {
				newBook = newDetailItem;

				// attempt to set it manually
				newBook.title = newDetailItem.getTitle ();
				newBook.author = newDetailItem.getAuthor ();
				newBook.publisher = newDetailItem.getPublisher ();
				newBook.summary = newDetailItem.getSummary ();
				newBook.isbn = newDetailItem.getIsbn ();
				newBook.dateadded = newDetailItem.getDateAdded ();
				timeAdded = newDetailItem.getDateAdded ();
				Console.Out.WriteLine ("Time added: " + timeAdded.ToString ());

				Console.Out.WriteLine ("Set Book Item entered: " + newBook.getTitle ());
			}
		}

		private void ConfigureView()
		{
			Console.Out.WriteLine ("Configure view manual controller entered");
			try
			{
				Console.Out.WriteLine("newbook title: " + newBook.getTitle());
				titleField.Text = newBook.getTitle();
				authorField.Text = newBook.getAuthor ();
				pubField.Text = newBook.getPublisher ();
				summaryField.Text = newBook.getSummary ();
				isbnField.Text = newBook.getIsbn ();
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("Error populating fields: " + ex.ToString ());
			}
		}

		partial void takePhotoButton_TouchUpInside (UIButton sender)
		{
			Console.Out.WriteLine("Take Photo button pressed.");

			pngFilename = "not working";

			DateTime photoTime = DateTime.Now.ToLocalTime ();

			// testing
			Console.Out.WriteLine(photoTime.ToLongDateString() + " == long date string");
			Console.Out.WriteLine(photoTime.ToLongTimeString() + " == long time string");
			Console.Out.WriteLine(photoTime.ToShortDateString() + " == short date string");
			photoString = "photo" + photoTime.Month + photoTime.Day + photoTime.Year + photoTime.Hour 
				+ photoTime.Minute + photoTime.Second + ".png";
			Console.Out.WriteLine("photostring: " + photoString);

			Camera.TakePicture (this, (obj) =>
				{
					var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
					var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					pngFilename = System.IO.Path.Combine (documentsDirectory, photoString); 
					NSData imgData = photo.AsPNG();
					NSError err = null;
					if (imgData.Save(pngFilename, false, out err)) 
					{
						Console.WriteLine("saved as " + pngFilename);
					} 
					else 
					{
						Console.WriteLine("NOT saved as " + pngFilename + " because" + err.LocalizedDescription);
					}

					// alert to let the user know the photo was added
					var alert = UIAlertController.Create("Photo Added!", "", UIAlertControllerStyle.Alert);

					// add buttons
					alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Default, null));

					// actually show the thing
					PresentViewController(alert, true, null);
				});
		}

		partial void addButton_TouchUpInside (UIButton sender)
		{
			Console.Out.WriteLine("Manual add button pressed.");
			string titleText = titleField.Text;
			string authorText = authorField.Text;
			string pubText = pubField.Text;
			string summaryText = summaryField.Text;
			string isbnText = isbnField.Text;

			Console.Out.WriteLine("book info: \n" + titleText + "\n" + authorText + "\n" + pubText + "\n" + summaryText + "\n" + isbnText);

			// then make it into a book object and load it into the database
			if(string.IsNullOrEmpty(isbnText) || isbnText.Length < 9)
			{
				newBook = new Book(); // used to be blank
				Console.Out.WriteLine("newbook blank isbn");
				newBook.usesuserphoto = true;
			}
			else
			{
				newBook = new Book(isbnText);
				Console.Out.WriteLine("newbook given isbn");
				newBook.usesuserphoto = false;
			}

			if(!string.IsNullOrEmpty(photoString))
			{
				Console.Out.WriteLine("photo string not null or empty! adding");
				newBook.userimagepath = photoString;
			}

			if(!string.IsNullOrEmpty(pngFilename))
			{
				Console.Out.WriteLine("pngfile name not null or empty: " + pngFilename);
				newBook.userimagepath = pngFilename;
			}

			// perform user initializations
			newBook.title = titleText;
			newBook.author = authorText;
			newBook.publisher = pubText;
			newBook.summary = summaryText;
			newBook.coverstring = null;

			// this is how to tell if this class was entered from BookViewController or the UI Alert Controller
			if(timeAdded == new DateTime())
			{
				var thisdate = DateTime.Now.ToLocalTime ();
				newBook.dateadded = thisdate;
			}
			else
			{
				newBook.dateadded = timeAdded;
			}

			// make sure that none of the important fields are empty before adding
			if((newBook.title == string.Empty) || (newBook.author == string.Empty))
			{
				Console.Out.WriteLine("Hold on a minute! You didn't add a title or an author!");
				string titleMessage = "Wait Just A Hot Second!";
				string contentMessage = "You left the title or the author empty!\nIt's okay to write \'I don't know\' and edit it later!";

				// now make an alert to notify the user
				var alert = UIAlertController.Create(titleMessage, contentMessage, UIAlertControllerStyle.Alert);

				// add buttons
				alert.AddAction(UIAlertAction.Create("Hmm...Okay", UIAlertActionStyle.Default, null));

				// actually show the thing
				PresentViewController(alert, true, null);

				// return so that the book's not added
				return;
			}

			insertToDatabase(newBook);
		}

		private void insertToDatabase(Book theBook)
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);

			using (var db = new SQLite.SQLiteConnection(_pathToDatabase ))
			{
				//db.Insert(theBook);
				//db.Update (theBook);
				db.InsertOrReplace (theBook);
			}
			Console.Out.WriteLine ("Book " + newBook.getTitle () + " inserted into database.");

			// ui alert to let the user know
			var alert = UIAlertController.Create("Book Added!", "Hit the back button to see the book in your Book List.", UIAlertControllerStyle.Alert);

			// add buttons
			alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Default, null));

			// actually show the thing
			PresentViewController(alert, true, null);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			Console.Out.WriteLine ("Manual controller viewdidload entered.");

			try
			{
				ConfigureView();
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("Configure View error (manual): " + ex.ToString ());
			}

			// dismiss the keyboard if anywhere in the view is tapped (except another textview)
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			// experiment with setting return key types
			titleField.ReturnKeyType = UIReturnKeyType.Next;
			authorField.ReturnKeyType = UIReturnKeyType.Next;
			pubField.ReturnKeyType = UIReturnKeyType.Next;
			summaryField.ReturnKeyType = UIReturnKeyType.Next;
			isbnField.ReturnKeyType = UIReturnKeyType.Done;

			// advance the keyboard to the next textview
			this.titleField.ShouldReturn += (titleField) => 
			{
				this.authorField.BecomeFirstResponder ();
				return true;
			};

			this.authorField.ShouldReturn += (authorField) => 
			{
				this.pubField.BecomeFirstResponder ();
				return true;
			};

			this.pubField.ShouldReturn += (pubField) => 
			{
				this.summaryField.BecomeFirstResponder();
				return true;
			};

			this.summaryField.ShouldReturn += (summaryField) => 
			{
				this.isbnField.BecomeFirstResponder ();
				return true;
			};
				
			// dismiss the keyboard once the last field is filled out
			this.isbnField.ShouldReturn += (isbnField) => 
			{
				this.isbnField.ResignFirstResponder();
				return true;
			};
		}
	}

}
