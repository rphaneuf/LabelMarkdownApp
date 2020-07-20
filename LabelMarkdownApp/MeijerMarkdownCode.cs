using Android.Graphics;
using Android.Widget;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using ZXing;
using ZXing.Common;



namespace Core
{
    enum UOM
    {
        LB = 1,
        EA = 2
    }
    class MeijerMarkdownCode
    {
        string _currentUPC;
        private int _currentCheckDigit;
        private double _currentPrice;

        private UOM CurrentUOM { get; set; }

        private string CurrentUPC
        {
            get { return _currentUPC; }
            set { _currentUPC = value; }
        }

        private double CurrentPrice
        {
            get { return _currentPrice; }
            set { _currentPrice = value; }
        }

        private int CurrentCheckDigit
        {
            get { return _currentCheckDigit; }
            set { _currentCheckDigit = value; }
        }

        private double CurrentAmount { get; set; }

        private double CurrentWeight { get; set; }

        // This method must be in a class in a platform project, even if
        // the HttpClient object is constructed in a shared project.
        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }

        public async void ExtractData(EditText upcInput, TextView invalidInputTxt, TextView descriptionTxt, TextView priceTxt, TextView uOMTxt)
        {
            // CurrentUPC = upcInput.Text.Substring(0, upcInput.Text.Length);

#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            HttpClient client = new HttpClient(insecureHandler);
#else
              HttpClient client = new HttpClient();  
#endif

            //var content = HttpContent()
            string path = String.Format("https://10.0.2.2:44344/home/GetItems?UPC=000000030640");   //+ upcInput.Text.Trim());
                                                                                                    //string path = String.Format("https://cservices.meijer.com/merchproductapi/api/v1/Gtins/Products/FALSE");
                                                                                                    // Data data = null;
                                                                                                    //HttpResponseMessage response = await client.PostAsync(path);

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(path);
            httpWReq.Method = "GET";
            httpWReq.Accept = "application/json";
            httpWReq.ContentType = "application/json";
            //httpWReq.ContentLength = 4;

            //  byte[] byte1 = Encoding.UTF8.GetBytes("[501]");
            // Stream requestbody = httpWReq.GetRequestStream();
            // requestbody.Write(byte1, 0, byte1.Length);


            var httpWResp = (HttpWebResponse)httpWReq.GetResponse();
            if (httpWResp.StatusCode == HttpStatusCode.OK)
            {
                var one = 1;
                one = 2;
            }
            else
            {
                var one = 3;
            }

            /*   if (response.IsSuccessStatusCode)
               {

                   data = await response.Content.ReadAsAsync<Data>();
                   if(data.Price == 0)
                   {
                       invalidInputTxt.Text = "Error. UPC not found";                     
                   }
                   else
                   {
                       descriptionTxt.Text = "Description " + data.Description.ToString().Trim();
                       priceTxt.Text = "Price: " + data.Price.ToString().Trim();
                       //uOMTxt = data.UnitOM = "2".ToString()?  
                       //CurrentPrice = data.Price;
                       //CurrentUOM = data.UnitOM.Trim();
                   } 
               }
               else
               {
                   invalidInputTxt.Text = "Error. Connection failed";
               } */

        }

        public string Markdown(EditText upcInput, CheckBox Percent, EditText markdownInput, TextView newPriceTxt)
        {
            //These 3 lines belong in ExractData 
            CurrentUPC = (upcInput.Text.Substring(0, 1).Equals("2")) ? upcInput.Text.Substring(0, 6) + "00000" : upcInput.Text.Substring(0, upcInput.Text.Length - 1);
            CurrentCheckDigit = Int32.Parse(upcInput.Text.Substring(11, 1));
            CurrentAmount = (upcInput.Text.Substring(0, 1).Equals("2")) ? Int32.Parse(upcInput.Text.Substring(6, 5)) / 100.00 : 0;

            //Hardcoded values for ALOE LEAVES due to API connection not working 
            CurrentPrice = 1.0;
            CurrentUOM = UOM.EA;
            CurrentWeight = 1;

            //Throw in check for improper input
            double markdownAmt = float.Parse(markdownInput.Text);
            double newPrice = (bool)Percent.Checked ? newPrice = CurrentPrice - (CurrentPrice * markdownAmt / 100) : newPrice = (CurrentPrice - markdownAmt);
            newPrice = (newPrice < 0) ? -1 : newPrice;
            CurrentPrice = newPrice;
            newPriceTxt.Text = CurrentPrice.ToString().Trim();

            string currDigits = markdownInput.Text.Replace(".", "");
            string newVal = "";
            for (int i = 0; i < 5 - currDigits.Length; i++)
            {
                newVal = newVal + "0";
            }
            currDigits += newVal;

            string newUPC = "999" + CurrentUPC + CurrentCheckDigit + currDigits + CurrentWeight.ToString("00.00").Replace(".", "") + (CurrentUOM == UOM.LB ? 4 : 1);

            return newUPC;
        }

        public void Generate(string UPC, ImageView barcodeImage)
        {
            int size = 660;
            int small_size = 264;
            BitMatrix bitmapMatrix = null;

            bitmapMatrix = new MultiFormatWriter().encode(UPC, BarcodeFormat.CODE_128, size, small_size);

            var width = bitmapMatrix.Width;
            var height = bitmapMatrix.Height;
            int[] pixelsImage = new int[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (bitmapMatrix[j, i])
                        pixelsImage[i * width + j] = (int)Convert.ToInt64(0xff000000);
                    else
                        pixelsImage[i * width + j] = (int)Convert.ToInt64(0xffffffff);

                }
            }

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            bitmap.SetPixels(pixelsImage, 0, width, 0, 0, width, height);

            var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var path = System.IO.Path.Combine(sdpath, "logeshbarcode.jpg");
            var stream = new FileStream(path, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            stream.Close();

            barcodeImage.SetImageBitmap(bitmap);
        }




    }
}