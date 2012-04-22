using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIL {
    public class ImageData {
        private int m_width;
        private int m_height;
        private int m_depth;
        private DataFormat m_format;
        private CompressedDataFormat m_compressedFormat;
        private EnvironmentMapFace m_face;
        private DataType m_dataType;
        private OriginLocation m_origin;
        private PaletteType m_paletteType;
        private byte[] m_data;
        private byte[] m_compressedData;
        private byte[] m_paletteData;

        public int Width {
            get {
                return m_width;
            }
        }

        public int Height {
            get {
                return m_height;
            }
        }

        public int Depth {
            get {
                return m_depth;
            }
        }

        public DataFormat Format {
            get {
                return m_format;
            }
        }

        public CompressedDataFormat CompressedFormat {
            get {
                return m_compressedFormat;
            }
        }

        public DataType DataType {
            get {
                return m_dataType;
            }
        }

        public bool HasCompressedData {
            get {
                return m_compressedFormat != CompressedDataFormat.None && m_compressedData != null;
            }
        }

        public OriginLocation Origin {
            get {
                return m_origin;
            }
        }

        public PaletteType PaletteType {
            get {
                return m_paletteType;
            }
        }

        public bool HasPaletteData {
            get {
                return m_paletteType != DevIL.PaletteType.None && m_paletteData != null;
            }
        }

        public byte[] Data {
            get {
                return m_data;
            }
        }

        public byte[] CompressedData {
            get {
                return m_compressedData;
            }
        }

        public byte[] PaletteData {
            get {
                return m_paletteData;
            }
        }

        private ImageData() { }

        internal static ImageData Load(int imageID, int imageNum, int faceNum, int layerNum, int mipMapNum) {
            if(imageID < 0)
                return null;

            IL.BindImage((uint)imageID);
            if(!IL.ActiveImage((uint) imageNum))
                return null;
            if(!IL.ActiveFace((uint) faceNum))
                return null;
            //IL.ActiveLayer((uint) layerNum); Not yet implemented
            if(!IL.ActiveMipMap((uint) mipMapNum))
                return null;
            
            ImageData imageData = new ImageData();
            ILImageInfo info = IL.GetImageInfo();
            imageData.m_width = info.Width;
            imageData.m_height = info.Height;
            imageData.m_depth = info.Depth;
            imageData.m_format = info.Format;
            imageData.m_compressedFormat = info.DxtcFormat;
            imageData.m_face = info.CubeFlags;
            imageData.m_dataType = info.DataType;
            imageData.m_origin = info.Origin;
            imageData.m_paletteType = info.PaletteType;
            IntPtr data = IL.GetData();

            if(data != IntPtr.Zero) {
                imageData.m_data = MemoryHelper.MarshalArray<byte>(data, info.SizeOfData);
            } else {
                return null; //If no data, then we hit an invalid for some reason, abort
            }

            if(imageData.HasCompressedData) {
                imageData.m_compressedData = IL.GetDxtcData(imageData.m_compressedFormat);
            }

            if(imageData.HasPaletteData) {

            }

            return imageData;
        }
    }
}
