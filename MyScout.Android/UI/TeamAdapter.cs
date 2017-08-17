﻿using Android.Content;
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
                return MainActivity.Teams.Count; // TODO: Change this
            }
        }
        protected Context teamActivityContext;

        // Constructors
        public TeamAdapter(Context context)
        {
            teamActivityContext = context;
        }

        // Methods
        public void Add(Team item)
        {
            MainActivity.Teams.Add(item); // TODO: Change this
            NotifyItemInserted(MainActivity.Teams.Count-1);
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
            MainActivity.Teams.RemoveAt(index); // TODO: Change this
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

            return new TeamEntryViewHolder(itemView, teamActivityContext);
        }

        public override void OnBindViewHolder(
            RecyclerView.ViewHolder holder, int position)
        {
            // Update the text on the TeamEntryViewHolder's TextView object to
            // match the team that should be displayed at this position.
            var tvh = (holder as TeamEntryViewHolder);
            if (tvh == null)
                return;

            var team = MainActivity.Teams[position]; // TODO: Change this
            tvh.Label.Text = (string.IsNullOrEmpty(team.ID)) ?
                team.Name : $"{team.Name} - {team.ID}";
        }
    }
}