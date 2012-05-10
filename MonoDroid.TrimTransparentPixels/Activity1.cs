using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.OS;

namespace MonoDroid.TrimTransparentBounds
{
    [Activity(Label = "Trim Transparent Pixels", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        private Bitmap _bitmap;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var layout = FindViewById<LinearLayout>(Resource.Id.LinearLayout);

            layout.SetBackgroundColor(Color.Beige);

            var imageView = FindViewById<ImageView>(Resource.Id.MyImageView);

            using (var stream = Assets.Open("crop_bitmap.png"))
            {
                _bitmap = BitmapFactory.DecodeStream(stream);
                imageView.SetImageBitmap(_bitmap);
            }

            var button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                var newBitmap = TrimBitmapBounds(); RunOnUiThread(() => imageView.SetImageBitmap(newBitmap));
            };
        }

        Bitmap TrimBitmapBounds()
        {
            var height = _bitmap.Height;
            var width = _bitmap.Width;

            var pixels = new int[height * width];

            _bitmap.GetPixels(pixels, 0, width, 1, 1, width - 1, height - 1);

            var bound = new Bounds();

            for (var i = 0; i < pixels.Length; i += 4) 
            {
                var y = i / width;
                var x = i - (y * width);
                var pixel = _bitmap.GetPixel(x, y);
                if (pixel != Color.Transparent)
                {
                    if (x < bound.Left) bound.Left = x;
                    if (x > bound.Right) bound.Right = x;
                    if (y < bound.Top) bound.Top = y;
                    if (y > bound.Bottom) bound.Bottom = y;
                }
            }

            var trimHeight = bound.Bottom - bound.Top;
            var trimWidth = bound.Right - bound.Left;

            return Bitmap.CreateBitmap(_bitmap, bound.Left, bound.Top, trimWidth, trimHeight);
        }

        private class Bounds
        {
            public int Top { get; set; }
            public int Bottom { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }

            public Bounds()
            {
                Left = int.MaxValue;
                Top = int.MaxValue;
                Bottom = Right = -1;
            }

            public override string ToString()
            {
                return string.Format("Top: {0} Bottom: {1} Left: {2} Right: {3}", Top, Bottom, Left, Right);
            }
        }
    }
}

