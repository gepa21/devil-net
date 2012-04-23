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
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace DevIL {

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageID {
        private int m_id;

        public int ID {
            get {
                return m_id;
            }
        }

        public ImageID(int id) {
            m_id = id;
        }

        public static implicit operator ImageID(int id) {
            return new ImageID(id);
        }

        public static implicit operator int(ImageID id) {
            return id.m_id;
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct ImageInfo {
        public DataFormat Format;
        public CompressedDataFormat DxtcFormat;
        public DataType DataType;
        public PaletteType PaletteType;
        public DataFormat PaletteBaseType;
        public CubeMapFace CubeFlags;
        public OriginLocation Origin;
        public int Width;
        public int Height;
        public int Depth;
        public int BitsPerPixel;
        public int BytesPerPixel;
        public int Channels;
        public int Duration;
        public int SizeOfData;
        public int OffsetX;
        public int OffsetY;
        public int PlaneSize;
        public int FaceCount;
        public int ImageCount;
        public int LayerCount;
        public int MipMapCount;
        public int PaletteBytesPerPixel;
        public int PaletteColumnCount;

        public bool HasDXTC {
            get {
                return DxtcFormat != CompressedDataFormat.None;
            }
        }

        public bool HasPalette {
            get {
                return PaletteType != DevIL.PaletteType.None;
            }
        }

        public bool IsCubeMap {
            get {
                return CubeFlags != CubeMapFace.None && CubeFlags != CubeMapFace.SphereMap;
            }
        }

        public bool IsSphereMap {
            get {
                return CubeFlags == CubeMapFace.SphereMap;
            }
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct PointF {
        float X;
        float Y;

        public PointF(float x, float y) {
            X = x;
            Y = y;
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct PointI {
        int X;
        int Y;

        public PointI(int x, int y) {
            X = x;
            Y = y;
        }
    }
}
