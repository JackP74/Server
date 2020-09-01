using System;
using System.Collections;

namespace Server
{
    internal class CommandHistory
    {
        private int currentPosn;
        private string CLastCommand;
        private ArrayList commandHistory = new ArrayList();

        internal CommandHistory()
        {
        }

        internal void Add(string command)
        {
            if (command != LastCommand)
            {
                commandHistory.Add(command);
                CLastCommand = command;
                currentPosn = commandHistory.Count;
            }
        }

        internal bool DoesPreviousCommandExist()
        {
            return currentPosn > 0;
        }

        internal bool DoesNextCommandExist()
        {
            return currentPosn < commandHistory.Count - 1;
        }

        internal string GetPreviousCommand()
        {
            CLastCommand = Convert.ToString(commandHistory[System.Threading.Interlocked.Decrement(ref currentPosn)]);
            return LastCommand;
        }

        internal string GetNextCommand()
        {
            CLastCommand = Convert.ToString(commandHistory[System.Threading.Interlocked.Increment(ref currentPosn)]);
            return LastCommand;
        }

        internal string LastCommand
        {
            get
            {
                return CLastCommand;
            }
        }

        internal string[] GetCommandHistory()
        {
            return (string[])commandHistory.ToArray(typeof(string));
        }

        internal string ClearHistory()
        {
            commandHistory = new ArrayList();
            return string.Empty;
        }
    }
}
