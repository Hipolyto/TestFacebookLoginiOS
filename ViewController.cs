using System;
using Facebook.LoginKit;
using Facebook.CoreKit;

using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using System.Drawing;

namespace TestFbLogin.iOS
{
    public partial class ViewController : UIViewController
    {
        // To see the full list of permissions, visit the following link:
        // https://developers.facebook.com/docs/facebook-login/permissions/v2.3

        // This permission is set by default, even if you don't add it, but FB recommends to add it anyway
        List<string> readPermissions = new List<string> { "public_profile" };

        LoginButton loginView;
        ProfilePictureView pictureView;
        UILabel nameLabel;
        UIButton loginButton;

        public bool IsFacebookUserLogin
        {
            get
            {
                return AccessToken.CurrentAccessToken != null;
            }
        }

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            // If was send true to Profile.EnableUpdatesOnAccessTokenChange method
            // this notification will be called after the user is logged in and
            // after the AccessToken is gotten
            Profile.Notifications.ObserveDidChange((sender, e) => 
            {

                if (e.NewProfile == null)
                {
                    UpdateProfile(null);
                    return;
                }
                else
                {
                    UpdateProfile(e.NewProfile);
                }
            });

            Profile.EnableUpdatesOnAccessTokenChange(true);
            AccessToken.Notifications.ObserveDidChange((sender, e) =>
            {
                UpdateVars();

                if (e.NewToken == null)
                {
                    //UpdateProfile(null);
                    return;
                }

                 //UpdateProfile(Profile.CurrentProfile);
            });

            // The user image profile is set automatically once is logged in
            pictureView = new ProfilePictureView(new CGRect(50, 50, 220, 220));

            // Set the Read and Publish permissions you want to get
            loginView = new LoginButton(new CGRect(51, 20, 218, 46))
            {
                LoginBehavior = LoginBehavior.Native,
                ReadPermissions = readPermissions.ToArray()
            };
            // Handle actions once the user is logged in
            loginView.Completed += LoginView_Completed;
            // Handle actions once the user is logged out
            loginView.LoggedOut += LoginView_LoggedOut;
            loginView.LoginBehavior = LoginBehavior.Native;



            // Create the label that will hold user's facebook name
            nameLabel = new UILabel(new RectangleF(20, 319, 280, 21))
            {
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };


            loginButton = new UIButton(new RectangleF(51, 350, 218, 100));
            loginButton.BackgroundColor = UIColor.Blue;
            loginButton.SetTitle("Log In Facebook", UIControlState.Normal);
            loginButton.Tag = 1;
            loginButton.TouchUpInside += (sender, e) =>
            {
                var loginManager = new LoginManager();
                loginManager.LoginBehavior = LoginBehavior.Native;
                loginManager.DefaultAudience = DefaultAudience.Everyone;
                if (loginButton.Tag == 1) // login
                {
                    loginManager.LogInWithReadPermissions(readPermissions.ToArray(), this, HandleLoginManagerRequestTokenHandler);
                }
                else
                {
                    loginManager.LogOut();
                    // UpdateVars();
                    // UpdateProfile(null);
                }
            };



            // Add views to main view
            View.AddSubview(loginView);
            View.AddSubview(pictureView);
            View.AddSubview(nameLabel);
            View.AddSubview(loginButton);

            CheckLogin();
        }



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void HandleLoginManagerRequestTokenHandler(LoginManagerLoginResult result, Foundation.NSError error)
        {
            LoginResult(result, error);
        }

        void LoginView_Completed(object sender, LoginButtonCompletedEventArgs e)
        {
            LoginResult(e.Result, e.Error);
        }

        void LoginResult(LoginManagerLoginResult result, Foundation.NSError error)
        {
            //UpdateVars();

            if (error != null)
            {
                // Handle if there was an error
                //UpdateProfile(null);
            }
            else if (result.IsCancelled)
            {
                // Handle if the user cancelled the login request
                // Handle your successful login
                //UpdateProfile(null);
            }
            else
            {
                // Handle your successful login
                //UpdateProfile(Profile.CurrentProfile);
            }
        }

        void LoginView_LoggedOut(object sender, EventArgs e)
        {
            // Handle your logout

            //UpdateVars();
            //UpdateProfile(null);
        }

        void UpdateVars()
        {
            if (AccessToken.CurrentAccessToken != null)
            {
                AppDelegate.FacebookToken = AccessToken.CurrentAccessToken.TokenString;
                AppDelegate.FacebookUserId = AccessToken.CurrentAccessToken.UserID;
            }
            else
            {
                AppDelegate.FacebookToken = null;
                AppDelegate.FacebookUserId = null;
            }
        }

        void CheckLogin()
        {
            if (AccessToken.CurrentAccessToken != null)
            {
                SHowMessage("Login", "Check Login user");
                UpdateProfile(Profile.CurrentProfile);
            }
            else
            {
                SHowMessage("Login", "Check Login no-user");
                UpdateProfile(null);
            }
        }

        void UpdateProfile(Profile profile)
        {
            if (profile != null)
            {
                
                nameLabel.Text = profile.Name;
                loginButton.SetTitle("Log Out Facebook", UIControlState.Normal);
                loginButton.Tag = 2;
            }
            else
            {
                
                nameLabel.Text = "Name";
                loginButton.SetTitle("Log In Facebook", UIControlState.Normal);
                loginButton.Tag = 1;
            }
        }

        void SHowMessage(string title, string message)
        {
            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, 
                (action) => {
                    Console.WriteLine("OK Clicked.");
            }));

            PresentViewController(alertController, true, null);
        }
    }
}
