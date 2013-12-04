using System;
using System.ComponentModel;

namespace libUIHelper
{
  public class Worker1
  {
    public interface Notification
    {
      void Progressed(int percentage);
      void Completed();
    }

    private BackgroundWorker m_worker = new BackgroundWorker();
    private Random m_random = new Random();
    private Notification m_notiTarget = null;

    public Worker1(Notification notiTarget)
    {
      m_notiTarget = notiTarget;
      m_worker.DoWork += new DoWorkEventHandler(DoWorker);
      m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
      m_worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
      m_worker.WorkerReportsProgress = true;
      m_worker.WorkerSupportsCancellation = true;
    }

    public void Start()
    {
      m_worker.RunWorkerAsync();
    }

    public void Cancel()
    {
      m_worker.CancelAsync();
    }

    private void DoWorker(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;

      int SleepTimeMilliSec = m_random.Next(1, 3) * 1000, i = 0;
      while (SleepTimeMilliSec >= i)
      {
        i += 100;
        System.Threading.Thread.Sleep(100);

        int percentComplete = (int)((float)i / (float)SleepTimeMilliSec * 100);
        worker.ReportProgress(percentComplete);
      }
    }

    private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      m_notiTarget.Completed();
    }

    private void ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      m_notiTarget.Progressed(e.ProgressPercentage);
    }
  }

}
