using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MyScout.Android.UI
{
    [Activity(Label = "MyScout", MainLauncher = false,
        Icon = "@drawable/icon", Theme = "@android:style/Theme.Material")]
    public class ListActivity<T> : Activity
    {
        // Variables/Constants
        public ListAdapter<T> ListAdapter;
        protected RecyclerView list;
        protected Button itemAddBtn;

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

        // GUI Events
        protected override void OnCreate(Bundle bundle)
        {
            // Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ListLayout);

            // Assign local references to GUI elements
            list = FindViewById<RecyclerView>(Resource.Id.ListRecyclerView);
            itemAddBtn = FindViewById<Button>(Resource.Id.ListAddBtn);

            // Assign events to GUI elements
            itemAddBtn.Click += ItemAddBtn_Click;
        }

        protected override void OnResume()
        {
            // Refresh data in team list
            ListAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        protected virtual void ItemAddBtn_Click(object sender, EventArgs e)
        {
            // Open edit item dialog
            var intent = new Intent(this, ListAdapter.EditItemActivity);
            StartActivity(intent);
        }
    }
}