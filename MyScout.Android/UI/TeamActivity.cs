using Android.App;
using Android.OS;

namespace MyScout.Android.UI
{
    [Activity(Label = "Choose a Team", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material")]
    public class TeamActivity : ListActivity<Team>
    {
        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Change UI elements specific to this extension of ListActivity
            itemAddBtn.Text = "Add Team";

            // Setup List
            SetupList(Event.Current.Teams, typeof(EditTeamActivity));
        }
    }
}