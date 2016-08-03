using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Naylah.SampleApp;
using Naylah.SampleApp.Droid;
using Xamarin.Forms;


namespace Naylah.SampleApp.Droid
{
    [Activity(
        Label = "Naylah Sample App", 
        Icon = "@drawable/ic_launcher", 
        MainLauncher = false, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)
        ]
    public class MainActivity : FormsAppCompatActivity
    {

        public App CurrentApp { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {

            FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            CurrentApp = new App();

            LoadApplication(CurrentApp);

        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (!CurrentApp.Initialized)
            {
                await CurrentApp.InitializeApp();
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async override void OnBackPressed()
        {
            bool? result = await App.CallHardwareBackPressed();
            if (result == true)
            {
                base.OnBackPressed();
            }
            else if (result == null)
            {
                Finish();
            }
        }
    }
}

