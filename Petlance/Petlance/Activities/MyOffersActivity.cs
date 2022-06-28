using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Google.Android.Material.FloatingActionButton;

namespace Petlance
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class MyOffersActivity : OfferListActivity
    {
        protected FloatingActionButton addButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Activity_layout = Resource.Layout.activity_list_offer;
            WHEREclause = $" `is_active`=1 AND `offer`.`executor`={Petlance.User.Id} ";
            FROMclause = " `offer` LEFT JOIN `animal` ON `offer`.`id`=`animal`.`offer` ";
            base.OnCreate(savedInstanceState);
            FloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.fab_add);
            FloatingActionButton.Click += FabOnClick;
            if (Petlance.User != null && Petlance.User is Executor)
                FloatingActionButton.Visibility = ViewStates.Visible;
        }
    }
}

