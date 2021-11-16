/*
 * Code is from an issue on the WindowsCommunityToolkit GitHub.
 * The issue is here: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089
 * Code link: https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/3089#issuecomment-802803251
 * 
 */

using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace VaraniumSharp.WinUI.Collections
{
	public class ObservableVector<T> : IObservableVector<T>
	{
        #region Events

        public event VectorChangedEventHandler<T> VectorChanged;

        #endregion

        #region Properties

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public bool IsVectorChangedDeferred
		{
			get { return _isVectorChangedDeferred; }
			set
			{
				if (value != _isVectorChangedDeferred)
				{
					_isVectorChangedDeferred = value;
					if (!_isVectorChangedDeferred && _numChangesPending > 0)
					{
						OnVectorChanged(CollectionChange.Reset, 0);
					}
				}
			}
		}

        public T this[int index]
		{
			get { return _list[index]; }
			set
			{
				var originalValue = _list[index];
				if (!ReferenceEquals(originalValue, value))
				{
					_list[index] = value;
					OnVectorChanged(CollectionChange.ItemChanged, index);
				}
			}
		}

        #endregion

        #region Public Methods

        public void Add(T item)
		{
			_list.Add(item);
			OnVectorChanged(CollectionChange.ItemInserted, _list.Count - 1);
		}

        public void Clear()
		{
			_list.Clear();
			OnVectorChanged(CollectionChange.Reset, 0);
		}

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
		{
			_list.Insert(index, item);
			OnVectorChanged(CollectionChange.ItemInserted, index);
		}

        public bool Remove(T item)
		{
			var index = _list.IndexOf(item);

			if (index >= 0)
			{
				RemoveAt(index);
				OnVectorChanged(CollectionChange.ItemRemoved, index);
				return true;
			}
			else
			{
				return false;
			}
		}

        public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
			OnVectorChanged(CollectionChange.ItemRemoved, index);
		}

        #endregion

        #region Private Methods

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        private void OnVectorChanged(CollectionChange change, int index)
		{
			if (!_isVectorChangedDeferred)
			{
				_numChangesPending = 0;
				VectorChanged?.Invoke(this, new VectorChangedEventArgs() { CollectionChange = change, Index = (uint)index });
			}
			else
			{
				_numChangesPending++;
			}
		}

        #endregion

        #region Variables

        private readonly List<T> _list = new List<T>();

        private bool _isVectorChangedDeferred;
        private int _numChangesPending;

        #endregion

        private class VectorChangedEventArgs : IVectorChangedEventArgs
		{
            #region Properties

            public CollectionChange CollectionChange { get; internal set; }
            public uint Index { get; internal set; }

            #endregion
        }
    }
}