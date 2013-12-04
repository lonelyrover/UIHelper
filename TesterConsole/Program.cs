using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using libUIHelper.UIStateMachine1;

namespace TesterConsole
{
  class ConsolePrompt
  {
    public delegate void DelegateCommand();
    private Stream m_inputStream = Console.OpenStandardInput();
    private Dictionary<String, Delegate> m_commandList = new Dictionary<String, Delegate>();
    private String m_promptName;

    public ConsolePrompt()
    {
      AddCommand("quit", new DelegateCommand(CommandQuit));
      AddCommand("help", new DelegateCommand(CommandHelp));
    }

    public void AddCommand(String strCmd, DelegateCommand delegateCmd, bool overwrite=false)
    {
      if (overwrite)
        m_commandList[strCmd] = delegateCmd;
      else
        m_commandList.Add(strCmd, delegateCmd);
    }

    public void SetPromptName(String promptName)
    {
      m_promptName = promptName;
    }

    public String GetPromptName()
    {
      return m_promptName;
    }

    private String Prompt(int maxLine=256)
    {
      byte[] bytes = new byte[maxLine];
      int outputLength = m_inputStream.Read(bytes, 0, maxLine);
      char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);
      return new String(chars).Trim();
    }

    private void CommandQuit()
    {
      Console.Out.WriteLine("Good bye.");
      Environment.Exit(0);
    }

    private void CommandHelp()
    {
      foreach (KeyValuePair<string, Delegate> entry in m_commandList)
      {
        Console.Out.WriteLine(entry.Key);
      }
    }

    public void Run()
    {
      while (true)
      {
        Console.Out.Write("["+System.AppDomain.CurrentDomain.FriendlyName+"]$ ");
        String strCmd = Prompt();
        if (!m_commandList.ContainsKey(strCmd))
        {
          strCmd = "help";
        }

        m_commandList[strCmd].DynamicInvoke();
      }
    }
  }

  class MainConsole : StateContext1.Notification
  {
    private StateContext1 m_sm1 = null;

    public MainConsole()
    {
      m_sm1 = new StateContext1(this);
    }

    public void Run()
    {
      ConsolePrompt console = new ConsolePrompt();

      console.AddCommand("start", new ConsolePrompt.DelegateCommand(CommandTrigger));
      console.AddCommand("stop", new ConsolePrompt.DelegateCommand(CommandTrigger));
      console.AddCommand("quit", new ConsolePrompt.DelegateCommand(CommandTrigger), true);

      console.Run();
    }

    void CommandTrigger()
    {
      m_sm1.Transit();
    }

    void StateContext1.Notification.OnInit()
    {

    }

    void StateContext1.Notification.OnStarted()
    {

    }

    void StateContext1.Notification.OnStopped()
    {

    }

    void StateContext1.Notification.OnQuit()
    {
      Environment.Exit(0);
    }

    void StateContext1.Notification.OnProgressed(int percentage)
    {
      Console.Write("\r{0}%", percentage);
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      new MainConsole().Run();
    }
  }
}
