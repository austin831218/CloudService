using System;
using System.Threading;
using System.Threading.Tasks;
using CloudService.Job;
using NLog;

namespace CloudService.Infrastructure
{
	public class JobScheduler
	{
		private readonly JobDescriber _describer;
		private readonly IJobQueue _queue;
		private CancellationTokenSource _cancelTokenSource;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public JobScheduler(JobDescriber describer, IJobQueue queue)
		{
			_describer = describer;
			_queue = queue;
		}

		public void Start()
		{
			_logger.Info($"scheduled job {_describer.Name} is scheduling");
			if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
			{
				_logger.Warn($"scheduled job {_describer.Name} has already been scheduling");
				return; // in-case Start called multiple times without Stop called
			}
			_cancelTokenSource = new CancellationTokenSource();
			var token = _cancelTokenSource.Token;
			var lastRun = _describer.LastScheduledTime ?? DateTime.UtcNow;
			_describer.NextTime = _describer.Schedule.GetNextOccurrence(lastRun);
			Task.Factory.StartNew((tk) =>
			{
				var ct = (CancellationToken)tk;
				while (!ct.IsCancellationRequested)
				{
					_logger.Trace("wait job {0}, last {1}, next {2}", _describer.Name,
								  _describer.LastScheduledTime.HasValue ? _describer.LastScheduledTime.ToString() : "none",
								  _describer.NextTime.ToString());
					var now = DateTime.UtcNow;
					if (now >= _describer.NextTime)
					{
						_describer.NextTime = _describer.Schedule.GetNextOccurrence(now);
						_describer.LastScheduledTime = now;
						_queue.Enqueue(_describer.Name);
					}
					Thread.Sleep(100);
				}
			}, token, token);
		}

		public void Stop()
		{
			_logger.Warn($"stop scheduling job {_describer.Name}");
			if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
			{
				_cancelTokenSource.Cancel();
			}
		}

	}
}
