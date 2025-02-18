using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlappingModel
{
    internal class ImageAnalysisResult(Pattern[] patterns, int[] frequencies, CoefficienceSet topPatterns, CoefficienceSet leftPatterns,
        CoefficienceSet rightPatterns, CoefficienceSet bottomPatterns)
    {
        public Pattern[] Patterns { get; private set; } = patterns;
        public int[] Frequencies { get; private set; } = frequencies;
        public CoefficienceSet TopPatterns { get; private set; } = topPatterns;
        public CoefficienceSet LeftPatterns { get; private set; } = leftPatterns;
        public CoefficienceSet RightPatterns { get; private set; } = rightPatterns;
        public CoefficienceSet BottomPatterns { get; private set; } = bottomPatterns;
    }
}
