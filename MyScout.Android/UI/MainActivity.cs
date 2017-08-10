using Android.App;
using Android.OS;
using System.Collections.Generic;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = true,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        // Variables/Constants
        public static List<Team> Teams = new List<Team>(); // TODO: Remove this from MainActivity
        // TODO: Make a variable for each GUI element in MainLayout

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainLayout);

            // TODO: Remove this debug code
            var chooseTeamBtn = FindViewById(Resource.Id.TeamChooseBtn);
            chooseTeamBtn.Click += ChooseTeamBtn_Click;
        }

        private void ChooseTeamBtn_Click(object sender, System.EventArgs e)
        {
            // TODO: Remove this debug event
            StartActivity(typeof(TeamActivity));
        }
    }
}