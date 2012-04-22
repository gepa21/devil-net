using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIL {
    public sealed class Image : IDisposable, IEquatable<Image> {
        private bool m_isDisposed = false;
        private uint m_id;

        private static Image s_default = new Image(0);

        internal uint ImageID {
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

        internal Image(uint id) {
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
            uint newID = IL.GenImage();
            Image clone = new Image(newID);
            IL.BindImage(newID);
            IL.CopyImage(m_id);
            return clone;
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
            return String.Format("ILImage - ID: {0}", m_id);
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
