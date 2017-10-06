using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@style/MyScoutTheme")]
    public class ToolbarActivity : AppCompatActivity
    {
        // Variables/Constants
        protected Toolbar toolbar;

        // GUI Events
        protected override sealed void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OnCreate();

            toolbar = FindViewById<Toolbar>(Resource.Id.MyScoutToolbar);
            SetSupportActionBar(toolbar);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }

        // Methods
        protected virtual void OnCreate() { }
    }
}