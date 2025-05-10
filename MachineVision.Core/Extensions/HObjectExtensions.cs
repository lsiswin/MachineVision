
using HalconDotNet;
using MachineVision.Core.TemplateMatch.Shard;

namespace MachineVision.Core.Extensions
{
    public static class HObjectExtensions
    {
        public static HObject Rgb1ToGray(this HObject image)
        {
            HOperatorSet.Rgb1ToGray(image, out HObject grayImage);
            return grayImage;
        }


        public static HObject ReduceDomain(this HObject image, HObject hObject)
        {
            if (hObject == null) return image;
            HOperatorSet.ReduceDomain(image, hObject, out HObject template);
            return template;
        }
        public static HObject ReduceDomain(this HObject image, RoiPararmeter roi)
        {
            if (roi == null) return image;
            if (roi.Row1 == 0 && roi.Column1 == 0 && roi.Row2 == 0 && roi.Column2 == 0)
            {
                return image;
            }
            HOperatorSet.GenRectangle1(
                out HObject rectangle,
                roi.Row1,
                roi.Column1,
                roi.Row2,
                roi.Column2
            );
            HOperatorSet.ReduceDomain(image, rectangle, out HObject reducedImage);
            return reducedImage;
        }

        public static void SaveImage(this HObject image, string filePath)
        {
            try
            {
                HOperatorSet.WriteImage(image, "bmp", 0, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving image: {ex.Message}");
            }
        }
    }
}
