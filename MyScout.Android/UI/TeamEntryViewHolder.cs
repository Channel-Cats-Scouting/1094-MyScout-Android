using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;

namespace MyScout.Android.UI
{
    public class TeamEntryViewHolder : RecyclerView.ViewHolder
    {
        // Variables/Constants
        public TextView Label { get; protected set; }

        // Constructors
        public TeamEntryViewHolder(View itemView) : base(itemView)
        {
            // Assign local references to GUI elements
            Label = itemView.FindViewById<TextView>(Resource.Id.TeamNameLbl);
        }
    }
}