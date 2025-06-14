using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGallery.Game
{
    public sealed class AccuracyLocator
    {
        private static AccuracyTracker accuracyTracker;

        public static void Provide(AccuracyTracker tracker)
        {
            accuracyTracker = tracker;
        }

        public static AccuracyTracker GetAccuracyTracker() => accuracyTracker;
    }
}
