/*
* Copyright (c) 2012 Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace DevIL {
    public static class IL {

        private const String DevILDLL = "DevIL.dll";
        private static bool _init = false;

        #region Init

        [DllImport(DevILDLL, EntryPoint = "ilInit", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilInit();

        public static void Init() {
            if(!_init) {
                ilInit();
                _init = true;
            }
        }

        [DllImport(DevILDLL, EntryPoint = "ilShutDown", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilShutDown();

        public static void ShutDown() {
            if(_init) {
                ilShutDown();
                _init = false;
            }
        }

        #endregion

        #region GenImages

        [DllImport(DevILDLL, EntryPoint = "ilGenImage", CallingConvention = CallingConvention.StdCall)]
        public static extern uint GenImage();

        [DllImport(DevILDLL, EntryPoint = "ilGenImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilGenImages(UIntPtr num, uint[] images);

        public static void GenImages(uint[] images) {
            ilGenImages(new UIntPtr((uint)images.Length), images);
        }

        #endregion

        #region BindImage

        [DllImport(DevILDLL, EntryPoint = "ilBindImage", CallingConvention = CallingConvention.StdCall)]
        public static extern void BindImage(uint image);

        #endregion

        #region DeleteImages

        [DllImport(DevILDLL, EntryPoint = "ilDeleteImage", CallingConvention = CallingConvention.StdCall)]
        public static extern void DeleteImage(uint image);

        [DllImport(DevILDLL, EntryPoint = "ilDeleteImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilDeleteImages(UIntPtr num, uint[] images);

        public static void DeleteImages(uint[] images) {
            ilDeleteImages(new UIntPtr((uint) images.Length), images);
        }

        #endregion

        #region Loading

        [DllImport(DevILDLL, EntryPoint = "ilLoadImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool LoadImage([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String fileName);

        [DllImport(DevILDLL, EntryPoint = "ilLoadL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilLoadL(uint type, IntPtr lump, uint size);

        public static bool LoadFromStream(ImageType imageExtension, Stream stream) {
            byte[] buffer = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint) buffer.Length;
            bool flag = false;
            unsafe {
                fixed(byte* ptr = buffer) {
                    flag = ilLoadL((uint) imageExtension, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        public static bool LoadFromStream(Stream stream) {
            byte[] buffer = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint) buffer.Length;
            bool flag = false;
            ImageType imageExtension = DetermineTypeFromLump(buffer);
            unsafe {
                fixed(byte* ptr = buffer) {
                    flag = ilLoadL((uint) imageExtension, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        #endregion

        #region Enable/Disable

        [DllImport(DevILDLL, EntryPoint = "ilEnable", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilEnable(uint mode);

        public static void Enable(ILEnable mode) {
            ilEnable((uint) mode);
        }

        [DllImport(DevILDLL, EntryPoint = "ilIsEnabled", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsEnabled(uint mode);

        public static bool IsEnabled(ILEnable mode) {
            return ilIsEnabled((uint) mode);
        }

        [DllImport(DevILDLL, EntryPoint = "ilDisable", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilDisable(uint mode);

        public static void Disable(ILEnable mode) {
            ilDisable((uint) mode);
        }

        [DllImport(DevILDLL, EntryPoint = "ilIsDisabled", CallingConvention = CallingConvention.StdCall)]
        private static extern bool ilIsDisabled(uint mode);

        public static bool IsDisabled(ILEnable mode) {
            return ilIsDisabled((uint) mode);
        }

        #endregion

        #region Get/Set Values

        //TODO: Leave public until we get everything stable
        [DllImport(DevILDLL, EntryPoint = "ilGetInteger", CallingConvention = CallingConvention.StdCall)]
        public static extern int ilGetInteger(uint mode);

        public static int GetInteger(ILIntegerMode mode) {
            return ilGetInteger((uint) mode);
        }

        //TODO: Leave public until we get everything stable
        [DllImport(DevILDLL, EntryPoint = "ilGetIntegerv", CallingConvention = CallingConvention.StdCall)]
        public static extern void ilGetInteger(uint mode, ref int value);

        public static void GetInteger(ILIntegerMode mode, ref int value) {
            ilGetInteger((uint) mode, ref value);
        }

        //TODO: Leave public until we get everything stable
        [DllImport(DevILDLL, EntryPoint = "ilSetInteger", CallingConvention = CallingConvention.StdCall)]
        public static extern void ilSetInteger(uint mode, int value);

        public static void SetInteger(ILIntegerMode mode, int value) {
            ilSetInteger((uint) mode, value);
        }

        public static void SetBoolean(ILBooleanMode mode, bool value) {
            ilSetInteger((uint) mode, (value) ? 1 : 0);
        }

        public static bool GetBoolean(ILBooleanMode mode) {
            return (ilGetInteger((uint) mode) == 1) ? true : false;
        }

        public static void SetDataFormat(DataFormat format) {
            ilSetInteger(ILDefines.IL_FORMAT_MODE, (int) format);
        }

        public static DataFormat GetDataFormat() {
            return (DataFormat) ilGetInteger(ILDefines.IL_FORMAT_MODE);
        }

        public static void SetOriginLocation(OriginLocation origin) {
            ilSetInteger(ILDefines.IL_ORIGIN_MODE, (int) origin);
        }

        public static OriginLocation GetOriginLocation() {
            return (OriginLocation) ilGetInteger(ILDefines.IL_ORIGIN_MODE);
        }

        public static void SetDataType(DataType type) {
            ilSetInteger(ILDefines.IL_TYPE_MODE, (int) type);
        }

        public static DataType GetDataType() {
            return (DataType) ilGetInteger(ILDefines.IL_TYPE_MODE);
        }

        [DllImport(DevILDLL, EntryPoint = "ilCompressFunc", CallingConvention = CallingConvention.StdCall)]
        public static extern void ilCompressFunction(uint mode);

        public static void SetCompression(CompressionAlgorithm compress) {
            ilCompressFunction((uint) compress);
        }

        public static void SetQuantization(Quantization mode) {
            ilSetInteger(ILDefines.IL_QUANTIZATION_MODE, (int) mode);
        }

        public static Quantization GetQuantization() {
            return (Quantization) ilGetInteger(ILDefines.IL_QUANTIZATION_MODE);
        }

        public static void SetDxtcFormat(CompressedDataFormat format) {
            ilSetInteger(ILDefines.IL_DXTC_FORMAT, (int) format);
        }

        public static CompressedDataFormat GetDxtcFormat() {
            return (CompressedDataFormat) ilGetInteger(ILDefines.IL_DXTC_FORMAT);
        }

        public static void SetJpgSaveFormat(JpgSaveFormat saveFormat) {
            ilSetInteger(ILDefines.IL_JPG_SAVE_FORMAT, (int) saveFormat);
        }

        public static JpgSaveFormat GetJpgSaveFormat() {
            return (JpgSaveFormat) ilGetInteger(ILDefines.IL_JPG_SAVE_FORMAT);
        }

        public static ImageInfo GetImageInfo() {
            ImageInfo info = new ImageInfo();
            info.Format = (DataFormat) ilGetInteger(ILDefines.IL_IMAGE_FORMAT);
            info.DxtcFormat = (CompressedDataFormat) ilGetInteger(ILDefines.IL_DXTC_DATA_FORMAT);
            info.DataType = (DataType) ilGetInteger(ILDefines.IL_IMAGE_TYPE);
            info.PaletteType = (PaletteType) ilGetInteger(ILDefines.IL_PALETTE_TYPE);
            info.PaletteBaseType = (DataFormat) ilGetInteger(ILDefines.IL_PALETTE_BASE_TYPE);
            info.CubeFlags = (EnvironmentMapFace) ilGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS);
            info.Origin = (OriginLocation) ilGetInteger(ILDefines.IL_IMAGE_ORIGIN);
            info.Width = ilGetInteger(ILDefines.IL_IMAGE_WIDTH);
            info.Height = ilGetInteger(ILDefines.IL_IMAGE_HEIGHT);
            info.Depth = ilGetInteger(ILDefines.IL_IMAGE_DEPTH);
            info.BitsPerPixel = ilGetInteger(ILDefines.IL_IMAGE_BITS_PER_PIXEL);
            info.BytesPerPixel = ilGetInteger(ILDefines.IL_IMAGE_BYTES_PER_PIXEL);
            info.Channels = ilGetInteger(ILDefines.IL_IMAGE_CHANNELS);
            info.Duration = ilGetInteger(ILDefines.IL_IMAGE_DURATION);
            info.SizeOfData = ilGetInteger(ILDefines.IL_IMAGE_SIZE_OF_DATA);
            info.OffsetX = ilGetInteger(ILDefines.IL_IMAGE_OFFX);
            info.OffsetY = ilGetInteger(ILDefines.IL_IMAGE_OFFY);
            info.PlaneSize = ilGetInteger(ILDefines.IL_IMAGE_PLANESIZE);
            info.FaceCount = ilGetInteger(ILDefines.IL_NUM_FACES);
            info.ImageCount = ilGetInteger(ILDefines.IL_NUM_IMAGES);
            info.LayerCount = ilGetInteger(ILDefines.IL_NUM_LAYERS);
            info.MipMapCount = ilGetInteger(ILDefines.IL_NUM_MIPMAPS);
            info.PaletteBytesPerPixel = ilGetInteger(ILDefines.IL_PALETTE_BPP);
            info.PaletteColumnCount = ilGetInteger(ILDefines.IL_PALETTE_NUM_COLS);
            return info;
        }

        #endregion

        #region Pop/Push Attributes

        [DllImport(DevILDLL, EntryPoint = "ilPushAttrib", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilPushAttribute(uint bits);

        public static void PushAttribute(AttributeBits bits) {
            ilPushAttribute((uint) bits);
        }

        [DllImport(DevILDLL, EntryPoint = "ilPopAttrib", CallingConvention = CallingConvention.StdCall)]
        public static extern void PopAttribute();

        #endregion

        #region DXTC related functions

        [DllImport(DevILDLL, EntryPoint = "ilDxtcDataToImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToImage();

        [DllImport(DevILDLL, EntryPoint = "ilDxtcDataToSurface", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToSurface();

        [DllImport(DevILDLL, EntryPoint = "ilFlipSurfaceDxtcData", CallingConvention = CallingConvention.StdCall)]
        public static extern void FlipSurfaceDxtcData();

        [DllImport(DevILDLL, EntryPoint = "ilInvertSurfaceDxtcDataAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool InvertSurfaceDxtcDataAlpha();

        [DllImport(DevILDLL, EntryPoint = "ilGetDXTCData", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilGetDxtcData(IntPtr ptr, uint size, uint dxtcFormat);

        public static byte[] GetDxtcData(CompressedDataFormat dxtcFormat) {
            uint bufferSize = ilGetDxtcData(IntPtr.Zero, 0, (uint) dxtcFormat);
            if(bufferSize == 0) {
                return null;
            }
            byte[] buffer = new byte[bufferSize];

            unsafe {
                fixed(byte* ptr = buffer) {
                    ilGetDxtcData(new IntPtr(ptr), bufferSize, (uint) dxtcFormat);
                }
            }
            return buffer;
        }

        [DllImport(DevILDLL, EntryPoint = "ilImageToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilImageToDxtcData(uint dxtcFormat);

        public static bool ImageToDxtcData(CompressedDataFormat dxtcFormat) {
            return ilImageToDxtcData((uint) dxtcFormat);
        }

        [DllImport(DevILDLL, EntryPoint = "ilSurfaceToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSurfaceToDxtcData(uint dxtcFormat);

        public static bool SurfaceToDxtcData(CompressedDataFormat dxtcFormat) {
            return ilSurfaceToDxtcData((uint) dxtcFormat);
        }

        #endregion

        #region Set Active Functions

        [DllImport(DevILDLL, EntryPoint = "ilActiveFace", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ActiveFace(uint face);

        [DllImport(DevILDLL, EntryPoint = "ilActiveImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ActiveImage(uint image);

        [DllImport(DevILDLL, EntryPoint = "ilActiveLayer", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ActiveLayer(uint layer);

        [DllImport(DevILDLL, EntryPoint = "ilActiveMipmap", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ActiveMipMap(uint mipmap);

        #endregion

        #region General Image 

        /// <summary>
        /// Copies the image's pixel data from DevIL to the data pointer.
        /// </summary>
        /// <param name="offsetX">Offset in the X direction at which to start copying from.</param>
        /// <param name="offsetY">Offset in the Y direction at which to start copying from.<</param>
        /// <param name="offsetZ">Offset in the Z direction at which to start copying from.<</param>
        /// <param name="width">Number of pixels to copy in the X direction.</param>
        /// <param name="height">Number of pixels to copy in the Y direction.</param>
        /// <param name="depth">Number of pixels to copy in the Z direction.</param>
        /// <param name="dataFormat">Pixel format of the image</param>
        /// <param name="dataType">Specifies the type of data.</param>
        /// <param name="data">Pointer to copy the data into</param>
        /// <returns>Size of the data copied, in bytes.</returns>
        [DllImport(DevILDLL, EntryPoint = "ilCopyPixels", CallingConvention = CallingConvention.StdCall)]
        public static extern uint CopyPixels(uint offsetX, uint offsetY, uint offsetZ, uint width, uint height, uint depth, DataFormat dataFormat, DataType dataType, IntPtr data);

        [DllImport(DevILDLL, EntryPoint = "ilGetData", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetData();

        [DllImport(DevILDLL, EntryPoint = "ilIsImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsImage(uint image);

        [DllImport(DevILDLL, EntryPoint = "ilConvertImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilConvertImage(uint format, uint type);

        public static bool ConvertImage(DataFormat format, DataType type) {
            return ilConvertImage((uint) format, (uint) type);
        }

        [DllImport(DevILDLL, EntryPoint = "ilClearImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClearImage();

        [DllImport(DevILDLL, EntryPoint = "ilCloneCurImage", CallingConvention = CallingConvention.StdCall)]
        public static extern uint CloneImage();

        [DllImport(DevILDLL, EntryPoint = "ilClearColour", CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearColor(float red, float green, float blue, float alpha);

        [DllImport(DevILDLL, EntryPoint = "ilClampNTSC", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClampNTSC();

        [DllImport(DevILDLL, EntryPoint = "ilCopyImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool CopyImage(uint srcImage);

        [DllImport(DevILDLL, EntryPoint = "ilDefaultImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DefaultImage();

        #endregion

        #region Error

        [DllImport(DevILDLL, EntryPoint = "ilGetError", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilGetError();

        public static ErrorType GetError() {
            return (ErrorType) ilGetError();
        }

        #endregion

        #region Determine Type

        [DllImport(DevILDLL, EntryPoint = "ilDetermineType", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilDetermineType([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String fileName);

        public static ImageType DetermineType(String fileName) {
            if(!File.Exists(fileName)) {
                return ImageType.Unknown;
            }
            return (ImageType) ilDetermineType(fileName);
        }

        [DllImport(DevILDLL, EntryPoint = "ilDetermineTypeL", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilDetermineTypeL(IntPtr lump, uint size);

        public static ImageType DetermineTypeFromLump(byte[] data) {
            if(data == null || data.Length == 0) {
                return ImageType.Unknown;
            }
            uint size = (uint) data.Length;
            ImageType type = ImageType.Unknown;
            unsafe {
                fixed(byte* buffer = data) {
                    type = (ImageType) ilDetermineTypeL(new IntPtr(buffer), size);
                }
            }
            return type;
        }

        #endregion

    }
}
