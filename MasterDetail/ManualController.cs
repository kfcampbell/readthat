using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MasterDetail
{
	partial class ManualController : UITableViewController
	{
		public ManualController (IntPtr handle) : base (handle)
		{
			// handle the creation and addition of a new book

		}

		partial void addButton_TouchUpInside (UIButton sender)
		{
			Console.Out.WriteLine("Manual add button pressed.");
		}
	}

}
