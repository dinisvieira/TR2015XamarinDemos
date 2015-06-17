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

namespace MemeDemo4.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton MemeButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView MemeImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView MemePicker { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField MemeText1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField MemeText2 { get; set; }

		[Action ("MemeButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void MemeButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (MemeButton != null) {
				MemeButton.Dispose ();
				MemeButton = null;
			}
			if (MemeImage != null) {
				MemeImage.Dispose ();
				MemeImage = null;
			}
			if (MemePicker != null) {
				MemePicker.Dispose ();
				MemePicker = null;
			}
			if (MemeText1 != null) {
				MemeText1.Dispose ();
				MemeText1 = null;
			}
			if (MemeText2 != null) {
				MemeText2.Dispose ();
				MemeText2 = null;
			}
		}
	}
}
