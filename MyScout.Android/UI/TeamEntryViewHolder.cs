using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Content;
using System;
using Android.App;

namespace MyScout.Android.UI
{
    public class TeamEntryViewHolder : RecyclerView.ViewHolder,
        View.IOnClickListener, View.IOnLongClickListener
    {
        // Variables/Constants
        public TextView Label { get; protected set; }
        public ImageButton PopupButton { get; protected set; }
        protected PopupMenu menu;
        protected Activity teamActivity;

        // Constructors
        public TeamEntryViewHolder(View itemView, Activity activity) : base(itemView)
        {
            teamActivity = activity;
            itemView.SetOnClickListener(this);
            itemView.SetOnLongClickListener(this);

            // Assign local references to GUI elements
            Label = itemView.FindViewById<TextView>(Resource.Id.TeamNameLbl);
            PopupButton = itemView.FindViewById<ImageButton>(Resource.Id.TeamPopupBtn);

            // Assign events to GUI elements
            PopupButton.Click += PopupButton_Click;
        }

        // Methods
        public void ClosePopupMenu()
        {
            if (menu == null) return;

            menu.Dismiss();
            menu = null;
        }

        public void OpenPopupMenu()
        {
            menu = new PopupMenu(teamActivity, PopupButton);
            menu.Inflate(Resource.Menu.TeamPopupMenu);
            menu.MenuItemClick += TeamPopupMenu_ItemClick;
            menu.DismissEvent += TeamPopupMenu_Dismiss;
            menu.Show();
        }

        // GUI Events
        public void OnClick(View view)
        {
            // Return the selected team's index
            var returnedData = new Intent();
            returnedData.PutExtra("SelectedTeamIndex", AdapterPosition);
            teamActivity.SetResult(Result.Ok, returnedData);

            teamActivity.Finish();
        }

        public bool OnLongClick(View view)
        {
            // TODO: Possibly make this open the edit team dialog instead?
            OpenPopupMenu();
            return true;
        }

        protected void PopupButton_Click(object sender, EventArgs e)
        {
            OpenPopupMenu();
        }

        protected void TeamPopupMenu_ItemClick(
            object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                // Edit Team
                case Resource.Id.TeamMenuEditBtn:
                    var intent = new Intent(teamActivity, typeof(EditTeamActivity));
                    intent.PutExtra("TeamIndex", AdapterPosition);
                    teamActivity.StartActivity(intent);
                    break;

                // Remove Team
                case Resource.Id.TeamMenuRemoveBtn:
                    TeamActivity.TeamListAdapter.Remove(this);
                    break;
            }
        }

        protected void TeamPopupMenu_Dismiss(
            object sender, PopupMenu.DismissEventArgs e)
        {
            ClosePopupMenu();
        }
    }
}