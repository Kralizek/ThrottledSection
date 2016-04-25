using System;

namespace Kralizek.ThrottledSection
{
    public class FixedResetThrottledSection : IThrottledSection
    {
        private readonly int _spots;
        private DateTimeOffset _resetTime;
        private readonly TimeSpan _interval;
        private int _usedSpots = 0;

        public FixedResetThrottledSection(int spots, DateTimeOffset startTime, TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), "The provided interval must be greater than zero");
            }

            _spots = spots;
            _interval = interval;
            _resetTime = startTime;
        }

        private DateTimeOffset GetResetTime(DateTimeOffset resetTime)
        {
            var now = Clock.Now;

            if (resetTime > now)
                return resetTime;

            do
            {
                resetTime += _interval;

            } while (resetTime <= now);

            return resetTime;
        }

        public DateTimeOffset NextResetTime => _resetTime;

        private IClock _clock;

        public IClock Clock
        {
            get { return _clock ?? StandardClock.Default; }
            set { _clock = value; }
        }

        public bool CanEnter()
        {
            var now = Clock.Now;

            if (now <= _resetTime && _usedSpots < _spots)
            {
                return true;
            }

            if (now > _resetTime)
            {
                _resetTime = GetResetTime(_resetTime);
                _usedSpots = 0;
            }

            return _usedSpots < _spots;
        }

        public bool TryEnter()
        {
            if (!CanEnter())
                return false;

            _usedSpots++;

            return true;
        }
    }
}