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
using System.IO;
using DevIL.Unmanaged;

namespace DevIL {
    public sealed class ImageImporter : IDisposable {
        private FilterEngine m_filterEngine;
        private TransformEngine m_transformEngine;
        private bool m_isDisposed;

        private static int s_ref = 0;
        private static Object s_sync = new Object();
        private Object m_sync = new Object();

        public FilterEngine Filter {
            get {
                return m_filterEngine;
            }
        }

        public TransformEngine Transform {
            get {
                return m_transformEngine;
            }
        }

        public ImageImporter() {
            m_filterEngine = new FilterEngine();
            m_transformEngine = new TransformEngine();
            m_isDisposed = false;
            AddRef();
        }

        ~ImageImporter() {
            Dispose(false);
        }

        public Image Load(String filename) {
            lock(m_sync) {
                CheckDisposed();

                ImageID id = GenImage();

                if(IL.LoadImage(filename)) {
                    return new Image(id);
                } else {
                    throw new IOException(String.Format("Failed to loade image: {0}", IL.GetError()));
                }
            }
        }

        public ManagedImage LoadManaged(String filename) {
            lock(m_sync) {
                Image image = Load(filename);
                if(image.IsValid) {
                    ManagedImage managedImage = new ManagedImage(image);
                    image.Dispose();
                    return managedImage;
                }
                return null;
            }
        }

        public Image Load(Stream stream) {
            lock(m_sync) {
                CheckDisposed();

                ImageID id = GenImage();

                if(IL.LoadImageFromStream(stream)) {
                    return new Image(id);
                } else {
                    throw new IOException(String.Format("Failed to loade image: {0}", IL.GetError()));
                }
            }
        }

        public ManagedImage LoadManaged(Stream stream) {
            lock(m_sync) {
                Image image = Load(stream);
                if(image.IsValid) {
                    ManagedImage managedImage = new ManagedImage(image);
                    image.Dispose();
                    return managedImage;
                }
                return null;
            }
        }

        public Image Load(Stream stream, ImageType imageType) {
            lock(m_sync) {
                CheckDisposed();

                ImageID id = GenImage();

                if(IL.LoadImageFromStream(imageType, stream)) {
                    return new Image(id);
                } else {
                    throw new IOException(String.Format("Failed to loade image: {0}", IL.GetError()));
                }
            }
        }

        public ManagedImage LoadManaged(Stream stream, ImageType imageType) {
            lock(m_sync) {
                Image image = Load(stream, imageType);
                if(image.IsValid) {
                    ManagedImage managedImage = new ManagedImage(image);
                    image.Dispose();
                    return managedImage;
                }
                return null;
            }
        }

        private void CheckDisposed() {
            if(m_isDisposed) {
                throw new ObjectDisposedException("Importer has been disposed.");
            }
        }

        private ImageID GenImage() {
            ImageID id = IL.GenerateImage();
            IL.BindImage(id);
            return id;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(!m_isDisposed) {

                Release();

                m_isDisposed = true;
            }
        }

        private static void AddRef() {
            lock(s_sync) {
                if(s_ref == 0) {
                    IL.Initialize();
                    ILU.Initialize();
                }
                s_ref++;
            }
        }

        private static void Release() {
            lock(s_sync) {
                s_ref--;

                if(s_ref == 0) {
                    IL.Shutdown();
                }
            }
        }
    }
}
