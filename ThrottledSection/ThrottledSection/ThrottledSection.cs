using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.ThrottledSection
{
    public interface IThrottledSection
    {
        bool CanEnter();

        bool TryEnter();
    }

    public class ThrottledSection : IThrottledSection
    {
        private readonly int _spots;
        private readonly TimeSpan _interval;
        private readonly ConcurrentQueue<DateTimeOffset> _queue; 

        public ThrottledSection(int spots, TimeSpan interval)
        {
            _spots = spots;
            _interval = interval;
            _queue = new ConcurrentQueue<DateTimeOffset>();
        }

        private IClock _clock;

        public IClock Clock
        {
            get { return _clock ?? StandardClock.Default; }
            set { _clock = value; }
        }

        public bool CanEnter()
        {
            DiscardExpiredItems();
            return _queue.Count < _spots;
        }

        public bool TryEnter()
        {
            if (!CanEnter()) return false;

            var now = Clock.Now;

            _queue.Enqueue(now);

            return true;
        }

        private void DiscardExpiredItems()
        {
            var now = Clock.Now;

            while (_queue.Any())
            {
                DateTimeOffset next;

                if (_queue.TryPeek(out next))
                {
                    if (next + _interval < now)
                    {
                        DateTimeOffset discard;
                        _queue.TryDequeue(out discard);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Contract.Ensures(_queue.All(i => i + _interval >= now));
        }
    }
}
