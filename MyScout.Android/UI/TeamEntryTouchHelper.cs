using Android.Support.V7.Widget.Helper;
using Android.Support.V7.Widget;

namespace MyScout.Android.UI
{
    public class TeamEntryTouchHelper : ItemTouchHelper.Callback
    {
        // Variables/Constants
        public override bool IsItemViewSwipeEnabled => true;  // We're allowed to swipe away items
        public override bool IsLongPressDragEnabled => false; // We're not allowed to reorder items
        protected TeamAdapter adapter;

        // Constructors
        public TeamEntryTouchHelper(TeamAdapter teamAdapter)
        {
            adapter = teamAdapter;
        }

        // GUI Events
        public override int GetMovementFlags(
            RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            // Make it so we can swipe from left (start) to right (end) to remove items
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView,
            RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            // This list doesn't support dragging and dropping, thus
            // this method should never even be called.
            return true;
        }

        public override void OnSwiped(
            RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Get the view holder item that was swiped away as a TeamViewHolder
            var teamViewHolder = (viewHolder as TeamEntryViewHolder);
            if (teamViewHolder == null)
                return;

            // Delete the item that was swiped away
            adapter.Remove(teamViewHolder);
        }
    }
}