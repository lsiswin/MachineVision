using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace MachineVision.shard.Controls
{
    public class DrawingObjectInfo
    {
        public HTuple[] HTuple {  get; set; }

        public HObject HObject { get; set; }

        public ShapeType ShapType {  get; set; }
    }
    public enum ShapeType
    {
        Rectangle1,
        Rectangle2,
        Region,
        Circle,
        Ellipse,
        Line,
        Polygon,
        Point
    }
}

