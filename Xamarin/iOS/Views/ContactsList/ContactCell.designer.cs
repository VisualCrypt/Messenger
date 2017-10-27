// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ObsidianMobile.iOS.Views
{
    [Register ("ContactCell")]
    partial class ContactCell
    {
        [Outlet]
        UIKit.UIImageView ProfileImage { get; set; }


        [Outlet]
        UIKit.UILabel ProfileName { get; set; }


        [Outlet]
        UIKit.UILabel Status { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ProfileImage != null) {
                ProfileImage.Dispose ();
                ProfileImage = null;
            }

            if (ProfileName != null) {
                ProfileName.Dispose ();
                ProfileName = null;
            }

            if (Status != null) {
                Status.Dispose ();
                Status = null;
            }
        }
    }
}