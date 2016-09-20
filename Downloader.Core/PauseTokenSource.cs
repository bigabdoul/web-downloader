using System.Threading;
using System.Threading.Tasks;

namespace Downloader.Core
{
    /// <summary>
    /// Propagates a notification that current tasks should be paused.
    /// </summary>
    public class PauseTokenSource
    {
        private TaskCompletionSource<bool> m_paused;
        internal static readonly Task s_completedTask = Task.FromResult(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseTokenSource"/> class.
        /// </summary>
        public PauseTokenSource()
        {
        }

        /// <summary>
        /// Returns a new <see cref="PauseToken"/> instance.
        /// </summary>
        public PauseToken Token { get { return new PauseToken(this); } }

        /// <summary>
        /// Gets or set a value that indicates whether the current task is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return m_paused != null; }
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(
                        ref m_paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = m_paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref m_paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Blocks the calling thread when the current token source is actually paused.
        /// Otherwise, returns control immediately to the calling thread.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance.</returns>
        internal Task WaitWhilePausedAsync()
        {
            var cur = m_paused;
            return cur != null ? cur.Task : s_completedTask;
        }
    }
}
