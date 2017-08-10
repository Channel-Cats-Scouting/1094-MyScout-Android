using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Choose a Team", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material")]
    public class TeamActivity : Activity
    {
        // Variables/Constants
        public static TeamAdapter TeamListAdapter;
        protected RecyclerView teamList;
        protected Button teamAddBtn;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TeamLayout);

            // Assign local references to GUI elements
            teamList = FindViewById<RecyclerView>(Resource.Id.TeamList);
            teamAddBtn = FindViewById<Button>(Resource.Id.TeamAddBtn);

            // Setup Team List
            var layoutManager = new LinearLayoutManager(this);
            teamList.SetLayoutManager(layoutManager);

            TeamListAdapter = new TeamAdapter(this);
            teamList.SetAdapter(TeamListAdapter);

            var callback = new TeamEntryTouchHelper(TeamListAdapter);
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(teamList);

            // Assign events to GUI elements
            teamAddBtn.Click += TeamAddBtn_Click;
        }

        private void TeamAddBtn_Click(object sender, EventArgs e)
        {
            // TODO: Bring up the team add dialog instead of running this debug code
            TeamListAdapter.Add(new Team($"Team {MainActivity.Teams.Count+1}",
                (MainActivity.Teams.Count + 1000).ToString()));
        }
    }
}