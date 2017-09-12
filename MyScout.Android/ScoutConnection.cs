﻿using Android.Bluetooth;
using Android.Util;
using Android.Widget;
using MyScout.Android.UI;
using System;
using System.IO;

namespace MyScout.Android
{
    public class ScoutConnection : BluetoothConnection
    {
        // Variables/Constants
        protected BluetoothServerSocket serverSocket;
        protected Actions currentAction = Actions.None;
        private string dataSetFilePath;

        protected enum Actions
        {
            None, ListenForTeam, ListenForDataSetInfo, ReceiveDataSet
        }

        // Constructors
        public ScoutConnection(BluetoothAdapter adapter)
        {
            // TODO: Only accept connections from the registered scout master
            serverSocket = adapter.ListenUsingRfcommWithServiceRecord(
                "MyScout_Scout", MyScoutUUID);
        }

        // Methods
        public void StartListening()
        {
            currentAction = Actions.ListenForTeam;
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
            if (currentAction == Actions.None) return;

            try
            {
                switch (currentAction)
                {
                    case Actions.ListenForTeam:
                        ListenForTeam();
                        break;

                    case Actions.ListenForDataSetInfo:
                        ListenForDataSetInfo();
                        break;

                    case Actions.ReceiveDataSet:
                        ReceiveDataSet();
                        break;
                }

                // TODO: Send round data to scout master if necessary
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Log.Error("MyScout", $"ERROR: {ex.Message}");
                #endif
            }
        }

        protected void ListenForTeam()
        {
            // Block this thread until team data is read, or an exception is thrown
            var team = reader.ReadTeam();
            RoundActivity.CurrentTeam = team;
            currentAction = Actions.ListenForDataSetInfo;
        }

        protected void ListenForDataSetInfo()
        {
            // Read the file name of the DataSet
            var dataSetFileName = reader.ReadString();
            DataSet.CurrentFileName = dataSetFileName;

            dataSetFilePath = Path.Combine(
                IO.DataSetDirectory, dataSetFileName);

            if (File.Exists(dataSetFilePath))
            {
                // Let the scout master know we already have the DataSet
                writer.Write(true);
                currentAction = Actions.None;

                // Load the DataSet
                LoadDataSet();
            }
            else
            {
                // Let the scout master know we need the DataSet
                writer.Write(false);
                currentAction = Actions.ReceiveDataSet;
            }
        }

        protected void ReceiveDataSet()
        {
            // Download the DataSet from the scout master
            int fileSize = reader.ReadInt32();
            var data = reader.ReadInChunks(fileSize, BluetoothIO.ChunkSize);
            currentAction = Actions.None;

            // Save the downloaded DataSet to the DataSets directory and load it
            File.WriteAllBytes(dataSetFilePath, data);
            LoadDataSet();
        }

        protected void LoadDataSet()
        {
            var dataSet = new DataSet();
            dataSet.Load(dataSetFilePath);
            DataSet.Current = dataSet;

            // Go to Round UI
            MainActivity.Instance.RunOnUiThread(() =>
            {
                MainActivity.Instance.StartActivity(typeof(RoundActivity));
                MainActivity.Instance.Finish();
            });
        }
    }
}