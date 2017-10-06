using Android.App;
using Android.Content;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Edit Event", Icon = "@drawable/icon",
        Theme = "@style/MyScoutTheme")]
    public class EditEventActivity : ToolbarActivity
    {
        // Variables/Constants
        protected EditText eventNameTxtbx;
        protected Button eventDataSetBtn, okBtn;
        protected int eventIndex;

        // GUI Events
        protected override void OnCreate()
        {
            // Setup GUI
            SetContentView(Resource.Layout.EditEventLayout);

            // Assign local references to GUI elements
            eventNameTxtbx = FindViewById<EditText>(Resource.Id.EditEventNameTxtbx);
            eventDataSetBtn = FindViewById<Button>(Resource.Id.EditEventDataSetBtn);
            okBtn = FindViewById<Button>(Resource.Id.EditEventBtn);

            // Assign events to GUI elements
            eventDataSetBtn.Click += EventDataSetBtn_Click;
            okBtn.Click += OkBtn_Click;

            // Get event index
            eventIndex = Intent.GetIntExtra("ItemIndex", -1);

            // If an event index was assigned, allow the user to edit that event
            if (eventIndex >= 0)
            {
                var evnt = EventActivity.Events[eventIndex];
                eventNameTxtbx.Text = evnt.Name;
                eventDataSetBtn.Text = evnt.DataSetFileName;
            }

            // If no event index was assigned, we must be adding an event instead
            else
            {
                Title = "Create Event";
                eventDataSetBtn.Text = "2017Game.xml"; // TODO: Set DataSet properly
            }
        }

        protected void EventDataSetBtn_Click(object sender, EventArgs e)
        {
            // TODO: Bring up DataSet list
        }

        protected void OkBtn_Click(object sender, EventArgs e)
        {
            // Apply our changes and close the dialog
            if (eventIndex >= 0)
            {
                // Edit existing event data
                var evnt = EventActivity.Events[eventIndex];
                evnt.Name = eventNameTxtbx.Text;
                evnt.DataSetFileName = eventDataSetBtn.Text;
            }
            else
            {
                // Generate a new event
                var evnt = new Event()
                {
                    Name = eventNameTxtbx.Text,
                    DataSetFileName = eventDataSetBtn.Text,
                    CurrentRoundIndex = 0
                };

                evnt.Rounds.Add(new Round());

                // Save the event and add it as a new entry to the list
                evnt.Save();
                EventActivity.Events.Add(evnt);
            }

            Finish();
        }
    }
}