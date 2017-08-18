using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = true,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        // Variables/Constants
        public static MainActivity MainActivityInstance;
        public static List<BluetoothDevice>
            RegisteredScouts = new List<BluetoothDevice>(); // TODO: Remove this from MainActivity

        public static List<Team> Teams = new List<Team>();  // TODO: Remove this from MainActivity
        public static Team SelectedTeam; // TODO: Remove this from MainActivity

        protected BluetoothAdapter adapter; // TODO: Remove this from MainActivity
        protected RadioGroup devicesGroup;
        protected Button chooseTeamBtn, scoutMasterBtn,
            scoutBtn, refreshBtn, registerScoutBtn;

        protected const int GET_TEAM_REQUEST = 0, GET_TEAM_CONNECT_REQUEST = 1;

        // Methods
        private BluetoothDevice GetSelectedDevice()
        {
            var rb = devicesGroup.FindViewById<RadioButton>
                (devicesGroup.CheckedRadioButtonId);

            // Get device
            var device = adapter.GetRemoteDevice((string)rb.Tag);
            ShowToast($"Got remote device {adapter.Address}", ToastLength.Long);

            return device;
        }

        private void ShowToast(string text, ToastLength duration)
        {
            Toast.MakeText(this, text, duration).Show();
        }

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            MainActivityInstance = this;

            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainLayout);

            // Assign local references to GUI elements
            chooseTeamBtn = FindViewById<Button>(Resource.Id.TeamChooseBtn);
            scoutMasterBtn = FindViewById<Button>(Resource.Id.ScoutMasterTestBtn);
            scoutBtn = FindViewById<Button>(Resource.Id.ScoutTestBtn);
            refreshBtn = FindViewById<Button>(Resource.Id.RefreshDevicesBtn);
            registerScoutBtn = FindViewById<Button>(Resource.Id.RegisterScoutBtn);
            devicesGroup = FindViewById<RadioGroup>(Resource.Id.BluetoothDevicesGroup);

            // Assign events to GUI elements
            chooseTeamBtn.Click += ChooseTeamBtn_Click;
            scoutMasterBtn.Click += ScoutMasterBtn_Click;
            scoutBtn.Click += ScoutBtn_Click;
            refreshBtn.Click += RefreshBtn_Click;
            registerScoutBtn.Click += RegisterScoutBtn_Click;
            devicesGroup.CheckedChange += DevicesGroup_CheckedChange;
        }

        protected override void OnActivityResult(
            int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == GET_TEAM_REQUEST || requestCode == GET_TEAM_CONNECT_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    // Set the selected team using the index returned from the team activity
                    int selectedTeamIndex = data.GetIntExtra("SelectedTeamIndex", -1);
                    if (selectedTeamIndex < 0) return;
                    SelectedTeam = Teams[selectedTeamIndex];

                    // TODO: Remove this
                    if (requestCode == GET_TEAM_REQUEST)
                    {
                        ShowToast($"Selected team \"{SelectedTeam.Name}\"", ToastLength.Short);
                    }
                    else
                    {
                        ScoutMasterBtn_Click(null, null);
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

        private void ScoutMasterBtn_Click(object sender, EventArgs e)
        {
            // If no team is selected, get one first
            if (SelectedTeam == null)
            {
                StartActivityForResult
                    (typeof(TeamActivity), GET_TEAM_CONNECT_REQUEST);
                return;
            }

            ShowToast("Assigning selected teams...", ToastLength.Short);

            foreach (var scout in RegisteredScouts)
            {
                // Establish a connection with the scout
                var connection = new ScoutMasterConnection(scout);
                connection.Start();

                // Send over the selected team's info on the next update loop
                connection.SendTeam(SelectedTeam);
            }

            scoutMasterBtn.Enabled = scoutBtn.Enabled = false;
        }

        private void ScoutBtn_Click(object sender, EventArgs e)
        {
            // Establish a connection with the scout master
            var connection = new ScoutConnection(adapter);
            connection.Start();
            connection.ListenForTeam();

            scoutMasterBtn.Enabled = scoutBtn.Enabled = false;
            ShowToast("Listening...", ToastLength.Short);
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            // Disable connect buttons
            scoutMasterBtn.Enabled = scoutBtn.Enabled = false;

            // Get the default Bluetooth adapter
            adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                ShowToast(
                    "This device does not have a Bluetooth adapter!", ToastLength.Long);

                refreshBtn.Enabled = false;
                return;
            }

            // Enable the Bluetooth adapter
            if (!adapter.IsEnabled)
                adapter.Enable();

            // Update list of paired devices
            devicesGroup.RemoveAllViews();

            foreach (var d in adapter.BondedDevices)
            {
                var rb = new RadioButton(this)
                {
                    Text = $"{d.Name} ({d.Address})",
                    Tag = d.Address
                };

                devicesGroup.AddView(rb);
            }
        }

        private void RegisterScoutBtn_Click(object sender, EventArgs e)
        {
            var scout = GetSelectedDevice();
            RegisteredScouts.Add(scout);

            ShowToast($"Registered scout \"{scout.Name}\" ({scout.Address})", ToastLength.Long);
        }

        private void DevicesGroup_CheckedChange(
            object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            scoutMasterBtn.Enabled = scoutBtn.Enabled = (e.CheckedId >= 0);
        }
    }
}