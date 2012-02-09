using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using DevIL;

namespace DevILNet.Sample {
    public partial class Form1 : Form {
        private static String s_image = "DevIL.jpg";
        private bool alienified = false;

        public Form1() {
            InitializeComponent();
            pictureBox1.Image = (System.Drawing.Image) LoadImage(false);
            Text = "Hello you DevIL!";
            this.CenterToScreen();
        }

        private Bitmap LoadImage(bool alienify) {
            String filePath = GetFullPath(s_image);
            if(!File.Exists(filePath)) {
                throw new FileNotFoundException(String.Format("{0} is missing from the root application folder.", filePath));
            }
            //Always be sure to init IL, ILU, etc. Repeat calls to Init get filtered out.
            IL.Init();

            if(alienify) {
                ILU.Init();
            }

            //Force DevIL to convert images upon load to a
            //predetermined format and data type.
            IL.Enable(ILEnable.AbsoluteFormat);
            IL.SetDataFormat(DataFormat.BGRA);
            IL.Enable(ILEnable.AbsoluteType);
            IL.SetDataType(DataType.UnsignedByte);

            //Generate + Bind an image to work with
            uint id = IL.GenImage();
            IL.BindImage(id);

            FileStream fs = null;
            try {
                fs = File.OpenRead(filePath);

                //Load the image from the stream
                if(IL.LoadFromStream(fs)) {
                    return LoadIntoBitmap(id, alienify);
                } else {
                    throw new IOException("Failed to load image");
                }
            } finally {
                if(fs != null) {
                    fs.Dispose();
                }

                //Delete image, and shutdown IL
                IL.DeleteImage(id);
                IL.ShutDown();
            }
        }

        private Bitmap LoadIntoBitmap(uint id, bool alienify) {
            if(alienify) {
                ILU.Alienify();
            }

            //We'll be reading the image into a Bitmap

            //ImagInfo represents all the relevant information about the currently bound image (or subimage, mipmap, etc).
            //Some properties can be queried from the GetInteger methods, but not all (mostly the enums).
            ImageInfo info = IL.GetImageInfo();
            Bitmap bitmap = new Bitmap(info.Width, info.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, info.Width, info.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //Since Scan0 is an IntPtr to the bitmap data and we're all the same size...just do a copy.
            uint what = IL.CopyPixels(0, 0, 0, (uint)info.Width, (uint)info.Height, 1, DataFormat.BGRA, DataType.UnsignedByte, data.Scan0);
            
            bitmap.UnlockBits(data);

            return bitmap;
        }

        private String GetFullPath(String file) {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), file);
        }

        private void button1_Click(object sender, EventArgs e) {
            alienified = !alienified;
            pictureBox1.Image = (System.Drawing.Image) LoadImage(alienified);
        }
    }
}
