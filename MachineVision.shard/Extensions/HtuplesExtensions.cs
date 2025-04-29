using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace MachineVision.shard.Extensions
{
    public static class HtuplesExtensions
    {
        public static HObject GenRectangle(this HTuple[] hTuples)
        {
            HObject hObject;
            HOperatorSet.GenEmptyObj(out hObject);
            if (hTuples[0].D != 0 && hTuples[1].D != 0 && hTuples[2].D != 0 && hTuples[3].D != 0)
            {
                HOperatorSet.GenRectangle1(
                    out hObject,
                    hTuples[0],
                    hTuples[1],
                    hTuples[2],
                    hTuples[3]
                );
                return hObject;
            }
            return null;
        }

        public static HObject GenCircle(this HTuple[] hTuples)
        {
            HObject hObject;
            HOperatorSet.GenEmptyObj(out hObject);
            if (hTuples[0].D != 0 && hTuples[1].D != 0 && hTuples[2].D != 0)
            {
                HOperatorSet.GenCircle(out hObject, hTuples[0], hTuples[1], hTuples[2]);
                return hObject;
            }
            return null;
        }

        public static HObject GenEllipse(this HTuple[] hTuples)
        {
            HObject hObject;
            HOperatorSet.GenEmptyObj(out hObject);
            if (
                hTuples[0].D != 0
                && hTuples[1].D != 0
                && hTuples[2].D != 0
                && hTuples[3].D != 0
                && hTuples[4].D != 0
            )
            {
                HOperatorSet.GenEllipse(
                    out hObject,
                    hTuples[0],
                    hTuples[1],
                    hTuples[2],
                    hTuples[3],
                    hTuples[4]
                );
                return hObject;
            }
            return null;
        }
    }
}
