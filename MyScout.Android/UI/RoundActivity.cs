using Android.App;
using Android.OS;
using Android.Widget;
using System;

namespace MyScout.Android.UI
{
    [Activity(Label = "Round", Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Material")]
    public class RoundActivity : Activity
    {
        // Variables/Constants
        public static Team CurrentTeam;
        protected LinearLayout roundLayout, autoLayout, teleOPLayout;
        protected Button doneBtn;

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
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
            // TODO: Go to connecting screen and send round data to scout master
        }
    }
}