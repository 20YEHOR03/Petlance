using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Navigation;
using SupportActionBarDrawerToggle = AndroidX.AppCompat.App.ActionBarDrawerToggle;
using System;

namespace Petlance
{
    public class PetlanceSupportActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        protected DrawerLayout drawerLayout;
        protected NavigationView navigationView;
        protected DrawerActivity activity;
        public View NavHeader { get; set; }

        public PetlanceSupportActionBarDrawerToggle(DrawerActivity activity, DrawerLayout drawerLayout, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, openDrawerContentDescRes, closeDrawerContentDescRes) => Initialize(drawerLayout, activity);

        public PetlanceSupportActionBarDrawerToggle(DrawerActivity activity, DrawerLayout drawerLayout, AndroidX.AppCompat.Widget.Toolbar toolbar, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, toolbar, openDrawerContentDescRes, closeDrawerContentDescRes) => Initialize(drawerLayout, activity);

        protected PetlanceSupportActionBarDrawerToggle(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        private void Initialize(DrawerLayout drawerLayout, DrawerActivity activity)
        {
            this.activity = activity;
            this.drawerLayout = drawerLayout;
            navigationView = drawerLayout.FindViewById<NavigationView>(Resource.Id.navigationView);
            NavHeader = navigationView.GetHeaderView(0);
        }
        public override void OnDrawerOpened(View drawerView)
        {
            if(drawerView != activity.Menu && activity.Menu != null)
                drawerLayout.CloseDrawer(activity.Menu);
            UpdateHeader();
            base.OnDrawerOpened(drawerView);
        }
        public void UpdateHeader()
        {
            navigationView.Menu.Clear();
            if (Petlance.User != null)
            {
                NavHeader.FindViewById<TextView>(Resource.Id.account_name).Text = Petlance.User.Name;
                NavHeader.FindViewById<TextView>(Resource.Id.balance).Text = Petlance.User.Paws.ToString();
                NavHeader.FindViewById<ImageView>(Resource.Id.profile_pic).SetImageBitmap(Images.GetBitmapFromBytes(Petlance.User.Picture));
                if(Petlance.User is Executor)
                    navigationView.InflateMenu(Resource.Menu.main_drawer_executor);
                else
                    navigationView.InflateMenu(Resource.Menu.main_drawer);
            }
            else
            {
                NavHeader.FindViewById<LinearLayout>(Resource.Id.paws_field).Visibility = ViewStates.Gone;
                navigationView.Menu.Clear();
                if (Petlance.Admin != null)
                {
                    NavHeader.FindViewById<TextView>(Resource.Id.account_name).Text = "Admin";
                    navigationView.InflateMenu(Resource.Menu.main_drawer_admin);
                }
                else
                {
                    NavHeader.Visibility = ViewStates.Gone;
                    navigationView.InflateMenu(Resource.Menu.main_drawer_unauth);
                }
            }
        }
    }
}