﻿using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using ZXing;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using BigTed;


namespace MasterDetail
{
	public partial class MasterViewController : UITableViewController
	{
		DataSource dataSource;
		private string _pathToDatabase;
		private string _databaseName = "book_database.db";
		public IList<Book> bookList;
		//UIImagePickerController imagePicker;

		public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Master", "Book List");
		}

		public override void ViewWillAppear (bool animated)
		{
			Console.Out.WriteLine ("view will appear entered.");
			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);

			// attempt to hide Cover Downloading pop-up.
			try
			{
				BTProgressHUD.Dismiss ();
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("Error hiding BTProgressHUD \n" + ex.ToString ());
			}

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = BookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			//NavigationItem.LeftBarButtonItem = EditButtonItem;


			// experiment with fuck the edit button item. going for sorting here
			var sortButton = new UIBarButtonItem (UIBarButtonSystemItem.Bookmarks, sortDataBase);


			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[]{addButton, sortButton};

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title = "Book List";
			//this.NavigationController.NavigationBar.BackgroundColor = UIColor.Blue;
			//NavigationController.NavigationBar.Appearance.SetBackgroundImage (UIImage.FromBundle ("unavailable2.png"), UIBarPosition.TopAttached, UIBarMetrics.Default);

			NavigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = BookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		private void ActionRefresh()
		{
			TableView.ReloadData ();
		}

		private void sortDataBase(object sender, EventArgs e)
		{
			Console.Out.WriteLine ("sort database entered.");

			// Create a new Alert Controller
			UIAlertController actionSheetAlert = UIAlertController
				.Create("Sort Books", "Select an option from below:", UIAlertControllerStyle.ActionSheet);

			// Add Actions
			actionSheetAlert.AddAction(UIAlertAction
				.Create("Recently Added First (default)",UIAlertActionStyle.Default, (action) => sortByNewest()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Recently Added Last",UIAlertActionStyle.Default, (action) => sortByOldest()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Alphabetical",UIAlertActionStyle.Default, (action) => sortAToZ()));

			actionSheetAlert.AddAction (UIAlertAction
				.Create ("Reverse Alphabetical", UIAlertActionStyle.Default, (action) => sortZToA()));

			actionSheetAlert.AddAction (UIAlertAction
				.Create ("Cancel", UIAlertActionStyle.Cancel, (action) => Console.Out.WriteLine("Cancel pressed.")));

			// Display the alert
			this.PresentViewController(actionSheetAlert,true,null);
		}

		private void sortByOldest()
		{
			Console.Out.WriteLine ("Sort by oldest entered");

			base.ViewDidLoad ();
			this.Title = "Book List";
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.Blue;

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = recentLastBookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);
		}

		private void sortByNewest()
		{
			Console.Out.WriteLine ("Sort by newest entered");
			base.ViewDidLoad ();
			this.Title = "Book List";
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.Blue;

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = BookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);

		}

		private void sortAToZ()
		{
			Console.Out.WriteLine ("Sort A to Z entered");

			base.ViewDidLoad ();
			this.Title = "Book List";
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.Blue;

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = aToZBookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);
		}

		private void sortZToA()
		{
			Console.Out.WriteLine ("Sort Z to A entered");

			base.ViewDidLoad ();
			this.Title = "Book List";
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.Blue;

			// begin creating and loading the database attempt
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_pathToDatabase = Path.Combine(documents, _databaseName);
			createDatabase ();

			var db = new SQLite.SQLiteConnection (_pathToDatabase);
			QueryValuations (db);
			bookList = zToABookList (db);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			TableView.Source = dataSource = new DataSource (this, bookList, _pathToDatabase);
		}

		private void createDatabase()
		{
			// Create the database and a table to hold Person information.
			using (var conn= new SQLite.SQLiteConnection(_pathToDatabase))
			{
				conn.CreateTable<Book>();
			}
			Console.Out.WriteLine ("Database was created");
		}

		private void insertToDatabase(Book newBook)
		{
			using (var db = new SQLite.SQLiteConnection(_pathToDatabase ))
			{
				db.Insert(newBook);
			}
			Console.Out.WriteLine ("Book " + newBook.getTitle () + " inserted into database.");
		}

		public static IEnumerable<Book> QueryValuations (SQLiteConnection db)
		{
			Book[] bookArray = db.Query<Book> ("select * from Book").ToArray ();
			for(int i = 0; i < bookArray.Length; i++)
			{
				//Console.Out.WriteLine ("Book array " + i + ": " + bookArray [i].getTitle ());
			}
			return db.Query<Book> ("select * from Book");
		}

		public static IList<Book> BookList (SQLiteConnection db)
		{
			
			Book[] bookArray = db.Query<Book> ("select * from Book order by dateAdded desc").ToArray ();
			IList<Book> newBookList = bookArray;
			return newBookList;
		}

		public static IList<Book> aToZBookList (SQLiteConnection db)
		{

			Book[] bookArray = db.Query<Book> ("select * from Book order by title asc").ToArray ();
			IList<Book> newBookList = bookArray;
			return newBookList;
		}

		public static IList<Book> zToABookList (SQLiteConnection db)
		{

			Book[] bookArray = db.Query<Book> ("select * from Book order by title desc").ToArray ();
			IList<Book> newBookList = bookArray;
			return newBookList;
		}

		public static IList<Book> recentLastBookList (SQLiteConnection db)
		{

			Book[] bookArray = db.Query<Book> ("select * from Book order by dateAdded asc").ToArray ();
			IList<Book> newBookList = bookArray;
			return newBookList;
		}

		public void AddNewItem (object sender, EventArgs args)
		{
			// Create a new Alert Controller
			UIAlertController actionSheetAlert = UIAlertController
				.Create("Add A New Item", "Select an option from below:", UIAlertControllerStyle.ActionSheet);

			// Add Actions
			actionSheetAlert.AddAction(UIAlertAction
				.Create("Scan an ISBN",UIAlertActionStyle.Default, (action) => scanIsbn()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Enter information manually",UIAlertActionStyle.Default, (action) => enterManually()));

			actionSheetAlert.AddAction(UIAlertAction
				.Create("Cancel",UIAlertActionStyle.Cancel, (action) => Console.WriteLine ("Cancel button pressed.")));

			// Display the alert
			this.PresentViewController(actionSheetAlert,true,null);

		}

		public void enterManually()
		{
			Console.Out.WriteLine ("Enter manually pressed.");
			PerformSegue ("manualSegue", this);
		}

		async void scanIsbn()
		{
			// start the barcode scanner
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			var result = await scanner.Scan();

			try
			{
				// create an object to hold the book.
				Book newBook = new Book (result.Text);
				Console.Out.WriteLine ("newbook title: " + newBook.getTitle ());

				// make sure it's actually a book before adding it.
				try
				{
					if (newBook.getAuthor().Length > 3) 
					{
						insertToDatabase (newBook);
						Console.Out.WriteLine("newbook added");
						dataSource.Objects.Insert (0, newBook);

						using (var indexPath = NSIndexPath.FromRowSection (0, 0))
							TableView.InsertRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);

					} else
						Console.Out.WriteLine ("newbook seems to be null");
				}
				catch(Exception ex)
				{
					Console.Out.WriteLine ("Adding newbook error: " + ex.ToString ());
				}
			}
			catch(Exception ex)
			{
				Console.Out.WriteLine ("scanning was probably canceled.\n" + ex.ToString ());
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail") 
			{
				var indexPath = TableView.IndexPathForSelectedRow;
				var item = dataSource.Objects [indexPath.Row];

				// load the book view controller with the selected book.
				((BookViewController)segue.DestinationViewController).SetDetailItem (item);
				Console.Out.WriteLine ("Book view controller book: " + item.getTitle ());
			}
			else if(segue.Identifier == "manualSegue")
			{
				//((ManualController)segue.DestinationViewController).setTestItem (5);
				Console.Out.WriteLine ("manual segue entered");
			}
		}

		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("Cell");
			readonly List<Book> objects;
			readonly MasterViewController controller;
			string _pathToDatabase;

			public DataSource (MasterViewController controller, IList<Book> newObjects, string path)
			{
				this.controller = controller;
				this.objects = new List<Book>(newObjects);
				this._pathToDatabase = path;
			}

			public IList<Book> Objects {
				get { return objects; }
			}

			// Customize the number of sections in the table view.
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return objects.Count;
			}

			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (CellIdentifier, indexPath);

				cell.TextLabel.Text = objects [indexPath.Row].getTitle();

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}

			public void deleteFromDatabase(Book oldBook)
			{
				Console.Out.WriteLine("Book " + oldBook.getTitle() + " deleted from database.");

				using (var db = new SQLite.SQLiteConnection(_pathToDatabase))
				{
					Console.Out.WriteLine ("object deleted: " + oldBook.getDateAdded ());
					db.Delete (oldBook);
					//return Delete
				}
			}

			private void createDatabase()
			{
				// Create the database and a table to hold Person information.
				using (var conn= new SQLite.SQLiteConnection(_pathToDatabase))
				{
					conn.CreateTable<Book>();
				}
				Console.Out.WriteLine ("Inner class: Database was created");
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) 
				{
					//Console.Out.WriteLine ("object to delete: " + objects [indexPath.Row].getDateAdded ());
					try
					{
						Console.Out.WriteLine("index: " + (indexPath.Row));
						deleteFromDatabase (objects[indexPath.Row]);
					}
					catch(Exception ex)
					{
						Console.Out.WriteLine ("error deleting book: \n" + ex.ToString ());
					}

					// Delete the row from the data source.
					objects.RemoveAt (indexPath.Row);
					controller.TableView.DeleteRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
				} 
				else if (editingStyle == UITableViewCellEditingStyle.Insert) 
				{
					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
				}
			}
		}
	}
}

