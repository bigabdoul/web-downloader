using System.Threading.Tasks;

namespace Downloader.Core
{
    public struct PauseToken
    {
        public static readonly PauseToken None = new PauseToken(new PauseTokenSource());

        private readonly PauseTokenSource m_source;

        internal PauseToken(PauseTokenSource source) { m_source = source; }

        public bool IsPaused { get { return m_source != null && m_source.IsPaused; } }

        public Task WaitWhilePausedAsync()
        {
            return IsPaused ?
                m_source.WaitWhilePausedAsync() :
                PauseTokenSource.s_completedTask;
        }
    }
}
