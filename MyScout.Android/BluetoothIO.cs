using Android.Bluetooth;
using System.Collections.Generic;

namespace MyScout.Android
{
    public static class BluetoothIO
    {
        // Variables/Constants
        public static List<BluetoothConnection> Connections = new List<BluetoothConnection>();
        public static BluetoothAdapter Adapter
        {
            get
            {
                // Get the default bluetooth adapter
                if (adapter == null)
                    GetBluetoothAdapter();

                return adapter;
            }
        }

        private static BluetoothAdapter adapter = null;
        public const string NoAdapterMessage =
            "ERROR: This device does not have a Bluetooth adapter!";
        public const int ChunkSize = 2048;

        // Methods
        public static void ClearAllConnections()
        {
            // Close all connections
            foreach (var connection in Connections)
            {
                connection.Close();
            }

            // Clear the list of connections
            Connections.Clear();
        }

        public static bool PrepareAdapter()
        {
            // Return if the device doesn't have a Bluetooth adapter
            if (Adapter == null)
                return false;

            // Enable the Bluetooth adapter if not already enabled
            if (!adapter.IsEnabled)
                adapter.Enable();

            // Cancel device discovery as it slows down a Bluetooth connection
            adapter.CancelDiscovery();
            return true;
        }

        private static void GetBluetoothAdapter()
        {
            // This was implemented as a method in case support for multiple
            // Bluetooth adapters gets added to Android in the future.
            adapter = BluetoothAdapter.DefaultAdapter;
        }
    }
}