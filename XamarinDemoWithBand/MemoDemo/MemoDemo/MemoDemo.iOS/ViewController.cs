using System;
using System.Collections.ObjectModel;
using Foundation;
using UIKit;

namespace MemeDemo4.iOS
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

            ObservableCollection<string> memes = await XamarinMemeGenerator.WantSomeMemesNowClass.ShowMeThoseMemes();
            MemePicker.Model = new MemesPickerViewModel(memes);
        }

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

        async partial void MemeButton_TouchUpInside(UIButton sender)
        {

            var rowSel = MemePicker.SelectedRowInComponent(new nint(0));
            var memeString = (MemePicker.Model as MemesPickerViewModel).GetTitle(rowSel);
            byte[] imageByteArr = await XamarinMemeGenerator.WantSomeMemesNowClass.GenerateMyMeme(memeString, MemeText1.Text, MemeText2.Text);

            var img = new UIImage(NSData.FromArray(imageByteArr));
            MemeImage.Image = img;
        }
	}


    public class MemesPickerViewModel : UIPickerViewModel
    {

        private string[] _memesArr;

        public MemesPickerViewModel(ObservableCollection<string> memes)
        {
            _memesArr = new string[memes.Count];
            memes.CopyTo(_memesArr, 0);
        }

        public string GetTitle(nint row)
        {
            return _memesArr[row];
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return _memesArr[row];
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _memesArr.Length;
        }
    }
}

