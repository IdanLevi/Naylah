using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;

namespace Naylah.SampleApp.Droid
{
    [Activity(
      Label = "NaylahSample",
      Theme = "@style/MyTheme.Splash",
      MainLauncher = true, /////PUT THIS TO FALSE ON MAIN ACTIVITY
      NoHistory = true,
      ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");

        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(t =>
            {
                Log.Debug(TAG, "Work is finished - start Activity1.");
                var MainIntent = new Intent(Application.Context, typeof(MainActivity));

                try
                {
                    Log.Debug("CCCCCCCCCCCCCCCCCCCCCCC", Intent.GetStringExtra("notificationId"));

                    string notificationId = Intent.Extras.GetString("notificationId");

                    MainIntent.PutExtra("notificationId", Intent.GetStringExtra("notificationId"));
                    Intent.RemoveExtra("notificationId");
                }
                catch (System.Exception)
                {
                }

                StartActivity(MainIntent);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}