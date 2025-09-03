namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Determines whether a bug is closed or open.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The bug is fixed, or no more work will be performed on it.
        /// </summary>
        Closed,

        /// <summary>
        /// The bug is currently open, and work is expected to be performed on it.
        /// </summary>
        Open
    }
}