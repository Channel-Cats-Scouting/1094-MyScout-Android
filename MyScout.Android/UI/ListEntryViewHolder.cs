using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Content;
using System;
using Android.App;

namespace MyScout.Android.UI
{
    public class ListEntryViewHolder<T> : RecyclerView.ViewHolder,
        View.IOnClickListener, View.IOnLongClickListener
    {
        // Variables/Constants
        public TextView Label { get; protected set; }
        public ImageButton PopupButton { get; protected set; }

        protected ListActivity<T> activity;
        protected ListAdapter<T> adapter;
        protected PopupMenu menu;

        // Constructors
        public ListEntryViewHolder(View itemView,
            ListActivity<T> activity, ListAdapter<T> adapter) : base(itemView)
        {
            this.activity = activity;
            this.adapter = adapter;

            itemView.SetOnClickListener(this);
            itemView.SetOnLongClickListener(this);

            // Assign local references to GUI elements
            Label = itemView.FindViewById<TextView>(Resource.Id.ListEntryNameLbl);
            PopupButton = itemView.FindViewById<ImageButton>(Resource.Id.ListEntryPopupBtn);

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
            menu = new PopupMenu(activity, PopupButton);
            menu.Inflate(Resource.Menu.ListPopupMenu);
            menu.MenuItemClick += PopupMenu_ItemClick;
            menu.DismissEvent += PopupMenu_Dismiss;
            menu.Show();
        }

        // GUI Events
        public void OnClick(View view)
        {
            activity.OnItemClick(AdapterPosition);
        }

        public bool OnLongClick(View view)
        {
            // TODO: Possibly make this open the edit dialog instead?
            OpenPopupMenu();
            return true;
        }

        protected void PopupButton_Click(object sender, EventArgs e)
        {
            OpenPopupMenu();
        }

        protected void PopupMenu_ItemClick(
            object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                // Edit Item
                case Resource.Id.ListPopupMenuEditBtn:
                    var intent = new Intent(activity, adapter.EditItemActivity);
                    intent.PutExtra("ItemIndex", AdapterPosition);
                    activity.StartActivity(intent);
                    break;

                // Remove Item
                case Resource.Id.ListPopupMenuRemoveBtn:
                    adapter.Remove(this);
                    break;
            }
        }

        protected void PopupMenu_Dismiss(
            object sender, PopupMenu.DismissEventArgs e)
        {
            ClosePopupMenu();
        }
    }
}