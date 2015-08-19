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
			newBook = new Book();
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

			if(isbnText != string.Empty)
			{
				newBook.isbn = isbnText;
				newBook.coverstring = "http://covers.openlibrary.org/b/isbn/" + isbnText + "-L.jpg";
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
