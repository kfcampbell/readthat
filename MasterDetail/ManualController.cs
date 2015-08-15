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
		Book newBook;
		private string _pathToDatabase;
		private string _databaseName = "book_database.db";

		public ManualController (IntPtr handle) : base (handle)
		{
			// handle the creation and addition of a new book
			// also needs database operation here
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
			var thisdate = DateTime.Now.ToLocalTime ();
			newBook.dateadded = thisdate;

			if(isbnText != string.Empty)
			{
				newBook.isbn = isbnText;
			}

			insertToDatabase(newBook);
		}

		private void insertToDatabase(Book theBook)
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);

			using (var db = new SQLite.SQLiteConnection(_pathToDatabase ))
			{
				db.Insert(theBook);
			}
			Console.Out.WriteLine ("Book " + newBook.getTitle () + " inserted into database.");

			// ui alert to let the user know
			var alert = UIAlertController.Create("Book Added!", "Restart the app to see the book in your database.", UIAlertControllerStyle.Alert);

			// add buttons
			alert.AddAction(UIAlertAction.Create("Okay", UIAlertActionStyle.Default, null));

			// actually show the thing
			PresentViewController(alert, true, null);
		}



		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			Console.Out.WriteLine ("Manual controller viewdidload entered.");

			// dismiss the keyboard if anywhere in the view is tapped (except another textview)
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

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
