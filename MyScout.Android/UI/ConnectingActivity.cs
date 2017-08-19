using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material.NoActionBar")]
    public class ConnectingActivity : Activity
    {
        // Variables/Constants
        protected ProgressBar progressBar;
        protected TextView connectingLbl;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConnectingLayout);

            // Assign local references to GUI elements
            connectingLbl = FindViewById<TextView>(Resource.Id.ConnectingLbl);
            progressBar = FindViewById<ProgressBar>(Resource.Id.ConnectingProgressCircle);

            // Set the progress bar color
            progressBar.IndeterminateDrawable.SetColorFilter(
                new Color(229, 124, 39), PorterDuff.Mode.Multiply);

            // TODO: Remove this debug code
            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}