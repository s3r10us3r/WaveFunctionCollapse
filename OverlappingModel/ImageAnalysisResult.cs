using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlappingModel
{
    internal class ImageAnalysisResult
    {
        public Pattern[] Patterns { get; private set; }
        public int[] Frequencies { get; private set; }
        public CoefficienceSet TopPatterns { get; private set; }
        public CoefficienceSet LeftPatterns { get; private set; }
        public CoefficienceSet RightPatterns { get; private set; }
        public CoefficienceSet BottomPatterns { get; private set; }

        public ImageAnalysisResult(Pattern[] patterns, int[] frequencies, CoefficienceSet topPatterns, CoefficienceSet leftPatterns, CoefficienceSet rightPatterns, CoefficienceSet bottomPatterns)
        {
            Patterns = patterns;
            Frequencies = frequencies;
            TopPatterns = topPatterns;
            LeftPatterns = leftPatterns;
            RightPatterns = rightPatterns;
            BottomPatterns = bottomPatterns;
        }
    }
}
