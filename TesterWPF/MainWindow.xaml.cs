using System.Windows;
using libUIHelper.UIStateMachine1;

namespace TesterWPF
{
  public partial class MainWindow : Window, StateContext1.Notification
  {
    private StateContext1 m_sm1 = null;
    public MainWindow()
    {
      InitializeComponent();

      m_sm1 = new StateContext1(this);
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
      m_sm1.Transit();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
      m_sm1.Transit();
    }

    private void QuitButton_Click(object sender, RoutedEventArgs e)
    {
      m_sm1.Transit();
    }

    void StateContext1.Notification.OnInit()
    {
      StartButton.IsEnabled   = true;
      StopButton.IsEnabled    = false;
      QuitButton.IsEnabled    = true;

      WorkerProgressBar.Value = 0;
    }

    void StateContext1.Notification.OnStarted()
    {
      StartButton.IsEnabled   = false;
      StopButton.IsEnabled    = true;
      QuitButton.IsEnabled    = true;

      WorkerProgressBar.Value = 0;
    }

    void StateContext1.Notification.OnStopped()
    {
      StartButton.IsEnabled   = false;
      StopButton.IsEnabled    = false;
      QuitButton.IsEnabled    = true;

      WorkerProgressBar.Value = 0;
    }

    void StateContext1.Notification.OnQuit()
    {
      Application.Current.Shutdown();
    }

    void StateContext1.Notification.OnProgressed(int percentage)
    {
      WorkerProgressBar.Value = percentage;
    }
  }
}
