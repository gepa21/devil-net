using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DevIL {

    public class MipMapChain : List<ImageData> {

    }

    public class MipMapChainCollection : List<MipMapChain> {

    }

    public class AnimationChainCollection : List<ManagedImage> {

    }
}
