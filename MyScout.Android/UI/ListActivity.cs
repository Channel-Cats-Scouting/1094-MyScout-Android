using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using System;
using System.Collections.Generic;

using SearchView = Android.Support.V7.Widget.SearchView;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@style/MyScoutTheme")]
    public class ListActivity<T> : ToolbarActivity
    {
        // Variables/Constants
        public ListAdapter<T> ListAdapter;
        protected RecyclerView list;

        // Methods
        public virtual void OnItemClick(int position)
        {
            // Return the selected item's index
            var returnedData = new Intent();
            returnedData.PutExtra("SelectedItemIndex", position);
            SetResult(Result.Ok, returnedData);

            Finish();
        }

        protected void SetupList(List<T> data, Type editItemActivity)
        {
            // Linear layout Manager
            var layoutManager = new LinearLayoutManager(this);
            list.SetLayoutManager(layoutManager);

            // List adapter
            ListAdapter = new ListAdapter<T>(this, data, editItemActivity);
            list.SetAdapter(ListAdapter);

            // Touch helper
            var callback = new ListEntryTouchHelper<T>(ListAdapter);
            var itemTouchHelper = new ItemTouchHelper(callback);
            itemTouchHelper.AttachToRecyclerView(list);
        }

        protected virtual void OnSearch(string query) { }

        // GUI Events
        protected override void OnCreate()
        {
            // Setup GUI
            SetContentView(Resource.Layout.ListLayout);

            // Assign local references to GUI elements
            list = FindViewById<RecyclerView>(Resource.Id.ListRecyclerView);
        }

        protected override void OnResume()
        {
            // Refresh data in team list
            ListAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ToolbarListMenu, menu);

            // Setup Search Bar
            var searchManager = (SearchManager)GetSystemService(SearchService);
            var searchView = menu.FindItem(
                Resource.Id.ToolbarSearchBtn).ActionView.JavaCast<SearchView>();

            searchView.SetSearchableInfo(
                searchManager.GetSearchableInfo(ComponentName));

            searchView.QueryTextChange += SearchView_QueryTextChange;

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // Add
                case Resource.Id.ToolbarAddItemBtn:
                    var intent = new Intent(this, ListAdapter.EditItemActivity);
                    StartActivity(intent);
                    return true;

                //// Search
                //case Resource.Id.ToolbarSearchBtn:
                //    // TODO
                //    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SearchView_QueryTextChange(
            object sender, SearchView.QueryTextChangeEventArgs e)
        {
            OnSearch(e.NewText);
            e.Handled = false;
        }
    }
}