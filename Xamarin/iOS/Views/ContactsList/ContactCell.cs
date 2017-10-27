using System;
using Foundation;
using ObsidianMobile.iOS.Extensions;
using UIKit;

namespace ObsidianMobile.iOS.Views
{
    public partial class ContactCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ContactCell");
        public static readonly UINib Nib;

        public string Name 
        {
            get
            {
                return ProfileName.Text;
            }
            set
            {
                ProfileName.Text = value;
            }
        }

        public string ProfileStatus
        {
            get
            {
                return Status.Text;
            }
            set
            {
                Status.Text = value;
            }
        }

        public UIImage Image
        {
            get
            {
                return ProfileImage.Image;
            }
            set
            {
                ProfileImage.Image = value;
            }
        }

        static ContactCell()
        {
            Nib = UINib.FromName("ContactCell", NSBundle.MainBundle);
        }

        protected ContactCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Status.TextColor = UIColor.FromRGB(57,116,111); //TODO only for online
            ProfileImage.MakeRoundImageView();
        }
    }
}
