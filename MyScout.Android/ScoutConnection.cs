using Android.Bluetooth;
using Android.Util;
using Android.Widget;
using MyScout.Android.UI;
using System;

namespace MyScout.Android
{
    public class ScoutConnection : BluetoothConnection
    {
        // Variables/Constants
        protected BluetoothServerSocket serverSocket;
        protected bool doListen = false;

        // Constructors
        public ScoutConnection(BluetoothAdapter adapter)
        {
            // TODO: Only accept connections from the registered scout master
            serverSocket = adapter.ListenUsingRfcommWithServiceRecord(
                "MyScout_Scout", MyScoutUUID);
        }

        // Methods
        public void ListenForTeam()
        {
            doListen = true;
        }

        protected override bool Connect()
        {
            // Accept any incoming connection requests
            while (socket == null)
            {
                try
                {
                    socket = serverSocket.Accept();
                    reader = new ExtendedBinaryReader(socket.InputStream);
                    writer = new ExtendedBinaryWriter(socket.OutputStream);
                    return true;
                }
                catch
                {
                    // Attempt to close the socket which failed to connect
                    try
                    {
                        CloseSocket();
                    }
                    catch { }

                    // Connection will be re-attempted on next loop if connectAttempts
                    // does not exceed the maximum allowed number of attempts.
                    if (++connectAttempts > MaxConnectAttempts)
                    {
                        // TODO: Display error
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void UpdateLoop()
        {
            if (!doListen) return;

            try
            {
                // Block this thread until team data is read, or an exception is thrown
                var team = reader.ReadTeam();

                // TODO: Assign received team data to UI elements

                // (TODO: Remove this) Show toast message on MainActivity
                MainActivity.Instance.RunOnUiThread(() =>
                {
                    var toast = Toast.MakeText(MainActivity.Instance,
                        $"Received team: {team.ID} - {team.Name}", ToastLength.Long);
                    toast.Show();
                });

                doListen = false;

                // TODO: Send round data to scout master if necessary
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Log.Error("MyScout", $"ERROR: {ex.Message}");
                #endif
            }
        }
    }
}