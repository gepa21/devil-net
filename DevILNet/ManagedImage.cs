using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIL {
    public class ManagedImage {
        private MipMapChainCollection m_faces;
        private AnimationChainCollection m_animChain;

        //May hold a single face representing a 2D image or faces of a cubemap
        public MipMapChainCollection Faces {
            get {
                return m_faces;
            }
        }

        public AnimationChainCollection AnimationChain {
            get {
                return m_animChain;
            }
        }

        public ManagedImage(Image image) {
            m_faces = new MipMapChainCollection();
            m_animChain = new AnimationChainCollection();

            if(!image.IsValid) {
                return;
            }

            int imageID = (int) image.ImageID;
            LoadFaces(imageID, 0);
            LoadAnimationChain(imageID);
        }

        private ManagedImage(int imageID, int imageNum) {
            m_faces = new MipMapChainCollection();
            m_animChain = new AnimationChainCollection();

            LoadFaces(imageID, imageNum);
        }

        private void LoadAnimationChain(int imageID) {
            IL.BindImage((uint) imageID);

            ILImageInfo info = IL.GetImageInfo();

            for(int i = 0; i < info.ImageCount; i++) {
                ManagedImage image = new ManagedImage(imageID, i);
                if(image.Faces.Count != 0)
                    m_animChain.Add(image);
            }
        }

        private void LoadFaces(int imageID, int imageNum) {
            IL.BindImage((uint) imageID);
            if(!IL.ActiveImage((uint) imageNum))
                return;

            ILImageInfo info = IL.GetImageInfo();

            //Get the first face and every other as a mip map chain, when we hit a null, we break
            for(int i = 0; i <= info.FaceCount; i++) {
                MipMapChain mipMapChain = CreateMipMapChain(imageID, imageNum, i);
                if(mipMapChain == null)
                    break;
                m_faces.Add(mipMapChain);
            }
        }

        private MipMapChain CreateMipMapChain(int imageID, int imageNum, int faceNum) {
            IL.BindImage((uint)imageID);
            if(!IL.ActiveImage((uint) imageNum))
                return null;
            if(!IL.ActiveFace((uint) faceNum))
                return null;

            ILImageInfo info = IL.GetImageInfo();
            MipMapChain mipMapChain = new MipMapChain();

            //Get the first mipmap and every other, when we hit a null, we break
            for(int i = 0; i <= info.MipMapCount; i++) {
                ImageData data = ImageData.Load(imageID, imageNum, faceNum, 0, i);
                if(data == null)
                    break;
                mipMapChain.Add(data);
            }
            mipMapChain.TrimExcess();
            return mipMapChain;
        }
    }
}
