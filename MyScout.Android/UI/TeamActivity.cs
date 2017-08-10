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
        protected RecyclerView teamList;
        protected TeamAdapter teamListAdapter;
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

            teamListAdapter = new TeamAdapter();
            teamList.SetAdapter(teamListAdapter);

            var callback = new TeamEntryTouchHelper(teamListAdapter);
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(teamList);

            // Assign events to GUI elements
            teamAddBtn.Click += TeamAddBtn_Click;
        }

        private void TeamAddBtn_Click(object sender, EventArgs e)
        {
            // TODO: Bring up the team add dialog instead of running this debug code
            teamListAdapter.Add(new Team($"Team {MainActivity.Teams.Count+1}",
                (MainActivity.Teams.Count + 1000).ToString()));
        }
    }
}