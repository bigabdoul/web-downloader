using System.Collections.Generic;

namespace Downloader.Core
{
    public class DownloadList : ICollection<DownloadListItem>
    {
        Dictionary<string, DownloadListItem> _internal = new Dictionary<string, DownloadListItem>();
        List<string> _keyList = new List<string>();

        public DownloadListItem this[int index]
        {
            get { return _internal[_keyList[index]]; }
            set { _internal[_keyList[index]] = value; }
        }

        private string FindKey(int index)
        {
            return _keyList[index];
        }

        public DownloadListItem Add(string sourceUrl, string destinationFolder)
        {
            var item = new DownloadListItem(sourceUrl, destinationFolder);
            this.Add(item);
            return item;
        }

        public void Add(DownloadListItem item)
        {
            _internal.Add(item.SourceUrl, item);
            _keyList.Add(item.SourceUrl);
        }

        public bool Remove(DownloadListItem item)
        {
            if(_internal.Remove(item.SourceUrl))
            {
                _keyList.Remove(item.SourceUrl);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _internal.Clear();
            _keyList.Clear();
        }

        public bool Contains(DownloadListItem item)
        {
            return _internal.ContainsValue(item);
        }

        public bool Contains(string key)
        {
            return _internal.ContainsKey(key);
        }

        public void CopyTo(DownloadListItem[] array, int arrayIndex)
        {
            _internal.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internal.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<DownloadListItem> GetEnumerator()
        {
            var iterator = _internal.GetEnumerator();
            while (iterator.MoveNext())
            {
                yield return iterator.Current.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
