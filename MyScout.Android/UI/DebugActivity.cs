using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using System;
using Android.Util;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", Icon = "@drawable/icon")]
    public class DebugActivity : Activity
    {
        // Variables/Constants
        public static Team SelectedTeam; // TODO: Remove this from DebugActivity

        protected BluetoothAdapter adapter; // TODO: Remove this from DebugActivity
        protected Button chooseTeamBtn, settingsBtn, roundBtn, scoutMasterBtn,
            deleteConfigBtn, saveEventBtn, nextRoundBtn;

        protected const int GET_TEAM_REQUEST = 0, GET_TEAM_CONNECT_REQUEST = 1;

        // Methods
        private void ShowToast(string text, ToastLength duration)
        {
            Toast.MakeText(this, text, duration).Show();
        }

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DebugLayout);

            // Assign local references to GUI elements
            chooseTeamBtn = FindViewById<Button>(Resource.Id.TeamChooseBtn);
            settingsBtn = FindViewById<Button>(Resource.Id.OpenSettingsBtn);
            roundBtn = FindViewById<Button>(Resource.Id.OpenRoundBtn);
            scoutMasterBtn = FindViewById<Button>(Resource.Id.OpenScoutMasterBtn);

            deleteConfigBtn = FindViewById<Button>(Resource.Id.DeleteConfigBtn);
            saveEventBtn = FindViewById<Button>(Resource.Id.SaveCurrentEventBtn);
            nextRoundBtn = FindViewById<Button>(Resource.Id.ScoutMasterTestBtn);

            // Assign events to GUI elements
            chooseTeamBtn.Click += ChooseTeamBtn_Click;
            settingsBtn.Click += SettingsBtn_Click;
            roundBtn.Click += RoundBtn_Click;
            scoutMasterBtn.Click += ScoutMasterBtn_Click;

            deleteConfigBtn.Click += DeleteConfigBtn_Click;
            saveEventBtn.Click += SaveEventBtn_Click;
            nextRoundBtn.Click += NextRoundBtn_Click;

            // Debug stuff™
            if (Event.Current == null)
            {
                Event.Current = new Event()
                {
                    Name = "Test Event",
                };

                Event.Current.Rounds.Add(new Round());
                ++Event.Current.CurrentRoundIndex;
            }
        }

        protected override void OnActivityResult(
            int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == GET_TEAM_REQUEST || requestCode == GET_TEAM_CONNECT_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    // Set the selected team using the index returned from the team activity
                    int selectedTeamIndex = data.GetIntExtra("SelectedItemIndex", -1);
                    if (selectedTeamIndex < 0) return;
                    SelectedTeam = Event.Current.Teams[selectedTeamIndex];

                    // TODO: Remove this
                    if (requestCode == GET_TEAM_REQUEST)
                    {
                        ShowToast($"Selected team \"{SelectedTeam.Name}\"", ToastLength.Short);
                    }
                    else
                    {
                        NextRoundBtn_Click(null, null);
                    }
                }
            }
        }

        private void ChooseTeamBtn_Click(object sender, EventArgs e)
        {
            // TODO: Remove this debug event
            StartActivityForResult(
                typeof(TeamActivity), GET_TEAM_REQUEST);
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SettingsActivity));
        }

        private void RoundBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RoundActivity));
        }

        private void ScoutMasterBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ScoutMasterActivity));
        }

        private void DeleteConfigBtn_Click(object sender, EventArgs e)
        {
            System.IO.File.Delete(Config.FilePath);
            ShowToast("Removed config file", ToastLength.Short);
        }

        private void SaveEventBtn_Click(object sender, EventArgs e)
        {
            if (Event.Current == null) return;
            if (string.IsNullOrEmpty(Event.Current.Name))
            {
                Event.Current.Name = "Test Event";
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            Event.Current.Save();
            sw.Stop();

            ShowToast(string.Format("Saved \"{0}\" in {1}(ms).",
                Event.Current.Name, sw.ElapsedMilliseconds), ToastLength.Long);
        }

        private void NextRoundBtn_Click(object sender, EventArgs e)
        {
            if (Event.Current == null)
            {
                Event.Current = new Event()
                {
                    Name = "Test Event",
                };
            }

            // If no team is selected, get one first
            if (SelectedTeam == null)
            {
                StartActivityForResult
                    (typeof(TeamActivity), GET_TEAM_CONNECT_REQUEST);
                return;
            }

            ShowToast("Assigning selected team(s)...", ToastLength.Short);

            // Setup the next round
            var round = new Round();
            /*round.TeamData.Length*/
            for (int i = 0; i < Config.RegisteredDevices.Count; ++i)
            {
                round.TeamData[i] = new TeamData(SelectedTeam);
            }

            Event.Current.Rounds.Add(round);
            ++Event.Current.CurrentRoundIndex;

            // TODO: Assign Teams to scouts automatically based on tablet color

            // Send data to the scouts
            var adapter = BluetoothIO.Adapter;
            for (int i = 0; i < Config.RegisteredDevices.Count; ++i)
            {
                // Get the device
                var scout = Config.RegisteredDevices[i];
                var device = adapter.GetRemoteDevice(scout);

                // Establish a connection with the scout
                ScoutMasterConnection connection;
                if (BluetoothIO.Connections.Count <= i)
                {
                    connection = new ScoutMasterConnection(device);
                    connection.Start();
                    BluetoothIO.Connections.Add(connection);
                }
                else
                {
                    connection = (BluetoothIO.Connections[i] as ScoutMasterConnection);
                }

                if (connection == null)
                    continue;

                // Send over the selected team's info on the next update loop
                connection.SendRoundInfo(i, Event.Current.CurrentRoundIndex);
            }
        }
    }
}