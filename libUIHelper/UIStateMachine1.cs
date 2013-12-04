using System.Threading;

namespace libUIHelper
{
  namespace UIStateMachine1
  {
    public abstract class State
    {
      private State m_nextState = null;

      protected bool TransitPending()
      {
        return m_nextState != null;
      }

      protected bool TryTransit(State nextState)
      {
        if (null == m_nextState)
        {
          m_nextState = nextState;
          return true;
        }

        return false;
      }

      protected void FinalizeTransit()
      {
        m_nextState = null;
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
        new StateInitial(this);
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
          m_context.SetState(this);
          m_context.m_notiTarget.OnInit();
        }

        public override void Transit()
        {
          if (!TransitPending())
            TryTransit(new StateStarted(m_context));
        }

        public override void TransitToFinal()
        {
          if (!TransitPending())
            TryTransit(new StateFinal(m_context));
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
          if (!TransitPending())
            TryTransit(new StateStopped(m_context));
        }

        public override void TransitToFinal()
        {
          if (!TransitPending())
            TryTransit(new StateFinal(m_context));
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          FinalizeTransit();
          m_context.SetState(this);
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
          if (!TransitPending())
            TryTransit(new StateFinal(m_context));
        }

        public override void TransitToFinal()
        {
          if (!TransitPending())
            TryTransit(new StateFinal(m_context));
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          FinalizeTransit();
          m_context.SetState(this);
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
        }

        public override void TransitToFinal()
        {
        }

        void Worker1.Notification.Progressed(int percentage)
        {
          m_context.m_notiTarget.OnProgressed(percentage);
        }

        void Worker1.Notification.Completed()
        {
          m_context.SetState(this);
          m_context.m_notiTarget.OnQuit();
        }
      }
    }
  }
}