using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Petlance.AdaptiveLayout;

namespace Petlance
{
    class DocumentLayout : LinearLayout
    {
        int VerificationId { get; set; }
        Dialog Dialog { get; set; }
        Dialog DeclineDialog { get; set; }
        User User { get; set; }
        byte[] Document { get; set; }

        private const string Subject = "Verification";
        private int ImageSize = 50 * vmin;
        private int ButtonSize = 15 * vmin;
        public DocumentLayout(Context context, User user, byte[] document, int verificationId) : base(context)
        {
            User = user;
            Document = document;
            VerificationId = verificationId;
            Initialize();
        }

        protected DocumentLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer){ }

        public DocumentLayout(Context context, IAttributeSet attrs) : base(context, attrs) => Initialize();

        public DocumentLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => Initialize();

        public DocumentLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) => Initialize();

        private void Initialize()
        {
            Orientation = Orientation.Vertical;
            Android.App.AlertDialog.Builder ad = new Android.App.AlertDialog.Builder(Context);
            ad.SetMessage("Error opening file");
            ad.SetPositiveButton("Confirm", (sender, e) => { });
            Dialog = ad.Create();
            ad = new Android.App.AlertDialog.Builder(Context);
            ad.SetView(Resource.Layout.decline);
            ad.SetPositiveButton("Confirm", Decline);
            ad.SetNegativeButton("Cancel", (sender, e) => { });
            DeclineDialog = ad.Create();
            ImageView imageView = new ImageView(Context) { LayoutParameters = new LayoutParams(ImageSize, ImageSize) };
            imageView.SetImageBitmap(Images.GetBitmapFromBytes(User.Picture));
            AddView(imageView);
            TextView textView = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Text = User.Name
            };
            AddView(textView);
            LinearLayout layout = new LinearLayout(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
            ImageView acceptButton = new ImageView(Context) { LayoutParameters = new LayoutParams(ButtonSize, ButtonSize) };
            acceptButton.SetImageResource(Resource.Drawable.plus);
            acceptButton.Click += AcceptButton_ClickAsync;
            layout.AddView(acceptButton); 
            TextView button = new TextView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Text = "Open document"
            };
            button.SetBackgroundResource(Resource.Drawable.enter_button);
            button.Click += Button_Click;
            layout.AddView(button);
            ImageView declineButton = new ImageView(Context) { LayoutParameters = new LayoutParams(ButtonSize, ButtonSize) };
            declineButton.SetImageResource(Resource.Drawable.plus);
            declineButton.Click += DeclineButton_Click;
            layout.AddView(declineButton);
            AddView(layout);
        }

        private async void AcceptButton_ClickAsync(object sender, EventArgs e)
        {
            Executor executor = new Executor(User, Document, DateTime.Now);
            executor.VerifyDocument(Document);
            await Apply("Your verification request was accepted.");
        }

        private void DeclineButton_Click(object sender, EventArgs e) => DeclineDialog.Show();

        private async void Button_Click(object sender, EventArgs e)
        {
            var filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "document.pdf");
            if (File.Exists(filePath))
                File.Delete(filePath);
            byte[] bytes = (Petlance.User as Executor).Document;
            try { File.WriteAllBytes(filePath, bytes); }
            catch { Dialog.Show(); }
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
        }

        private async void Decline(object sender, EventArgs e) => await Apply("Your verification request was declined. The reasons are:" +
                                                     $"<br><b>{DeclineDialog.FindViewById<EditText>(Resource.Id.text).Text}</b>");

        private async Task Apply(string text)
        {
            RemoveAllViews();
            Visibility = Android.Views.ViewStates.Gone;
            await EmailService.SendEmail(User, Subject, text);
            using Database database = new Database();
            database.Delete(VerificationId, "verification");
            (Context as PetlanceActivity).ShowActivity<DocumentsActivity>();
            (Context as PetlanceActivity).Finish();

        }
    }
}