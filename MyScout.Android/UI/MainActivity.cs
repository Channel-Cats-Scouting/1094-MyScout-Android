using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material.NoActionBar")]
    public class MainActivity : Activity
    {
        // Variables/Constants
        public static MainActivity Instance; // TODO: Remove this line
        protected ProgressBar progressBar;
        protected TextView connectingLbl;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            Instance = this; // TODO: Remove this line

            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConnectingLayout);

            // Assign local references to GUI elements
            connectingLbl = FindViewById<TextView>(Resource.Id.ConnectingLbl);
            progressBar = FindViewById<ProgressBar>(Resource.Id.ConnectingProgressCircle);

            // Set the progress bar color
            progressBar.IndeterminateDrawable.SetColorFilter(
                new Color(229, 124, 39), PorterDuff.Mode.Multiply);

            Init();
        }

        // Methods
        protected void Init()
        {
            // Load the config file if not already loaded
            if (!Config.Loaded)
            {
                // Attempt to load the config file
                if (!Config.Load())
                {
                    // Loading failed! Go to the first-time setup screen
                    // TODO: Go to first-time setup screen instead
                    StartActivity(typeof(SettingsActivity));
                    // TODO: Call Finish here to prevent user from going back to this screen
                    return;
                }
            }

            // Call the appropriate method based on the assigned tablet type
            switch (Config.TabletType)
            {
                case Config.TabletTypes.Scout:
                    ScoutInit();
                    break;

                case Config.TabletTypes.ScoutMaster:
                    ScoutMasterInit();
                    break;
            }
        }

        protected void ScoutInit()
        {
            // Connect to scout master if not already connected
            if (BluetoothIO.Connections.Count != 1)
            {
                connectingLbl.Text = "Connecting...";
                if (!ConnectToScoutMaster()) return;
            }

            // Get the connection to the scout master as a ScoutConnection
            var connection = (BluetoothIO.Connections[0] as ScoutConnection);
            if (connection == null) return;

            // Start listening for teams
            connectingLbl.Text = "Waiting for Scout Master to assign team...";
            connection.ListenForTeam();
        }

        protected void ScoutMasterInit()
        {
            // TODO: Go to entry-selection activity instead
            StartActivity(typeof(DebugActivity));
            Finish();
        }

        protected bool ConnectToScoutMaster()
        {
            // Prepare the Bluetooth adapter
            if (!BluetoothIO.PrepareAdapter())
            {
                // Show toast error message if the device doesn't have a Bluetooth adapter
                Toast.MakeText(this, BluetoothIO.NoAdapterMessage,
                    ToastLength.Long).Show();
                return false;
            }

            // Establish a connection to the scout master
            var connection = new ScoutConnection(BluetoothIO.Adapter);
            connection.Start();

            // Add this connection to the list of Bluetooth connections
            BluetoothIO.ClearAllConnections();
            BluetoothIO.Connections.Add(connection);
            return true;
        }
    }
}