using Android.Bluetooth;
using Android.Util;
using System;
using System.IO;

namespace MyScout.Android
{
    public class ScoutMasterConnection : BluetoothConnection
    {
        // Variables/Constants
        protected Team teamToSend = null;
        protected Actions currentAction = Actions.None;

        protected enum Actions
        {
            None, SendTeam, SendDataSetInfo, SendDataSet
        }

        // Constructors
        public ScoutMasterConnection(BluetoothDevice device)
        {
            socket = device.CreateRfcommSocketToServiceRecord(MyScoutUUID);
        }

        // Methods
        public void SendRoundInfo(Team team)
        {
            teamToSend = team;
            currentAction = Actions.SendTeam;
        }

        protected override void UpdateLoop()
        {
            if (currentAction == Actions.None) return;

            try
            {
                switch (currentAction)
                {
                    case Actions.SendTeam:
                        WriteTeam();
                        break;

                    case Actions.SendDataSetInfo:
                        WriteDataSetInfo();
                        break;

                    case Actions.SendDataSet:
                        WriteDataSet();
                        break;
                }

                // TODO: Receive round data from connected scout if necessary
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Log.Error("MyScout", $"ERROR: {ex.Message}");
                #endif
            }
        }

        protected void WriteTeam()
        {
            // Send team data to connected scout
            writer.Write(teamToSend);
            teamToSend = null;
            currentAction = Actions.SendDataSetInfo;
        }

        protected void WriteDataSetInfo()
        {
            // Send DataSet FileName
            writer.Write(DataSet.CurrentFileName);

            // Wait to see if we need to send the DataSet over, or if the scout already has it
            bool hasDataSet = reader.ReadBoolean();
            currentAction = (hasDataSet) ?
                Actions.None : Actions.SendDataSet;
        }

        protected void WriteDataSet()
        {
            // Send the DataSet in it's entirity
            // TODO: Maybe compress this?
            string dataSetPath = Path.Combine(
                IO.DataSetDirectory, DataSet.CurrentFileName);

            var data = File.ReadAllBytes(dataSetPath);
            writer.Write(data.Length);
            writer.WriteInChunks(data, BluetoothIO.ChunkSize);

            currentAction = Actions.None;
        }
    }
}