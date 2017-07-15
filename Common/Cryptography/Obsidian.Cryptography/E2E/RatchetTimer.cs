﻿using System;
using System.Diagnostics;

namespace Obsidian.Cryptography.E2E
{
    public class RatchetTimer
    {
        long _previousSecondTicks;

        /// <summary>
        /// Generates the Dynamic Key Ids. A Hybrid
        /// of a Clock and a Counter.
        /// </summary>
        /// <remarks>Not thread safe.</remarks>
        public  long GetNextTicks(long persitedMaxKeyId)
        {
            if (persitedMaxKeyId >= _previousSecondTicks)
                _previousSecondTicks = persitedMaxKeyId;

            // The ids should be similar to timestaps, but we don't want to
            // be too exact here to prevent misuse in timing attacs. A resolution of
            // seconds should be fuzzy enough. 
            const long resolution = TimeSpan.TicksPerSecond;
            var date = DateTime.UtcNow;
            var currentTicks = date.Ticks - date.Ticks % resolution;

            // If the clock ticks forward, this should be the normal case.
            if (currentTicks > _previousSecondTicks)
            {
                _previousSecondTicks = currentTicks;
                return currentTicks;
            }
            // If our clock has gone backwards, or if persitedMaxKeyId was 'in 
            // the future' for unknwn reasons, we just increment till time catches
            // up, if that ever happens.
            // We do not simply increment persitedMaxKeyId, because if persistance
            // is broken, we'll generate the same number over and over again.
            _previousSecondTicks++; 

            Debug.Assert(_previousSecondTicks > persitedMaxKeyId);
            return _previousSecondTicks;
        }
    }
}
