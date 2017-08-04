using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.IO;

namespace XamarinPrototype2
{
    [Activity(Label = "Data Transferring Prototype #1",
        MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Variables/Constants
        String msg = "YO BEEN HACKED";

        // Methods
        private void Save(String MSG)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.Path + "/ScoutingData";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Android.OS.Environment.ExternalStorageDirectory.Path + "/ScoutingData");
                Toast.MakeText(this, path + " Was Created", ToastLength.Long).Show();
            }
            string filePath = Path.Combine(path, "HackS.txt");
            if (File.Exists(filePath))
            {
                StreamWriter Existstream = File.CreateText(filePath);
                Existstream.Write(MSG);
                Toast.MakeText(this, filePath + " Was Updated", ToastLength.Long).Show();
                Existstream.Close();
            }
            else {
                StreamWriter stream = File.CreateText(filePath);
                stream.Write(MSG);
                Toast.MakeText(this, filePath + " Was Created", ToastLength.Long).Show();
                stream.Close();
            }
        }

        #region GUI

        // GUI Controls
        private Button btEnableBtn;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            btEnableBtn = FindViewById<Button>(Resource.Id.btEnableBtn);

            btEnableBtn.Click += BTTstBtn_Click;
        }

        private void BTTstBtn_Click(object sender, EventArgs e)
        {
            Save(msg);

            }
            //bt.GetRemoteDevice("");

            // TODO
        }

        #endregion
    }