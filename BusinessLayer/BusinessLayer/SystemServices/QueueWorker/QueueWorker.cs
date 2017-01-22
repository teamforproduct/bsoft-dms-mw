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
        BackgroundWorker worker = new BackgroundWorker();
        Queue<IQueueTask> workqueue = new Queue<IQueueTask>();
        AutoResetEvent workAdded = new AutoResetEvent(false);
        ILogger _logger; 

        public event Action<IQueueTask> WorkUnitAdded;
        public event Action<IQueueTask> WorkCompleted;

        public QueueWorker()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public void StopWorker()
        {
            worker.CancelAsync();
            worker.DoWork -= worker_DoWork;
        }

        public void AddToQueue(IQueueTask workUnit)
        {
            lock (workqueue)
            {
                workUnit.Status = EnumWorkStatus.Pending;
                WorkUnitAdded?.Invoke(workUnit);
                workqueue.Enqueue(workUnit);
                workAdded.Set();
            }
        }

        bool m_IsBusy = false;
        public bool IsBusy
        {
            set
            {
                m_IsBusy = value;
            }
            get
            {
                return m_IsBusy;
            }
        }

        IQueueTask workInProgress = null;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        lock (workqueue)
                        {
                            if (workqueue.Count > 0)
                            {
                                IsBusy = true;
                                workInProgress = workqueue.Dequeue();
                            }
                        }
                        if (workInProgress == null)
                        {
                            IsBusy = false;
                            workAdded.WaitOne();
                        }
                        else
                        {
                            workInProgress.Status = EnumWorkStatus.Processing;
                            workInProgress.Execute();

                            workInProgress.Status = EnumWorkStatus.Success;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(workInProgress.CurrentContext, ex, "Error in Queue worker");
                        if (workInProgress != null)
                        {
                            workInProgress.StatusDescription = ex.Message;
                        }
                    }
                    finally
                    {
                        if (workInProgress != null)
                        {
                            if (workInProgress.Status != EnumWorkStatus.Success)
                            {
                                workInProgress.Status = EnumWorkStatus.Error;
                            }
                            WorkCompleted?.Invoke(workInProgress);

                            workInProgress = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Trace.TraceError(ex.ToString());
            }
        }
    }
}