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
	[Register ("BookViewController")]
	partial class BookViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel authorLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView coverImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel dateAddedLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton editButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton priceButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel publisherLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton summaryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel titleLabel { get; set; }

		[Action ("editButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void editButton_TouchUpInside (UIButton sender);

		[Action ("priceButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void priceButton_TouchUpInside (UIButton sender);

		[Action ("summaryButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void summaryButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (authorLabel != null) {
				authorLabel.Dispose ();
				authorLabel = null;
			}
			if (coverImage != null) {
				coverImage.Dispose ();
				coverImage = null;
			}
			if (dateAddedLabel != null) {
				dateAddedLabel.Dispose ();
				dateAddedLabel = null;
			}
			if (editButton != null) {
				editButton.Dispose ();
				editButton = null;
			}
			if (priceButton != null) {
				priceButton.Dispose ();
				priceButton = null;
			}
			if (publisherLabel != null) {
				publisherLabel.Dispose ();
				publisherLabel = null;
			}
			if (summaryButton != null) {
				summaryButton.Dispose ();
				summaryButton = null;
			}
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}
		}
	}
}
