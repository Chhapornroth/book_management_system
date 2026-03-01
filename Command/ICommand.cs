namespace WindowsFormsApp.Command
{
    /// <summary>
    /// Command Pattern:
    /// Encapsulates a request as an object, allowing for parameterization,
    /// queuing, logging, and undo operations.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns>True if execution was successful, false otherwise</returns>
        bool Execute();

        /// <summary>
        /// Undoes the command (if supported)
        /// </summary>
        /// <returns>True if undo was successful, false otherwise</returns>
        bool Undo();
    }
}

