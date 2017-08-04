using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace DataTransferringPrototype
{
    [Activity(Label = "Data Transferring Prototype",
        MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Variables/Constants
        public static ProgressBar ProgressBar;
        public static TextView ProgressLbl;

        private BluetoothDevice device = null;
        private RadioGroup pairedDevices;
        private Button pickBtn, refreshBtn, hostBtn, joinBtn;

        // Methods
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            pairedDevices = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            pickBtn = FindViewById<Button>(Resource.Id.pickBtn);
            refreshBtn = FindViewById<Button>(Resource.Id.refreshBtn);
            hostBtn = FindViewById<Button>(Resource.Id.hostBtn);
            joinBtn = FindViewById<Button>(Resource.Id.joinBtn);
            ProgressLbl = FindViewById<TextView>(Resource.Id.progressLbl);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            pickBtn.Click += PickBtn_Click;
            refreshBtn.Click += RefreshBtn_Click;
            pairedDevices.CheckedChange += PairedDevices_CheckedChange;
            hostBtn.Click += HostBtn_Click;
            joinBtn.Click += JoinBtn_Click;

            BluetoothIO.MainActivityInstance = this;
        }

        private void Connect()
        {
            var rb = pairedDevices.FindViewById<RadioButton>
                (pairedDevices.CheckedRadioButtonId);

            // Get device
            var adapter = BluetoothIO.Adapter;
            device = adapter.GetRemoteDevice((string)rb.Tag);

            Toast.MakeText(this, $"Got remote device {adapter.Address}",
                ToastLength.Long).Show();
        }

        private void HostBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
                BluetoothIO.Host(device);
            }
            catch (Exception ex)
            {
                // TODO: Write log and catch errors on the thread
                Toast.MakeText(this, $"ERROR: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private void JoinBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
                BluetoothIO.Join();
            }
            catch (Exception ex)
            {
                // TODO: Write log and catch errors on the thread
                Toast.MakeText(this, $"ERROR: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private void PairedDevices_CheckedChange(object sender,
            RadioGroup.CheckedChangeEventArgs e)
        {
            hostBtn.Enabled = joinBtn.Enabled = (e.CheckedId >= 0);
        }

        private void PickBtn_Click(object sender, EventArgs e)
        {
            // TODO: Let the user pick an image from their gallery
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            // Disable the host/join buttons
            hostBtn.Enabled = joinBtn.Enabled = false;

            // Get the default Bluetooth adapter
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                Toast.MakeText(this, "This device does not have a Bluetooth adapter!",
                    ToastLength.Long).Show();

                refreshBtn.Enabled = false;
                return;
            }

            // Enable the Bluetooth adapter
            if (!adapter.IsEnabled)
                adapter.Enable();

            // Update list of paired devices
            pairedDevices.RemoveAllViews();

            foreach (var d in adapter.BondedDevices)
            {
                var rb = new RadioButton(this)
                {
                    Text = $"{d.Name} ({d.Address})",
                    Tag = d.Address
                };

                pairedDevices.AddView(rb);
            }

            BluetoothIO.Adapter = adapter;
        }
    }
}