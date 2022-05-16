using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AssetsDownloadWork.Managers{
	public class TimeSlinger{
		private static List<CustomInterval> activeIntervals = new List<CustomInterval>();
		private static List<CustomTimer> activeTimers = new List<CustomTimer>();
		private static Timer updateTimer = new Timer();
		private static Random random = new Random();

		private static void TimeSlingerTick(object sender, ElapsedEventArgs e){
			foreach(CustomTimer timer in TimeSlinger.activeTimers.Where(timer => timer.timeout - (new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - timer.created) < 0).ToList()){
				try{
					timer.onFinishAction?.Invoke();
				}catch{
					TimeSlinger.activeTimers.Remove(timer);
					throw;
				}finally{
					TimeSlinger.activeTimers.Remove(timer);
				}
			}

			foreach(CustomInterval interval in TimeSlinger.activeIntervals.Where(interval => interval.timeout - (new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - interval.created) < 0).ToList()){
				try{
					interval.onExecuteAction?.Invoke();
				}catch{
					TimeSlinger.activeIntervals.Remove(interval);
					throw;
				}finally{
					interval.created = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
				}
			}
		}

		// Timers
		public static void FireTimer(Action onFinishAction, float timeout, string timerName = ""){
			TimeSlinger.activeTimers.Add(new CustomTimer(onFinishAction, timeout, timerName));
		}

		public static void KillTimer(string timerName){
			TimeSlinger.activeTimers.RemoveAll((timer) => timer.name == timerName);
		}

		public static bool TimerExists(string timerName){
			return TimeSlinger.activeTimers.Find((timer) => timer.name == timerName) != null;
		}

		public static void ForceExecuteTimer(string timerName){
			CustomTimer timer = TimeSlinger.activeTimers.Find(_timer => _timer.name == timerName);

			try{
				timer.onFinishAction?.Invoke();
			}catch{
				TimeSlinger.activeTimers.Remove(timer);
				throw;
			}finally{
				TimeSlinger.activeTimers.Remove(timer);
			}
		}

		// Intervals
		public static void FireInterval(Action onExecuteAction, float timeout, string intervalName = ""){
			TimeSlinger.activeIntervals.Add(new CustomInterval(onExecuteAction, timeout, intervalName));
		}

		public static void FireIntervalUntil(Func<bool> condition, Action onExecuteAction, Action onConditionAction, float timeout, string untilIntervalName = ""){
			if(string.IsNullOrEmpty(untilIntervalName)){
				untilIntervalName = TimeSlinger.randomID();
			}

			TimeSlinger.FireInterval(() => {
				onExecuteAction?.Invoke();

				if(!condition()){
					return;
				}

				onConditionAction?.Invoke();
				TimeSlinger.KillInterval(untilIntervalName);
			}, timeout, untilIntervalName);
		}
		
		public static void KillInterval(string intervalName){
			TimeSlinger.activeIntervals.RemoveAll((interval) => interval.name == intervalName);
		}

		public static bool IntervalExists(string intervalName){
			return TimeSlinger.activeIntervals.Find((interval) => interval.name == intervalName) != null;
		}
		
		public static void ForceExecuteInterval(string intervalName){
			CustomInterval interval = TimeSlinger.activeIntervals.Find(_interval => _interval.name == intervalName);

			try{
				interval.onExecuteAction?.Invoke();
			}catch{
				TimeSlinger.activeIntervals.Remove(interval);
				throw;
			}finally{
				interval.created = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
			}
		}

		// Logic
		public TimeSlinger(){
			TimeSlinger.updateTimer.Elapsed += TimeSlinger.TimeSlingerTick;
			TimeSlinger.updateTimer.Interval = 100;
			TimeSlinger.updateTimer.Start();
		}

		~TimeSlinger(){
			TimeSlinger.updateTimer = null;

			TimeSlinger.activeIntervals.Clear();
			TimeSlinger.activeTimers.Clear();
		}

		private static string randomID(int length = 16){
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length).Select(s => s[TimeSlinger.random.Next(s.Length)]).ToArray());
		}
	}

	public class CustomTimer{
		public Action onFinishAction;
		public float timeout;
		public string name;
		public long created;

		public CustomTimer(Action _onFinishAction, float _timeout, string _name){
			this.onFinishAction = _onFinishAction;
			this.timeout = _timeout;
			this.name = _name;

			this.created = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
		}
	}

	public class CustomInterval{
		public Action onExecuteAction;
		public float timeout;
		public string name;
		public long created;

		public CustomInterval(Action _onExecuteAction, float _timeout, string _name){
			this.onExecuteAction = _onExecuteAction;
			this.timeout = _timeout;
			this.name = _name;

			this.created = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
		}
	}
}