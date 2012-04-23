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
using DevIL.Unmanaged;

namespace DevIL {
    public sealed class Image : IDisposable, IEquatable<Image> {
        private bool m_isDisposed = false;
        private ImageID m_id;

        private static Image s_default = new Image(new ImageID(0));

        internal ImageID ImageID {
            get {
                return m_id;
            }
        }

        internal bool IsValid {
            get {
                return m_id >= 0 && IL.IsInitialized; //Just in case someone tries to use it after the last importer is disposed
            }
        }

        public static Image DefaultImage {
            get {
                return s_default;
            }
        }

        internal Image(ImageID id) {
            m_id = id;
        }

        ~Image() {
            Dispose(false);
        }

        public void Bind() {
            IL.BindImage(m_id);
        }

        public ManagedImage LoadManaged() {
            if(IsValid) {
                return new ManagedImage(this);
            }
            return null;
        }

        public Image Clone() {
            ImageID newID = IL.GenerateImage();
            Image clone = new Image(newID);
            IL.BindImage(newID);
            IL.CopyImage(m_id);
            return clone;
        }

        public ManagedImage ToManaged() {
            return new ManagedImage(this);
        }

        public bool Equals(Image other) {
            if(other.ImageID == ImageID)
                return true;

            return false;
        }

        public override bool Equals(object obj) {
            Image image = obj as Image;
            if(image == null) {
                return false;
            } else {
                return image.ImageID == ImageID;
            }
        }

        public override int GetHashCode() {
            return m_id.GetHashCode();
        }

        public override string ToString() {
            return m_id.ToString();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (m_isDisposed) {
                if (m_id > 0) {
                    IL.DeleteImage(m_id);
                }
                m_isDisposed = true;
            }
        }
    }
}
