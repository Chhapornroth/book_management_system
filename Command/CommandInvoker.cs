using System.Collections.Generic;

namespace WindowsFormsApp.Command
{
    /// <summary>
    /// Command Pattern - Invoker:
    /// Manages command execution and maintains a history for undo operations.
    /// </summary>
    public class CommandInvoker
    {
        private readonly Stack<ICommand> _commandHistory = new();

        /// <summary>
        /// Executes a command and adds it to history
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>True if execution was successful, false otherwise</returns>
        public bool ExecuteCommand(ICommand command)
        {
            if (command == null)
                return false;

            bool result = command.Execute();
            
            if (result)
            {
                _commandHistory.Push(command);
            }

            return result;
        }

        /// <summary>
        /// Undoes the last executed command
        /// </summary>
        /// <returns>True if undo was successful, false otherwise</returns>
        public bool UndoLastCommand()
        {
            if (_commandHistory.Count == 0)
                return false;

            var lastCommand = _commandHistory.Pop();
            return lastCommand.Undo();
        }

        /// <summary>
        /// Checks if there are commands that can be undone
        /// </summary>
        public bool CanUndo => _commandHistory.Count > 0;

        /// <summary>
        /// Gets the number of commands in history
        /// </summary>
        public int HistoryCount => _commandHistory.Count;

        /// <summary>
        /// Clears the command history
        /// </summary>
        public void ClearHistory()
        {
            _commandHistory.Clear();
        }
    }
}

