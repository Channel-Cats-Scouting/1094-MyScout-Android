using Android.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace MyScout.Android.UI
{
    public class TeamAdapter : RecyclerView.Adapter
    {
        // Variables/Constants
        public override int ItemCount
        {
            get
            {
                return DebugActivity.Teams.Count; // TODO: Change this
            }
        }
        protected Activity teamActivity;

        // Constructors
        public TeamAdapter(Activity activity)
        {
            teamActivity = activity;
        }

        // Methods
        public void Add(Team item)
        {
            DebugActivity.Teams.Add(item); // TODO: Change this
            NotifyItemInserted(DebugActivity.Teams.Count-1);
        }

        public void Remove(TeamEntryViewHolder item)
        {
            item.ClosePopupMenu(); // Close the popup menu if it happens to be open
            Remove(item.AdapterPosition);
        }

        public void Remove(int index)
        {
            // Remove the item at the given postion from the Teams list and tell
            // Android we did so (so Android removes that item from the UI list, too).
            DebugActivity.Teams.RemoveAt(index); // TODO: Change this
            NotifyItemRemoved(index);
        }

        // GUI Events
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new TeamEntryViewHolder from the UI layout
            // we made defined in TeamListEntryLayout.axml
            var itemView = LayoutInflater.From(parent.Context).Inflate(
                Resource.Layout.TeamListEntryLayout, parent, false);

            return new TeamEntryViewHolder(itemView, teamActivity);
        }

        public override void OnBindViewHolder(
            RecyclerView.ViewHolder holder, int position)
        {
            // Update the text on the TeamEntryViewHolder's TextView object to
            // match the team that should be displayed at this position.
            var tvh = (holder as TeamEntryViewHolder);
            if (tvh == null)
                return;

            var team = DebugActivity.Teams[position]; // TODO: Change this
            tvh.Label.Text = team.ToString();
        }
    }
}