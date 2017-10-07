using Android.App;
using Android.Widget;
using System;
using System.Collections.Generic;

using AppCompatTextView = Android.Support.V7.Widget.AppCompatTextView;

namespace MyScout.Android.UI
{
    [Activity(Label = "Round", Icon = "@drawable/icon",
        Theme = "@style/MyScoutTheme")]
    public class RoundActivity : ToolbarActivity
    {
        // Variables/Constants
        public static Team CurrentTeam;
        protected LinearLayout roundLayout, autoLayout, teleOPLayout;
        protected Button doneBtn;

        // GUI Events
        protected override void OnCreate()
        {
            // Setup GUI
            SetContentView(Resource.Layout.RoundLayout);

            // Assign local references to GUI elements
            roundLayout = FindViewById<LinearLayout>(Resource.Id.RoundLinearLayout);
            autoLayout = FindViewById<LinearLayout>(Resource.Id.AutoLinearLayout);
            teleOPLayout = FindViewById<LinearLayout>(Resource.Id.TeleOPLinearLayout);
            doneBtn = FindViewById<Button>(Resource.Id.RoundDoneBtn);

            // Assign events to GUI elements
            doneBtn.Click += DoneBtn_Click;

            // Set ActionBar Label
            if (CurrentTeam != null)
            {
                Title = CurrentTeam.ToString();
            }

            // Build GUI from DataSet
            if (DataSet.Current != null)
            {
                // Build Autonomous GUI
                DataSet.Current.FillAutonomousGUI(autoLayout);
                if (autoLayout.ChildCount < 1)
                {
                    roundLayout.RemoveView(autoLayout);
                }

                // Build Tele-OP GUI
                DataSet.Current.FillTeleOPGUI(teleOPLayout);
                if (teleOPLayout.ChildCount < 1)
                {
                    roundLayout.RemoveView(teleOPLayout);
                }
            }
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            var connection = (BluetoothIO.Connections[0] as ScoutConnection);
            if (connection == null) return;

            // Get round data from GUI
            var autoData = GetGUIData(DataSet.Current.RoundAutoData, autoLayout);
            var teleOPData = GetGUIData(DataSet.Current.RoundTeleOPData, teleOPLayout);

            // Send data to scout master and go to connecting screen
            connection.WriteRoundData(autoData, teleOPData);
            StartActivity(typeof(MainActivity));
            Finish();
        }

        private object[] GetGUIData(List<DataPoint> dataPoints, LinearLayout layout)
        {
            var data = new object[dataPoints.Count];
            int i = -1;

            for (int layoutIndex = 0; layoutIndex < layout.ChildCount; ++layoutIndex)
            {
                var view = layout.GetChildAt(layoutIndex);
                if (view.GetType() == typeof(TextView) ||
                    view.GetType() == typeof(AppCompatTextView))
                {
                    continue;
                }

                // Get the appropriate data based on type
                var type = dataPoints[++i].DataType;

                if (type == typeof(bool))
                {
                    // Booleans
                    var chkBx = (view as CheckBox);
                    data[i] = (chkBx == null) ? false : chkBx.Checked;
                }
                else if (type == typeof(string))
                {
                    // Strings
                    var txtBx = (view as EditText);
                    data[i] = txtBx?.Text;
                }
                else
                {
                    // Numbers
                    var txtBx = (view as EditText);
                    if (txtBx != null && !string.IsNullOrEmpty(txtBx.Text))
                    {
                        if (type == typeof(int))
                        {
                            data[i] = int.Parse(txtBx.Text);
                            continue;
                        }
                        else if (type == typeof(float))
                        {
                            data[i] = float.Parse(txtBx.Text);
                            continue;
                        }
                        else if (type == typeof(double))
                        {
                            data[i] = double.Parse(txtBx.Text);
                            continue;
                        }
                    }

                    data[i] = 0;
                }
            }

            return data;
        }
    }
}