using Android.Bluetooth;
using Android.Util;
using MyScout.Android.UI;
using System;
using System.IO;

namespace MyScout.Android
{
    public class ScoutMasterConnection : BluetoothConnection
    {
        // Variables/Constants
        protected Round round = null;
        protected Team teamToSend = null;
        protected int teamDataIndex;
        protected Actions currentAction = Actions.None;

        protected enum Actions
        {
            None, SendTeam, SendDataSetInfo,
            SendDataSet, ReadRoundData
        }

        // Constructors
        public ScoutMasterConnection(BluetoothDevice device)
        {
            socket = device.CreateRfcommSocketToServiceRecord(MyScoutUUID);
        }

        // Methods
        public void SendRoundInfo(int roundTeamIndex, int roundIndex)
        {
            round = Event.Current.Rounds[roundIndex];
            var teamData = round.TeamData[roundTeamIndex];

            teamDataIndex = roundTeamIndex;
            teamToSend = teamData.Team;
            currentAction = Actions.SendTeam;
        }

        public override void Close()
        {
            UpdateScoutMasterUI();
            base.Close();
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

                    case Actions.ReadRoundData:
                        ReadRoundData();
                        break;
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Log.Error("MyScout", $"ERROR: {ex.Message}");
                #endif

                Close();
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
            writer.Write(Event.Current.DataSetFileName);

            // Wait to see if we need to send the DataSet over, or if the scout already has it
            bool hasDataSet = reader.ReadBoolean();
            currentAction = (hasDataSet) ?
                Actions.ReadRoundData : Actions.SendDataSet;
        }

        protected void WriteDataSet()
        {
            // Send the DataSet in it's entirity
            // TODO: Maybe compress this?
            string dataSetPath = Path.Combine(
                IO.DataSetDirectory, Event.Current.DataSetFileName);

            var data = File.ReadAllBytes(dataSetPath);
            writer.Write(data.Length);
            writer.WriteInChunks(data, BluetoothIO.ChunkSize);

            currentAction = Actions.ReadRoundData;
        }

        protected void ReadRoundData()
        {
            var dataSet = DataSet.Current;
            var teamData = round.TeamData[teamDataIndex];

            // Wait until ready
            if (!reader.ReadBoolean()) return;

            // Get Autonomous Round Data
            teamData.AutoData = new object[dataSet.RoundAutoData.Count];
            for (int i = 0; i < dataSet.RoundAutoData.Count; ++i)
            {
                var point = dataSet.RoundAutoData[i];
                teamData.AutoData[i] = reader.ReadByType(point.DataType);
            }

            // Get Tele-OP Round Data
            teamData.TeleOPData = new object[dataSet.RoundTeleOPData.Count];
            for (int i = 0; i < dataSet.RoundTeleOPData.Count; ++i)
            {
                var point = dataSet.RoundTeleOPData[i];
                teamData.TeleOPData[i] = reader.ReadByType(point.DataType);
            }

            // Update the Scout Master UI
            UpdateScoutMasterUI();
            currentAction = Actions.None;
        }

        protected void UpdateScoutMasterUI()
        {
            if (ScoutMasterActivity.Instance != null)
            {
                ScoutMasterActivity.Instance.RunOnUiThread(new Action(() =>
                {
                    --ScoutMasterActivity.WaitingOnScouts;
                    ScoutMasterActivity.Instance.UpdateUI();
                }));
            }
        }
    }
}