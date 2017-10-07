using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using System.IO;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material.NoActionBar")]
    public class ScoutMasterActivity : Activity
    {
        // Variables/Constants
        public static ScoutMasterActivity Instance;
        public static int WaitingOnScouts = 0;

        protected Button[] teamSlots;
        protected Button prevBtn, nextBtn, exportBtn, startRoundBtn;
        protected TextView roundLbl;
        protected const int GET_TEAM_REQUEST = 0;
        private static int teamSlotEditIndex;

        // Methods
        public void UpdateUI()
        {
            if (WaitingOnScouts > 0)
            {
                startRoundBtn.Text = $"Waiting on {WaitingOnScouts} scout(s)...";
                startRoundBtn.Enabled = false;
            }
            else
            {
                // Perform end-of-round actions
                if (!startRoundBtn.Enabled)
                {
                    // Go to the next round automatically if there is one
                    if (Event.Current.CurrentRoundIndex < Event.Current.Rounds.Count - 1)
                    {
                        ++Event.Current.CurrentRoundIndex;
                    }

                    // Save the current event
                    // TODO: Do this on another thread
                    Event.Current.Save();

                    Toast.MakeText(this, "Event Saved.",
                        ToastLength.Short).Show();
                }

                // Enable everything
                prevBtn.Enabled = nextBtn.Enabled = startRoundBtn.Enabled = true;

                // Update round navigation bar
                int roundID = Event.Current.CurrentRoundIndex + 1;
                roundLbl.Text = $"Round {roundID}";
                prevBtn.Enabled = (roundID > 1);
                nextBtn.Text = (roundID >= Event.Current.Rounds.Count) ?
                    "+" : ">";

                // Update team slots
                UpdateTeamSlots();

                // Update start round button
                startRoundBtn.Text = "Start Round";
                // TODO: Update text on start round button to say "Redo Round"
                startRoundBtn.Enabled = ShouldEnableStartRoundBtn();
            }
        }

        public void UpdateTeamSlots()
        {
            // Enable team slots and update text shown on them
            var currentRound = Event.Current.CurrentRound;
            if (currentRound == null) return;

            var teamData = currentRound.TeamData;
            for (int i = 0; i < teamSlots.Length; ++i)
            {
                var teamSlot = teamSlots[i];
                teamSlot.Enabled = true;
                teamSlot.Text = (teamData[i] == null) ?
                    "- No Team Assigned -" : teamData[i].Team.ToString();
            }
        }

        protected bool ShouldEnableStartRoundBtn()
        {
            var currentRound = Event.Current.CurrentRound;
            if (currentRound == null) return false;

            var teamData = currentRound.TeamData;
            if (teamData == null) return false;

            for (int i = 0; i < teamData.Length; ++i)
            {
                if (teamData[i] == null)
                    return false;
            }

            return true;
        }

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Go to the event-selection screen if no event has been selected
            if (Event.Current == null)
            {
                OnBackPressed();
                return;
            }

            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScoutMasterLayout);
            Instance = this;

            // Assign local references to GUI elements
            prevBtn = FindViewById<Button>(Resource.Id.ScoutMasterPrevBtn);
            roundLbl = FindViewById<TextView>(Resource.Id.ScoutMasterRoundLbl);
            nextBtn = FindViewById<Button>(Resource.Id.ScoutMasterNextBtn);

            teamSlots = new Button[Round.TeamCount];
            teamSlots[0] = FindViewById<Button>(Resource.Id.RedTeamSlot1);
            teamSlots[1] = FindViewById<Button>(Resource.Id.RedTeamSlot2);
            teamSlots[2] = FindViewById<Button>(Resource.Id.RedTeamSlot3);
            teamSlots[3] = FindViewById<Button>(Resource.Id.BlueTeamSlot1);
            teamSlots[4] = FindViewById<Button>(Resource.Id.BlueTeamSlot2);
            teamSlots[5] = FindViewById<Button>(Resource.Id.BlueTeamSlot3);

            exportBtn = FindViewById<Button>(Resource.Id.ScoutMasterExportBtn);
            startRoundBtn = FindViewById<Button>(Resource.Id.ScoutMasterStartRoundBtn);

            // Assign events to GUI elements
            prevBtn.Click += PrevBtn_Click;
            nextBtn.Click += NextBtn_Click;

            foreach (var btn in teamSlots)
            {
                btn.Click += TeamSlot_Click;
            }

            exportBtn.Click += ExportBtn_Click;
            startRoundBtn.Click += StartRoundBtn_Click;

            // Update GUI Elements
            UpdateUI();
        }

        protected override void OnActivityResult(
            int requestCode, [GeneratedEnum]Result resultCode, Intent data)
        {
            if (requestCode == GET_TEAM_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    // Set the selected team using the index returned from the team activity
                    int selectedTeamIndex = data.GetIntExtra("SelectedItemIndex", -1);
                    if (selectedTeamIndex < 0) return;

                    var teamData = Event.Current.CurrentRound.TeamData;
                    teamData[teamSlotEditIndex] = new TeamData(
                        Event.Current.Teams[selectedTeamIndex]);

                    UpdateTeamSlots();
                    startRoundBtn.Enabled = ShouldEnableStartRoundBtn();
                }
            }
        }

        public override void OnBackPressed()
        {
            // TODO: Ask user if they would like to save the event
            Event.Current.Save();

            Instance = null;
            Event.Current = null;
            Finish();
        }

        protected void PrevBtn_Click(object sender, EventArgs e)
        {
            if (Event.Current.CurrentRoundIndex > 0)
            {
                --Event.Current.CurrentRoundIndex;
                UpdateUI();
            }
        }

        protected void NextBtn_Click(object sender, EventArgs e)
        {
            // Add a new round if there's no next round to go to
            if (Event.Current.CurrentRoundIndex >= Event.Current.Rounds.Count-1)
            {
                Event.Current.Rounds.Add(new Round());
            }

            ++Event.Current.CurrentRoundIndex;
            UpdateUI();
        }

        protected void TeamSlot_Click(object sender, EventArgs e)
        {
            // Get team slot ID
            var btn = (sender as Button);
            if (btn == null) return;

            int slotID = int.Parse((string)btn.Tag);

            // Open team select GUI
            teamSlotEditIndex = slotID;
            StartActivityForResult(
                typeof(TeamActivity), GET_TEAM_REQUEST);
        }

        protected void ExportBtn_Click(object sender, EventArgs e)
        {
            // TODO: Make this happen on another thread
            string dataSheetDir = Path.Combine(IO.ExternalDataDirectory, "DataSheets");
            Directory.CreateDirectory(dataSheetDir);

            string filePath = Path.Combine(dataSheetDir,
                $"Event{Event.Current.Index}.csv");
            Event.Current.ExportCSV(filePath);

            Toast.MakeText(this, $"Exported CSV to \"{filePath}\"",
                ToastLength.Long).Show();
        }

        protected void StartRoundBtn_Click(object sender, EventArgs e)
        {
            // Disable GUI Elements
            prevBtn.Enabled = nextBtn.Enabled = startRoundBtn.Enabled = false;
            foreach (var btn in teamSlots)
            {
                btn.Enabled = false;
            }

            // Send data to the scouts
            var adapter = BluetoothIO.Adapter;
            WaitingOnScouts = 0;

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
                    if (!connection.Active)
                    {
                        connection = new ScoutMasterConnection(device);
                        connection.Start();
                        BluetoothIO.Connections[i] = connection;
                    }
                }

                if (connection == null)
                    continue;

                // Send over the selected team's info on the next update loop
                // TODO: Open assign scout devices GUI
                ++WaitingOnScouts;
                connection.SendRoundInfo(i, Event.Current.CurrentRoundIndex);
            }

            UpdateUI();
        }
    }
}