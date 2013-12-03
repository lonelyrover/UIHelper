using System.Threading;

namespace libUIHelper
{
  namespace UIStateMachine1
  {
    public abstract class State
    {
      private AutoResetEvent m_waitEvent = new AutoResetEvent(false);

      public void Wait()
      {
        m_waitEvent.WaitOne();
      }

      protected void Signal()
      {
        m_waitEvent.Set();
      }

      abstract public void Transit();
      abstract public void TransitToFinal();
    }

    public class StateContext1
    {
      public interface Notification
      {
        void OnInit();
        void OnStarted();
        void OnStopped();
        void OnQuit();
        void OnProgressed(int percentage);
      }

      private State m_state;
      private Notification m_notiTarget;

      public StateContext1(Notification notiTarget)
      {
        m_notiTarget = notiTarget;
        SetState(new StateInitial(this));
      }

      public void SetState(State state)
      {
        m_state = state;
      }

      public void Transit()
      {
        m_state.Transit();
      }

      public void TransitToFinal()
      {
        m_state.TransitToFinal();
      }

      public class StateInitial : State
      {
        private StateContext1 m_context = null;

        public StateInitial(StateContext1 context)
        {
          m_context = context;
          m_context.m_notiTarget.OnInit();
        }

        public override void Transit()
        {
          m_context.SetState(new StateStarted(m_context));
        }

        public override void TransitToFinal()
        {
          m_context.SetState(new StateFinal(m_context));
        }
      }

      public class StateStarted : State, Worker1.Notification
      {
        private StateContext1 m_context = null;

        public StateStarted(StateContext1 context)
        {
          m_context = context;
          (new Worker1(this)).Start();
        }

        public override void Transit()
        {
          base.Wait();
          m_context.SetState(new StateStopped(m_context));
        }

        public override void TransitToFinal()
        {
          base.Wait();
          m_context.SetState(new StateFinal(m_context));
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          base.Signal();
          m_context.m_notiTarget.OnStarted();
        }
      }

      public class StateStopped : State, Worker1.Notification
      {
        private StateContext1 m_context = null;

        public StateStopped(StateContext1 context)
        {
          m_context = context;
          (new Worker1(this)).Start();
        }

        public override void Transit()
        {
          base.Wait();
          m_context.SetState(new StateFinal(m_context));
        }

        public override void TransitToFinal()
        {
          base.Wait();
          m_context.SetState(new StateFinal(m_context));
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          base.Signal();
          m_context.m_notiTarget.OnStopped();
        }
      }

      public class StateFinal : State, Worker1.Notification
      {
        private StateContext1 m_context;

        public StateFinal(StateContext1 context)
        {
          m_context = context;
          (new Worker1(this)).Start();
        }

        public override void Transit()
        {
          base.Wait();
        }

        public override void TransitToFinal()
        {
          base.Wait();
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          base.Signal();
          m_context.m_notiTarget.OnQuit();
        }
      }
    }
  }
}