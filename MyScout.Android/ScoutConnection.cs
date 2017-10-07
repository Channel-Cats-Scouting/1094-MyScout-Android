using Android.Bluetooth;
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
        protected object[] roundAutoData, roundTeleOPData;
        protected Actions currentAction = Actions.None;
        private string dataSetFilePath;
        private bool startListeningOnceDone = false;

        protected enum Actions
        {
            None, ListenForTeam, ListenForDataSetInfo,
            ReceiveDataSet, WriteRoundData
        }

        // Constructors
        public ScoutConnection(BluetoothAdapter adapter)
        {
            serverSocket = adapter.ListenUsingRfcommWithServiceRecord(
                "MyScout_Scout", MyScoutUUID);
        }

        // Methods
        public void StartListening()
        {
            if (currentAction != Actions.None)
            {
                startListeningOnceDone = true;
            }
            else
            {
                startListeningOnceDone = false;
                currentAction = Actions.ListenForTeam;
            }
        }

        public void WriteRoundData(object[] autoData, object[] teleOPData)
        {
            roundAutoData = autoData;
            roundTeleOPData = teleOPData;
            currentAction = Actions.WriteRoundData;
        }

        protected override bool Connect()
        {
            while (socket == null)
            {
                try
                {
                    // Accept incoming connection requests and ensure the
                    // accepted connection is from the registered scout master
                    socket = serverSocket.Accept();

                    if (Config.RegisteredDevices.Count < 1 ||
                        socket.RemoteDevice.Address != Config.RegisteredDevices[0])
                    {
                        CloseSocket();
                        return false;
                    }

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

                    case Actions.WriteRoundData:
                        WriteRoundData();
                        break;
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Log.Error("MyScout", $"ERROR: {ex.Message}");
                #endif

                socket = null;
                serverSocket = BluetoothIO.Adapter.ListenUsingRfcommWithServiceRecord(
                    "MyScout_Scout", MyScoutUUID);
            }

            // Start listening for team data if ready
            if (startListeningOnceDone)
            {
                StartListening();
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

        protected void WriteRoundData()
        {
            var dataSet = DataSet.Current;
            writer.Write(true);

            // Write Autonomous Data
            for (int i = 0; i < roundAutoData.Length; ++i)
            {
                writer.WriteByType(roundAutoData[i],
                    dataSet.RoundAutoData[i].DataType);
            }

            // Write Tele-OP Data
            for (int i = 0; i < roundTeleOPData.Length; ++i)
            {
                writer.WriteByType(roundTeleOPData[i],
                    dataSet.RoundTeleOPData[i].DataType);
            }

            // TODO: Remove this debug code
            MainActivity.Instance.RunOnUiThread(new Action(() =>
            {
                Toast.MakeText(MainActivity.Instance,
                    "Sent data", ToastLength.Long).Show();
            }));

            roundAutoData = roundTeleOPData = null;
            currentAction = Actions.None;
        }
    }
}