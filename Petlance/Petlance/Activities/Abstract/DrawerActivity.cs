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

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public abstract class DrawerActivity : PetlanceActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected int menu_layout;
        protected View menu;

        public View NavHeader { get; set; }
        protected DrawerLayout DrawerLayout { get; set; }
        protected NavigationView NavigationView { get; set; }
        protected PetlanceSupportActionBarDrawerToggle Toggle { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            DrawerLayout.SetScrimColor(Color.Transparent);
            Toggle = new PetlanceSupportActionBarDrawerToggle(this, DrawerLayout, Toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            DrawerLayout.AddDrawerListener(Toggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            Toggle.SyncState();
            NavigationView = FindViewById<NavigationView>(Resource.Id.navigationView);
            NavigationView.SetNavigationItemSelectedListener(this);
            NavHeader = NavigationView.GetHeaderView(0);
        }
        

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.main:
                    ShowActivity<MainActivity>();
                    break;
                case Resource.Id.profile:
                    ShowActivity<UserActivity>();
                    break;
                case Resource.Id.favourites:
                    ShowActivity<FavoritesActivity>();
                    break;
                case Resource.Id.promote:
                    ShowActivity<VerifyActivity>();
                    break;
                case Resource.Id.orders:
                    ShowActivity<UserOrdersActivity>();
                    break;
                case Resource.Id.my_offers:
                    ShowActivity<MyOffersActivity>();
                    break;
                case Resource.Id.comments:
                    ShowActivity<CommentsActivity>();
                    break;
                case Resource.Id.logout:
                    Petlance.LogOut();
                    ShowActivity<LoginActivity>();
                    break;
                case Resource.Id.reports:
                    ShowActivity<ReportsActivity>();
                    break;
                case Resource.Id.documents:
                    ShowActivity<DocumentsActivity>();
                    break;
                default:
                    return true;
            }
            DrawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnBackPressed()
        {
            if (DrawerLayout.IsDrawerOpen(GravityCompat.Start))
                DrawerLayout.CloseDrawer(GravityCompat.Start);
            else
                base.OnBackPressed();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(menu_layout, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    DrawerLayout.CloseDrawer(menu);
                    Toggle.OnOptionsItemSelected(item);
                    return true;
                case Resource.Id.filter_button:
                    if (DrawerLayout.IsDrawerOpen(menu))
                        DrawerLayout.CloseDrawer(menu);
                    else
                    {
                        DrawerLayout.CloseDrawer(NavigationView);
                        DrawerLayout.OpenDrawer(menu);
                    }
                    return true;
                case Resource.Id.favorite_button:
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}