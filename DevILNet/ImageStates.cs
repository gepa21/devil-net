using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIL {

    public interface IImageState {
        void Apply();
    }

    public class ImageState : IImageState {
        private bool m_useAbsoluteFormat = false;
        private bool m_useAbsoluteDataType = false;
        private bool m_useAbsoluteOrigin = false;

        private DataFormat m_absoluteFormat = DataFormat.BGRA;
        private DataType m_absoluteDataType = DataType.UnsignedByte;
        private OriginLocation m_absoluteOrigin = OriginLocation.LowerLeft;

        private bool m_convertPalette = false;
        private bool m_defaultImageOnFail = false;
        private bool m_useColorKey = false;
        private bool m_blitBlend = true;

        public bool UseAbsoluteFormat {
            get {
                return m_useAbsoluteFormat;
            }
            set {
                m_useAbsoluteFormat = value;
            }
        }

        public bool UseAbsoluteDataType {
            get {
                return m_useAbsoluteDataType;
            }
            set {
                m_useAbsoluteDataType = value;
            }
        }

        public bool UseAbsoluteOrigin {
            get {
                return m_useAbsoluteOrigin;
            }
            set {
                m_useAbsoluteOrigin = value;
            }
        }

        public DataFormat AbsoluteFormat {
            get {
                return m_absoluteFormat;
            }
            set {
                m_absoluteFormat = value;
                UseAbsoluteFormat = true;
            }
        }

        public DataType AbsoluteDataType {
            get {
                return m_absoluteDataType;
            }
            set {
                m_absoluteDataType = value;
                UseAbsoluteDataType = true;
            }
        }

        public OriginLocation AbsoluteOrigin {
            get {
                return m_absoluteOrigin;
            }
            set {
                m_absoluteOrigin = value;
                UseAbsoluteOrigin = true;
            }
        }

        public bool ConvertPalette {
            get {
                return m_convertPalette;
            }
            set {
                m_convertPalette = value;
            }
        }

        public bool DefaultImageOnFail {
            get {
                return m_defaultImageOnFail;
            }
            set {
                m_defaultImageOnFail = value;
            }
        }

        public bool UseColorKey {
            get {
                return m_useColorKey;
            }
            set {
                m_useColorKey = value;
            }
        }

        public bool BlitBlend {
            get {
                return m_blitBlend;
            }
            set {
                m_blitBlend = value;
            }
        }

        public virtual void Apply() {
            if(!IL.IsInitialized) {
                return;
            }

            if(m_useAbsoluteFormat) {
                IL.Enable(ILEnable.AbsoluteFormat);
            } else {
                IL.Disable(ILEnable.AbsoluteFormat);
            }

            IL.SetDataFormat(m_absoluteFormat);

            if(m_useAbsoluteDataType) {
                IL.Enable(ILEnable.AbsoluteType);
            } else {
                IL.Disable(ILEnable.AbsoluteType);
            }

            IL.SetDataType(m_absoluteDataType);

            if(m_useAbsoluteOrigin) {
                IL.Enable(ILEnable.AbsoluteOrigin);
            } else {
                IL.Disable(ILEnable.AbsoluteOrigin);
            }

            IL.SetOriginLocation(m_absoluteOrigin);

            if(m_convertPalette) {
                IL.Enable(ILEnable.ConvertPalette);
            } else {
                IL.Disable(ILEnable.ConvertPalette);
            }

            if(m_defaultImageOnFail) {
                IL.Enable(ILEnable.DefaultOnFail);
            } else {
                IL.Disable(ILEnable.DefaultOnFail);
            }

            if(m_useColorKey) {
                IL.Enable(ILEnable.UseColorKey);
            } else {
                IL.Disable(ILEnable.UseColorKey);
            }

            if(m_blitBlend) {
                IL.Enable(ILEnable.BlitBlend);
            } else {
                IL.Disable(ILEnable.BlitBlend);
            }
        }
    }

    public class QuantizationState : IImageState {
        private Quantization m_quantMode = Quantization.Wu;
        private int m_maxQuantIndices = 256;
        private int m_neuQuantSample = 15;

        public Quantization QuantizationMode {
            get {
                return m_quantMode;
            }
            set {
                m_quantMode = value;
            }
        }

        public int MaxQuantizationIndices {
            get {
                return m_maxQuantIndices;
            }
            set {
                m_maxQuantIndices = value;
            }
        }

        public int NeuQuantizationSamples {
            get {
                return m_neuQuantSample;
            }
            set {
                m_neuQuantSample = value;
            }
        }

        public virtual void Apply() {
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetQuantization(m_quantMode);
            IL.SetInteger(ILIntegerMode.MaxQuantizationIndices, m_maxQuantIndices);
            IL.SetInteger(ILIntegerMode.NeuQuantizationSample, m_neuQuantSample);
        }
    }

    public class CompressionState : IImageState {
        private CompressionAlgorithm m_compression = CompressionAlgorithm.ZLib;
        private CompressedDataFormat m_dxtcFormat = CompressedDataFormat.DXT1;
        private CompressedDataFormat m_vtfCompression = CompressedDataFormat.None;
        private bool m_keepDxtcData = false;

        public CompressionAlgorithm Compression {
            get {
                return m_compression;
            }
            set {
                m_compression = value;
            }
        }

        public CompressedDataFormat DxtcFormat {
            get {
                return m_dxtcFormat;
            }
            set {
                m_dxtcFormat = value;
            }
        }

        public CompressedDataFormat VtfCompression {
            get {
                return m_vtfCompression;
            }
            set {
                m_vtfCompression = value;
            }
        }

        public bool KeepDxtcData {
            get {
                return m_keepDxtcData;
            }
            set {
                m_keepDxtcData = value;
            }
        }

        public virtual void Apply() {
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetCompression(m_compression);
            IL.SetDxtcFormat(m_dxtcFormat);
            IL.SetInteger(ILIntegerMode.VTFCompression, (int)m_vtfCompression);
            IL.SetBoolean(ILBooleanMode.KeepDxtcData, m_keepDxtcData);
        }
    }

    public class SaveState : IImageState {
        private bool m_saveInterlaced = false;
        private bool m_overwriteExistingFile = false;
        private CompressionLibrary m_compressionLibrary = CompressionLibrary.Default;

        public bool SaveInterlaced {
            get {
                return m_saveInterlaced;
            }
            set {
                m_saveInterlaced = value;
            }
        }

        public bool OverwriteExistingFile {
            get {
                return m_overwriteExistingFile;
            }
            set {
                m_overwriteExistingFile = value;
            }
        }

        public CompressionLibrary CompressionLibrary {
            get {
                return m_compressionLibrary;
            }
            set {
                m_compressionLibrary = value;
            }
        }

        public virtual void Apply() {
            if(!IL.IsInitialized) {
                return;
            }

            if(m_saveInterlaced) {
                IL.Enable(ILEnable.SaveInterlaced);
            } else {
                IL.Disable(ILEnable.SaveInterlaced);
            }

            if(m_overwriteExistingFile) {
                IL.Enable(ILEnable.OverwriteExistingFile);
            } else {
                IL.Disable(ILEnable.OverwriteExistingFile);
            }

            switch(m_compressionLibrary) {
                case CompressionLibrary.Default:
                    IL.Disable(ILEnable.NvidiaCompression);
                    IL.Disable(ILEnable.SquishCompression);
                    break;
                case CompressionLibrary.Nvidia:
                    IL.Disable(ILEnable.SquishCompression);
                    IL.Enable(ILEnable.NvidiaCompression);
                    break;
                case CompressionLibrary.Squish:
                    IL.Disable(ILEnable.NvidiaCompression);
                    IL.Enable(ILEnable.SquishCompression);
                    break;
            }
        }
    }

    public class JpgSaveState : SaveState {
        private int m_jpgQuality = 99;
        private JpgSaveFormat m_jpgSaveFormat = JpgSaveFormat.Jfif;
        private bool m_jpgProgressive = false;

        public int JpgQuality {
            get {
                return m_jpgQuality;
            }
            set {
                m_jpgQuality = value;
            }
        }

        public JpgSaveFormat JpgSaveFormat {
            get {
                return m_jpgSaveFormat;
            }
            set {
                m_jpgSaveFormat = value;
            }
        }

        public bool UseJpgProgressive {
            get {
                return m_jpgProgressive;
            }
            set {
                m_jpgProgressive = value;
            }
        }

        public override void Apply() {
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetInteger(ILIntegerMode.JpgQuality, m_jpgQuality);
            IL.SetJpgSaveFormat(m_jpgSaveFormat);

            if(m_jpgProgressive) {
                IL.Enable(ILEnable.JpgProgressive);
            } else {
                IL.Disable(ILEnable.JpgProgressive);
            }
        }
    }

    public class TgaSaveState : SaveState {
        private String m_tgaId = String.Empty;
        private String m_tgaAuthName = String.Empty;
        private String m_tgaAuthComment = String.Empty;
        private bool m_useTgaRle = false;
        private bool m_tgaCreateTimeStamp = false;

        public String TgaID {
            get {
                return m_tgaId;
            }
            set {
                m_tgaId = value;
            }
        }

        public String TgaAuthorName {
            get {
                return m_tgaAuthName;
            }
            set {
                m_tgaAuthName = value;
            }
        }

        public String TgaAuthorComment {
            get {
                return m_tgaAuthComment;
            }
            set {
                m_tgaAuthComment = value;
            }
        }

        public bool UseTgaRle {
            get {
                return m_useTgaRle;
            }
            set {
                m_useTgaRle = value;
            }
        }

        public override void Apply() {
            base.Apply();
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetString(ILStringMode.TgaID, m_tgaId);
            IL.SetString(ILStringMode.TgaAuthorName, m_tgaAuthName);
            IL.SetString(ILStringMode.TgaAuthorComment, m_tgaAuthComment);
            IL.SetBoolean(ILBooleanMode.TgaRLE, m_useTgaRle);
            IL.SetBoolean(ILBooleanMode.TgaCreateStamp, m_tgaCreateTimeStamp);
        }
    }

    public class TiffSaveState : SaveState {
        private String m_tifAuthName = String.Empty;
        private String m_tifDescription = String.Empty;
        private String m_tifDocumentName = String.Empty;
        private String m_tifHostComputer = String.Empty;

        public String TifAuthorName {
            get {
                return m_tifAuthName;
            }
            set {
                m_tifAuthName = value;
            }
        }

        public String TifDescription {
            get {
                return m_tifDescription;
            }
            set {
                m_tifDescription = value;
            }
        }

        public String TifDocumentName {
            get {
                return m_tifDocumentName;
            }
            set {
                m_tifDocumentName = value;
            }
        }

        public String TifHostComputer {
            get {
                return m_tifHostComputer;
            }
            set {
                m_tifHostComputer = value;
            }
        }

        public override void Apply() {
            base.Apply();
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetString(ILStringMode.TifAuthorName, m_tifAuthName);
            IL.SetString(ILStringMode.TifDescription, m_tifDescription);
            IL.SetString(ILStringMode.TifDocumentName, m_tifDocumentName);
            IL.SetString(ILStringMode.TifHostComputer, m_tifHostComputer);
        }
    }

    public class PngSaveState : SaveState {
        private int m_pngAlphaIndex = -1;
        private String m_pngAuthName = String.Empty;
        private String m_pngTitle = String.Empty;
        private String m_pngDescription = String.Empty;
        private bool m_usePngInterlace = false;

        public int PngAlphaIndex {
            get {
                return m_pngAlphaIndex;
            }
            set {
                m_pngAlphaIndex = value;
            }
        }

        public String PngAuthorName {
            get {
                return m_pngAuthName;
            }
            set {
                m_pngAuthName = value;
            }
        }

        public String PngTitle {
            get {
                return m_pngTitle;
            }
            set {
                m_pngTitle = value;
            }
        }

        public String PngDescription {
            get {
                return m_pngDescription;
            }
            set {
                m_pngDescription = value;
            }
        }

        public bool UsePngInterlace {
            get {
                return m_usePngInterlace;
            }
            set {
                m_usePngInterlace = value;
            }
        }

        public override void Apply() {
            base.Apply();
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetInteger(ILIntegerMode.PngAlphaIndex, m_pngAlphaIndex);
            IL.SetString(ILStringMode.PngAuthorName, m_pngAuthName);
            IL.SetString(ILStringMode.PngTitle, m_pngTitle);
            IL.SetString(ILStringMode.PngDescription, m_pngDescription);
            IL.SetBoolean(ILBooleanMode.PngInterlace, m_usePngInterlace);
        }
    }

    public class BmpSaveState : SaveState {
        private bool m_useBmpRle = false;
        private int m_pcdPicNumber = 2;

        public bool UseBmpRle {
            get {
                return m_useBmpRle;
            }
            set {
                m_useBmpRle = value;
            }
        }

        public int PcdPicNumber {
            get {
                return m_pcdPicNumber;
            }
            set {
                m_pcdPicNumber = value;
            }
        }

        public override void Apply() {
            base.Apply();
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetBoolean(ILBooleanMode.BmpRLE, m_useBmpRle);
            IL.SetInteger(ILIntegerMode.PcdPicNumber, m_pcdPicNumber);
        }
    }

    public class SgiSaveState : SaveState {
        private bool m_useSgiRle = false;

        public bool UseSgiRle {
            get {
                return m_useSgiRle;
            }
            set {
                m_useSgiRle = value;
            }
        }

        public override void Apply() {
            base.Apply();
            if(!IL.IsInitialized) {
                return;
            }

            IL.SetBoolean(ILBooleanMode.SgiRLE, m_useSgiRle);
        }
    }
}
