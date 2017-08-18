using Android.Bluetooth;
using Android.Util;
using System;

namespace MyScout.Android
{
    public class ScoutMasterConnection : BluetoothConnection
    {
        // Variables/Constants
        protected Team teamToSend = null;

        // Constructors
        public ScoutMasterConnection(BluetoothDevice device)
        {
            socket = device.CreateRfcommSocketToServiceRecord(MyScoutUUID);
        }

        // Methods
        public void SendTeam(Team team)
        {
            teamToSend = team;
        }

        protected override void UpdateLoop()
        {
            if (teamToSend == null) return;

            try
            {
                // Send team data to connected scout
                writer.Write(teamToSend);
                teamToSend = null;

                // TODO: Receive round data from connected scout if necessary
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