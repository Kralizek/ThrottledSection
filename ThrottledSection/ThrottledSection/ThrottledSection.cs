using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.ThrottledSection
{
	public class ThrottledSection
	{
		private readonly int _totalSpots;
		private readonly TimeSpan _interval;
		private readonly DateTimeOffset[] _spots;
		private int _currentPosition = 0;
		private int _availableSpots = 0;
		private object _lock = new object();
		private readonly ISubject<Spot> _subject = new Subject<Spot>();

		public struct Spot { }

		public ThrottledSection(int totalSpots, TimeSpan interval)
		{
			_totalSpots = totalSpots;
			_interval = interval;
			_spots = new DateTimeOffset[_totalSpots];
			_availableSpots = _totalSpots;
		}

		private IClock _clock;

		public IClock Clock
		{
			get { return _clock ?? StandardClock.Default; }
			set { _clock = value; }
		}

		public IObservable<Spot> SpotAvailable => _subject;

		public int AvailableSpots => _availableSpots;

		public bool IsFull => _availableSpots == 0;

		public bool HasSpots => _availableSpots > 0;

		public bool TryEnter()
		{
			if (_availableSpots == 0)
			{
				return false;
			}

			lock (_lock)
			{
				var now = Clock.Now;
				_spots[_currentPosition] = now;
				_currentPosition = Next(_currentPosition);
				_availableSpots--;

				return true;
			}

		}

		private int Next(int current)
		{
			return (current + 1) % _totalSpots;
		}

		private int Prev(int current)
		{
			var nextValue = current - 1;
			if (nextValue < 0)
				nextValue = _totalSpots - 1;

			return nextValue;
		}
	}

	/*
	public void FreeAvailableSpots()
	{
		int start = _currentPosition;
		int current = _currentPosition;
		int spotsToFree = 0;

		do
		{
			
			//var prev = Prev(current);
			
			//if (_spots[prev] == DateTimeOffset.MinValue)
			//	break;
				
			//var compareTime = _spots[prev] + _interval;

			//if ((_spots[prev] + _interval) > Clock.Now)
			//{
			//	break;
			//}

			//if ((_spots[prev] + _interval) <= Clock.Now) 
			//{
			//	_availableSpots++;
			//}
			
			//current = prev;
			

	var next = Next(current);
			
			if (_spots[next] == DateTimeOffset.MinValue)
				break;
				
			var compareTime = _spots[next] + _interval;

			if (compareTime > Clock.Now) 
			{
				break;
			}

			if (compareTime <= Clock.Now) 
			{
				spotsToFree ++;
			}
			
			current = next;
		} while (current != start);
		
		_availableSpots = Math.Min(spotsToFree, _totalSpots);
	}*/
}
