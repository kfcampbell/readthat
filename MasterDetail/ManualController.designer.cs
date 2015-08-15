// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MasterDetail
{
	[Register ("ManualController")]
	partial class ManualController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton addButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField authorField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField isbnField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField pubField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField summaryField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField titleField { get; set; }

		[Action ("addButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void addButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (addButton != null) {
				addButton.Dispose ();
				addButton = null;
			}
			if (authorField != null) {
				authorField.Dispose ();
				authorField = null;
			}
			if (isbnField != null) {
				isbnField.Dispose ();
				isbnField = null;
			}
			if (pubField != null) {
				pubField.Dispose ();
				pubField = null;
			}
			if (summaryField != null) {
				summaryField.Dispose ();
				summaryField = null;
			}
			if (titleField != null) {
				titleField.Dispose ();
				titleField = null;
			}
		}
	}
}
