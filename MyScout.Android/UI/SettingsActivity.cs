﻿using Android.App;
using Android.Views;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Settings", Icon = "@drawable/icon",
        Theme = "@style/MyScoutTheme")]
    public class SettingsActivity : ToolbarActivity
    {
        // Variables/Constants
        public static bool IsFirstTimeSetup = false;
        protected LinearLayout typeLayout, devicesLayout;
        protected RadioGroup typeGroup, devicesGroup;
        protected RadioButton scoutOption, scoutMasterOption;
        protected Button okBtn, refreshDevicesBtn, registerScoutBtn;
        protected TextView devicesTxt;

        // Methods
        protected string GetSelectedDevice()
        {
            if (devicesGroup.CheckedRadioButtonId < 0) return null;

            // Get selected RadioButton
            var rb = devicesGroup.FindViewById<RadioButton>
                (devicesGroup.CheckedRadioButtonId);

            // Return device address
            return (string)rb.Tag;
        }

        protected void UpdateUI()
        {
            if (scoutMasterOption.Checked)
            {
                devicesTxt.Text = "Register Scouts:";
                registerScoutBtn.Visibility = ViewStates.Visible;
                okBtn.Enabled = true;
            }
            else
            {
                devicesTxt.Text = "Scout Master Tablet:";
                registerScoutBtn.Visibility = ViewStates.Gone;
                okBtn.Enabled = (devicesGroup.CheckedRadioButtonId >= 0);
            }
        }

        // GUI Events
        protected override void OnCreate()
        {
            // Setup GUI
            SetContentView(Resource.Layout.SettingsLayout);

            // Assign local references to GUI elements
            typeLayout = FindViewById<LinearLayout>(Resource.Id.SettingsTypeLayout);
            typeGroup = FindViewById<RadioGroup>(Resource.Id.SettingsTypeGroup);
            scoutOption = FindViewById<RadioButton>(Resource.Id.SettingsScoutOption);
            scoutMasterOption = FindViewById<RadioButton>(Resource.Id.SettingsScoutMasterOption);

            devicesLayout = FindViewById<LinearLayout>(Resource.Id.SettingsDevicesLayout);
            devicesTxt = FindViewById<TextView>(Resource.Id.SettingsDevicesTxt);
            devicesGroup = FindViewById<RadioGroup>(Resource.Id.BluetoothDevicesGroup);
            refreshDevicesBtn = FindViewById<Button>(Resource.Id.RefreshDevicesBtn);
            registerScoutBtn = FindViewById<Button>(Resource.Id.RegisterScoutBtn);
            okBtn = FindViewById<Button>(Resource.Id.SettingsOKBtn);

            // Assign events to GUI elements
            typeGroup.CheckedChange += TypeGroup_CheckedChange;
            devicesGroup.CheckedChange += DevicesGroup_CheckedChange;
            refreshDevicesBtn.Click += RefreshBtn_Click;
            registerScoutBtn.Click += RegisterScoutBtn_Click;
            okBtn.Click += OkBtn_Click;

            // Change GUI elements to match settings
            if (IsFirstTimeSetup)
            {
                Title = "First-Time Setup";
            }

            // Load config file if not already loaded
            else if (!Config.Loaded)
            {
                Config.Load();
            }

            scoutOption.Checked = (Config.TabletType == Config.TabletTypes.Scout);
            scoutMasterOption.Checked = (Config.TabletType == Config.TabletTypes.ScoutMaster);
            UpdateUI();
        }

        private void TypeGroup_CheckedChange(
            object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            UpdateUI();
        }

        private void DevicesGroup_CheckedChange(
            object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            // Enable the register scout button ONLY if a device is selected
            registerScoutBtn.Enabled = (e.CheckedId >= 0);
            okBtn.Enabled = (scoutMasterOption.Checked || registerScoutBtn.Enabled);
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
            bool isScoutMaster = scoutMasterOption.Checked;
            Config.TabletType = (isScoutMaster) ?
                Config.TabletTypes.ScoutMaster : Config.TabletTypes.Scout;

            // Register scout master if this is a scout tablet
            if (!isScoutMaster)
            {
                string device = GetSelectedDevice();
                if (!string.IsNullOrEmpty(device))
                {
                    if (Config.RegisteredDevices.Count > 0)
                        Config.RegisteredDevices.Clear();

                    Config.RegisteredDevices.Add(device);
                }
            }

            // Save config file
            Config.Save();

            // Close settings activity
            if (IsFirstTimeSetup)
            {
                IsFirstTimeSetup = false;
                StartActivity(typeof(MainActivity));
            }

            Finish();
        }
    }
}