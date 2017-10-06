using Android.App;

namespace MyScout.Android.UI
{
    [Activity(Label = "Choose a Team", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@style/MyScoutTheme")]
    public class TeamActivity : ListActivity<Team>
    {
        // GUI Events
        protected override void OnCreate()
        {
            base.OnCreate();

            // Setup List
            SetupList(Event.Current.Teams, typeof(EditTeamActivity));
        }
    }
}