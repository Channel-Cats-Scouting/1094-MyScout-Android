using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Edit Team", Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material")]
    public class EditTeamActivity : Activity
    {
        // Variables/Constants
        protected EditText teamIDTxtbx, teamNameTxtbx;
        protected Button okBtn;
        protected int teamIndex;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EditTeamLayout);

            // Assign local references to GUI elements
            teamIDTxtbx = FindViewById<EditText>(Resource.Id.EditTeamIDTxtbx);
            teamNameTxtbx = FindViewById<EditText>(Resource.Id.EditTeamNameTxtbx);
            okBtn = FindViewById<Button>(Resource.Id.EditTeamBtn);

            // Assign events to GUI elements
            okBtn.Click += OkBtn_Click;

            // Get team index
            teamIndex = Intent.GetIntExtra("TeamIndex", -1);

            // If a team index was assigned, we should allow the user to edit that team
            if (teamIndex >= 0)
            {
                var team = MainActivity.Teams[teamIndex]; // TODO: Change this
                teamNameTxtbx.Text = team.Name;
                teamIDTxtbx.Text = team.ID;
            }

            // If no team index was assigned to the dialog, we must be adding a team instead
            else
            {
                Title = "Add Team";
            }
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            // Apply our changes and close the dialog
            var team = new Team(teamNameTxtbx.Text, teamIDTxtbx.Text);
            if (teamIndex >= 0)
            {
                // Edit existing team data
                MainActivity.Teams[teamIndex] = team; // TODO: Change this
            }
            else
            {
                // Add team as new entry to list
                MainActivity.Teams.Add(team); // TODO: Change this
            }

            Finish();
        }
    }
}