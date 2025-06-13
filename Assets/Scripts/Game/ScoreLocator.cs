using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGallery.Game
{
    public sealed class ScoreLocator
    {
        private static ScoreTracker scoreTracker;

        public static void Provide(ScoreTracker tracker)
        {
            scoreTracker = tracker;
        }

        public static ScoreTracker GetScoreTracker() => scoreTracker;

    }
}
