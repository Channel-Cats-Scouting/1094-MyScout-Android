using Android.App;
using Android.OS;
using System.Collections.Generic;
using System.IO;

namespace MyScout.Android.UI
{
    [Activity(Label = "Choose an Event", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material")]
    public class EventActivity : ListActivity<Event>
    {
        // Variables/Constants
        public static List<Event> Events = new List<Event>();

        // Methods
        public void RefreshList()
        {
            Events.Clear();

            // Load all Events
            string eventsDir = Event.EventsDirectory;
            foreach (var file in Directory.GetFiles(eventsDir,
                $"{Event.FileNamePrefix}*{Event.Extension}"))
            {
                var evnt = Event.GetEventEntry(file);
                if (evnt == null) continue;

                if (int.TryParse(Path.GetFileNameWithoutExtension(
                    file).Substring(Event.FileNamePrefix.Length), out evnt.Index))
                {
                    Events.Add(evnt);
                }
            }
        }

        public override void OnItemClick(int position)
        {
            Event.Current = Events[position];

            // Load the DataSet used by the selected event
            var dataSet = new DataSet();
            var dataSetPath = Path.Combine(
                IO.DataSetDirectory, Event.Current.DataSetFileName);

            if (File.Exists(dataSetPath))
            {
                dataSet.Load(dataSetPath);
                DataSet.Current = dataSet;
            }
            else
            {
                // TODO: Show error
                return;
            }

            // Load the selected event
            Event.Current.Load(Event.Current.Index);

            // Go to the scout master GUI
            StartActivity(typeof(ScoutMasterActivity));
        }

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Change UI elements specific to this extension of ListActivity
            itemAddBtn.Text = "Create Event";

            // Setup List
            RefreshList();
            SetupList(Events, typeof(EditEventActivity));
        }
    }
}