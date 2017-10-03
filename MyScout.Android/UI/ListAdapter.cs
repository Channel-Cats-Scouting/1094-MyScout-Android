using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using System;

namespace MyScout.Android.UI
{
    public class ListAdapter<T> : RecyclerView.Adapter
    {
        // Variables/Constants
        public Type EditItemActivity { get; protected set; }
        public override int ItemCount
        {
            get => (list == null) ?
                0 : list.Count;
        }

        protected ListActivity<T> activity;
        protected List<T> list;

        // Constructors
        public ListAdapter(ListActivity<T> activity,
            List<T> data, Type editItemActivity)
        {
            this.activity = activity;
            EditItemActivity = editItemActivity;
            list = data;
        }

        // Methods
        public void Add(T item)
        {
            list.Add(item);
            NotifyItemInserted(list.Count - 1);
        }

        public void Remove(ListEntryViewHolder<T> item)
        {
            item.ClosePopupMenu(); // Close the popup menu if it happens to be open
            Remove(item.AdapterPosition);
        }

        public void Remove(int index)
        {
            // Remove the item at the given postion from the list and tell
            // Android we did so (so Android removes that item from the UI list, too).
            list.RemoveAt(index);
            NotifyItemRemoved(index);
        }

        // GUI Events
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new ListEntryViewHolder from the UI layout
            // we made defined in ListEntryLayout.axml
            var itemView = LayoutInflater.From(parent.Context).Inflate(
                Resource.Layout.ListEntryLayout, parent, false);

            return new ListEntryViewHolder<T>(itemView, activity, this);
        }

        public override void OnBindViewHolder(
            RecyclerView.ViewHolder holder, int position)
        {
            // Update the text on the ListEntryViewHolder's TextView object to
            // match the list item that should be displayed at this position.
            var tvh = (holder as ListEntryViewHolder<T>);
            if (tvh == null)
                return;

            var item = list[position];
            tvh.Label.Text = GetTextForListItem(item);
        }

        protected virtual string GetTextForListItem(object item)
        {
            // Made a method so it can be easily overridden
            return item.ToString();
        }
    }
}