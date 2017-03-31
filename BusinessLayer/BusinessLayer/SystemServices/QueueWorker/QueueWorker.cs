using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.QueueWorker
{
    public class QueueWorker
    {
        readonly BackgroundWorker _worker = new BackgroundWorker();
        readonly Queue<IQueueTask> _workqueue = new Queue<IQueueTask>();
        readonly AutoResetEvent _workAdded = new AutoResetEvent(false);
        readonly ILogger _logger; 

        public event Action<IQueueTask> WorkUnitAdded;
        public event Action<IQueueTask> WorkCompleted;

        public QueueWorker()
        {
            _worker.DoWork += worker_DoWork;
            _worker.RunWorkerAsync();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public void StopWorker()
        {
            _worker.CancelAsync();
            _worker.DoWork -= worker_DoWork;
        }

        public void AddToQueue(IQueueTask workUnit)
        {
            lock (_workqueue)
            {
                workUnit.Status = EnumWorkStatus.Pending;
                WorkUnitAdded?.Invoke(workUnit);
                _workqueue.Enqueue(workUnit);
                _workAdded.Set();
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            set
            {
                _isBusy = value;
            }
            get
            {
                return _isBusy;
            }
        }

        IQueueTask _workInProgress;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        lock (_workqueue)
                        {
                            if (_workqueue.Count > 0)
                            {
                                IsBusy = true;
                                _workInProgress = _workqueue.Dequeue();
                            }
                        }
                        if (_workInProgress == null)
                        {
                            IsBusy = false;
                            _workAdded.WaitOne();
                        }
                        else
                        {
                            _workInProgress.Status = EnumWorkStatus.Processing;
                            _workInProgress.Execute();

                            _workInProgress.Status = EnumWorkStatus.Success;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_workInProgress.CurrentContext, ex, "Error in Queue _worker");
                        if (_workInProgress != null)
                        {
                            _workInProgress.StatusDescription = ex.Message;
                        }
                    }
                    finally
                    {
                        if (_workInProgress != null)
                        {
                            if (_workInProgress.Status != EnumWorkStatus.Success)
                            {
                                _workInProgress.Status = EnumWorkStatus.Error;
                            }
                            WorkCompleted?.Invoke(_workInProgress);

                            _workInProgress = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_workInProgress?.CurrentContext != null)
                {
                    _logger.Error(_workInProgress.CurrentContext, ex, "General exception in Queue worker!");
                }
                
            }
        }
    }
}