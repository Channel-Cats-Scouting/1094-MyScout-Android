using Android.App;
using Android.Content;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Edit Team", Icon = "@drawable/icon",
        Theme = "@style/MyScoutTheme")]
    public class EditTeamActivity : ToolbarActivity
    {
        // Variables/Constants
        protected EditText teamIDTxtbx, teamNameTxtbx;
        protected Button okBtn;
        protected int teamIndex;

        // GUI Events
        protected override void OnCreate()
        {
            // Setup GUI
            SetContentView(Resource.Layout.EditTeamLayout);

            // Assign local references to GUI elements
            teamIDTxtbx = FindViewById<EditText>(Resource.Id.EditTeamIDTxtbx);
            teamNameTxtbx = FindViewById<EditText>(Resource.Id.EditTeamNameTxtbx);
            okBtn = FindViewById<Button>(Resource.Id.EditTeamBtn);

            // Assign events to GUI elements
            okBtn.Click += OkBtn_Click;

            // Get team index
            teamIndex = Intent.GetIntExtra("ItemIndex", -1);

            // If a team index was assigned, we should allow the user to edit that team
            if (teamIndex >= 0)
            {
                var team = Event.Current.Teams[teamIndex];
                teamNameTxtbx.Text = team.Name;
                teamIDTxtbx.Text = team.ID;
            }

            // If no team index was assigned to the dialog, we must be adding a team instead
            else
            {
                Title = "Add Team";
            }
        }

        protected void OkBtn_Click(object sender, EventArgs e)
        {
            // Apply our changes and close the dialog
            var team = new Team(teamNameTxtbx.Text, teamIDTxtbx.Text);
            if (teamIndex >= 0)
            {
                // Edit existing team data
                Event.Current.Teams[teamIndex] = team;
            }
            else
            {
                // Add team as new entry to list
                Event.Current.Teams.Add(team);
            }

            Finish();
        }
    }
}