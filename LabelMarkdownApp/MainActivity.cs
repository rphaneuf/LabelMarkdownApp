using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Core;
using System;

namespace LabelMarkdownApp
{
    [Activity(Label = "Label Markdown", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //Markdown code
            EditText upcInput = FindViewById<EditText>(Resource.Id.UPCInput);
            Button searchBtn = FindViewById<Button>(Resource.Id.SearchBtn);
            TextView invalidInputTxt = FindViewById<TextView>(Resource.Id.InvalidInputTxt);
            TextView descriptionTxt = FindViewById<TextView>(Resource.Id.DescriptionTxt);
            TextView priceTxt = FindViewById<TextView>(Resource.Id.PriceTxt);
            TextView uOMTxt = FindViewById<TextView>(Resource.Id.UOMTxt);
            EditText markdownInput = FindViewById<EditText>(Resource.Id.MarkdownInput);
            CheckBox percentChck = FindViewById<CheckBox>(Resource.Id.PercentChck);
            CheckBox dollarChck = FindViewById<CheckBox>(Resource.Id.DollarChck);
            Button generateBtn = FindViewById<Button>(Resource.Id.GenerateBtn);
            TextView newPriceTxt = FindViewById<TextView>(Resource.Id.NewPriceTxt);
            ImageView barcodeImage = FindViewById<ImageView>(Resource.Id.BarcodeImage);

            MeijerMarkdownCode myClass = new MeijerMarkdownCode();

            searchBtn.Click += (sender, e) =>
            {
                if (upcInput.Text == "UPC #")
                {
                    invalidInputTxt.Text = "Error. Improper UPC";
                }
                else
                {
                    //myClass.ExtractData(upcInput, invalidInputTxt, descriptionTxt, priceTxt, uOMTxt);
                    descriptionTxt.Text = "Description: ALOE LEAVES";
                    priceTxt.Text = "Price: 1.0";
                    uOMTxt.Text = "Unit of Measure: EA";
                }

            };

            generateBtn.Click += (sender, e) =>
            {
              
                string[] PERMISSIONS =
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE"
                };

                var permission = ContextCompat.CheckSelfPermission(this, "android.permission.WRITE_EXTERNAL_STORAGE");
                var permissionread = ContextCompat.CheckSelfPermission(this, "android.permission.READ_EXTERNAL_STORAGE");
   
                if (permission != Permission.Granted && permissionread != Permission.Granted)
                { 
                    ActivityCompat.RequestPermissions(this, PERMISSIONS, 1); 
                }
                

                try
                {
                    if (permission == Permission.Granted && permissionread == Permission.Granted)
                    {
                        string UPC = myClass.Markdown(upcInput, percentChck, markdownInput, newPriceTxt);
                        myClass.Generate(UPC, barcodeImage);
                    }
                    else
                    {
                        Console.WriteLine("No Permission");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex} ");
                }

            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}