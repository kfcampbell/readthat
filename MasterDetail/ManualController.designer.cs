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

		[Action ("addButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void addButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (addButton != null) {
				addButton.Dispose ();
				addButton = null;
			}
		}
	}
}
