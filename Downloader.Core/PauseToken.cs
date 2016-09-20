using System.Threading.Tasks;

namespace Downloader.Core
{
    /// <summary>
    /// Represents a token for pausing code execution.
    /// </summary>
    public struct PauseToken
    {
        /// <summary>
        /// Represents the default pause token.
        /// </summary>
        public static readonly PauseToken None = new PauseToken(new PauseTokenSource());

        private readonly PauseTokenSource m_source;

        internal PauseToken(PauseTokenSource source) { m_source = source; }

        /// <summary>
        /// Gets a value that indicates whether current token source is paused.
        /// </summary>
        public bool IsPaused { get { return m_source != null && m_source.IsPaused; } }

        /// <summary>
        /// Blocks the calling thread when the current token source is actually paused.
        /// Otherwise, returns control immediately to the calling thread.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public Task WaitWhilePausedAsync()
        {
            return IsPaused ?
                m_source.WaitWhilePausedAsync() :
                PauseTokenSource.s_completedTask;
        }
    }
}
