using Android.App;
using Android.OS;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Settings", Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material")]
    public class SettingsActivity : Activity
    {
        // Variables/Constants
        protected RadioGroup devicesGroup;
        protected RadioButton scoutOption, scoutMasterOption, redOption, blueOption;
        protected Button okBtn, refreshDevicesBtn, registerScoutBtn;

        // Methods
        private string GetSelectedDevice()
        {
            if (devicesGroup.CheckedRadioButtonId < 0) return null;

            // Get selected RadioButton
            var rb = devicesGroup.FindViewById<RadioButton>
                (devicesGroup.CheckedRadioButtonId);

            // Return device address
            return (string)rb.Tag;
        }

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SettingsLayout);

            // Assign local references to GUI elements
            scoutOption = FindViewById<RadioButton>(Resource.Id.SettingsScoutOption);
            scoutMasterOption = FindViewById<RadioButton>(Resource.Id.SettingsScoutMasterOption);
            redOption = FindViewById<RadioButton>(Resource.Id.SettingsRedOption);
            blueOption = FindViewById<RadioButton>(Resource.Id.SettingsBlueOption);
            devicesGroup = FindViewById<RadioGroup>(Resource.Id.BluetoothDevicesGroup);
            refreshDevicesBtn = FindViewById<Button>(Resource.Id.RefreshDevicesBtn);
            registerScoutBtn = FindViewById<Button>(Resource.Id.RegisterScoutBtn);
            okBtn = FindViewById<Button>(Resource.Id.SettingsOKBtn);

            // Assign events to GUI elements
            devicesGroup.CheckedChange += DevicesGroup_CheckedChange;
            refreshDevicesBtn.Click += RefreshBtn_Click;
            registerScoutBtn.Click += RegisterScoutBtn_Click;
            okBtn.Click += OkBtn_Click;

            // Load config file if not already loaded
            if (!Config.Loaded)
                Config.Load();

            // Change GUI elements to match settings
            scoutOption.Checked = (Config.TabletType == Config.TabletTypes.Scout);
            scoutMasterOption.Checked = (Config.TabletType == Config.TabletTypes.ScoutMaster);
            redOption.Checked = (Config.TabletColor == Config.AllianceColors.Red);
            blueOption.Checked = (Config.TabletColor == Config.AllianceColors.Blue);
        }

        private void DevicesGroup_CheckedChange(
            object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            // Enable the register scout button ONLY if a device is selected
            registerScoutBtn.Enabled = (e.CheckedId >= 0);
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            // Disable the register scout button
            registerScoutBtn.Enabled = false;

            // Prepare the Bluetooth adapter
            if (!BluetoothIO.PrepareAdapter())
            {
                // Show toast error message if the device doesn't have a Bluetooth adapter
                Toast.MakeText(this, BluetoothIO.NoAdapterMessage, ToastLength.Long);
                return;
            }

            // Update list of paired devices
            var adapter = BluetoothIO.Adapter;
            adapter.StartDiscovery();
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

            // Cancel device discovery as it slows down a Bluetooth connection
            adapter.CancelDiscovery();
        }

        private void RegisterScoutBtn_Click(object sender, EventArgs e)
        {
            string device = GetSelectedDevice();
            if (!string.IsNullOrEmpty(device))
            {
                Config.RegisteredDevices.Add(device);
                Toast.MakeText(this, "Registered device!",
                    ToastLength.Short).Show();
            }
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            // Change setings to match GUI elements
            Config.TabletType = (scoutMasterOption.Checked) ?
                Config.TabletTypes.ScoutMaster : Config.TabletTypes.Scout;

            Config.TabletColor = (redOption.Checked) ?
                Config.AllianceColors.Red : Config.AllianceColors.Blue;

            // Save config file
            Config.Save();

            // Close settings activity
            Finish();
        }
    }
}