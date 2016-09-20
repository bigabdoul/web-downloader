using System.Collections.Generic;

namespace Downloader.Core
{
    /// <summary>
    /// Represents a collection of download list items.
    /// </summary>
    public class DownloadList : ICollection<DownloadListItem>
    {
        Dictionary<string, DownloadListItem> _internal = new Dictionary<string, DownloadListItem>();
        List<string> _keyList = new List<string>();

        /// <summary>
        /// Initializes an instance of the <see cref="DownloadList"/> class.
        /// </summary>
        public DownloadList()
        {
        }

        /// <summary>
        /// Gets or updates a <see cref="DownloadListItem"/> in the underlying collection.
        /// </summary>
        /// <param name="index">The index of the item to return or update.</param>
        /// <returns>A reference to instance of <see cref="DownloadListItem"/>.</returns>
        public DownloadListItem this[int index]
        {
            get { return _internal[_keyList[index]]; }
            set { _internal[_keyList[index]] = value; }
        }
        
        /// <summary>
        /// Adds a new download list item.
        /// </summary>
        /// <param name="sourceUrl">The source URL of the item to download.</param>
        /// <param name="destinationFolder">The fully-qualified path of the destination folder.</param>
        /// <returns>A new instance of the <see cref="DownloadListItem"/> class.</returns>
        public DownloadListItem Add(string sourceUrl, string destinationFolder)
        {
            var item = new DownloadListItem(sourceUrl, destinationFolder);
            this.Add(item);
            return item;
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to the end of the internal dictionary.
        /// </summary>
        /// <param name="item">The download item to add.</param>
        public void Add(DownloadListItem item)
        {
            _internal.Add(item.SourceUrl, item);
            _keyList.Add(item.SourceUrl);
        }

        /// <summary>
        /// Attempts to remove the specified item from the internal dictionary.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if the collection contains <paramref name="item"/>, false if not.</returns>
        public bool Remove(DownloadListItem item)
        {
            if(_internal.Remove(item.SourceUrl))
            {
                _keyList.Remove(item.SourceUrl);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears both the underlying dictionary and the list of maintained keys.
        /// </summary>
        public void Clear()
        {
            _internal.Clear();
            _keyList.Clear();
        }

        /// <summary>
        /// Determines whether the specified item exists in the collection.
        /// </summary>
        /// <param name="item">The item to find in the collection.</param>
        /// <returns>true if <paramref name="item"/> is contained in the underlying dictionary; otherwise, false.</returns>
        public bool Contains(DownloadListItem item)
        {
            return _internal.ContainsValue(item);
        }

        /// <summary>
        /// Determines whether the specified key exists in the collection.
        /// </summary>
        /// <param name="key">The key or source URL to find in the collection.</param>
        /// <returns>true if <paramref name="key"/> is found in the collection; otherwise, false.</returns>
        public bool Contains(string key)
        {
            return _internal.ContainsKey(key);
        }

        /// <summary>
        /// Copies the values of the underlying dictionary to the specified array starting at the specified index.
        /// </summary>
        /// <param name="array">The array of <see cref="DownloadListItem"/> to which elements of the current collection are to be copied.</param>
        /// <param name="arrayIndex">The index in <paramref name="array"/> at which to start copying the elements of the current collection.</param>
        public void CopyTo(DownloadListItem[] array, int arrayIndex)
        {
            _internal.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of items contained in the current collection.
        /// </summary>
        public int Count
        {
            get { return _internal.Count; }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator for iterating through the elements of the current collection.
        /// </summary>
        /// <returns></returns>
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
