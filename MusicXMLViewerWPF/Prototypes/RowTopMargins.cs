using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicXMLScore.Prototypes
{
    class RowTopMargins : IList<RowTopMargin>, ICollection<RowTopMargin>, IEnumerable<RowTopMargin>, ICollection, IEnumerable, IList
    {
        readonly ObservableCollection<RowTopMargin> margins = new ObservableCollection<RowTopMargin>();
        readonly FrameworkElement owner;

        public RowTopMargins(FrameworkElement owner)
        {
            this.owner = owner;
            margins.CollectionChanged += Margins_CollectionChanged;
        }

        private void Margins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            owner.InvalidateArrange();
        }

        public int Count => margins.Count;

        public bool IsFixedSize => ((IList)margins).IsFixedSize;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => (object)this;

        public FrameworkElement Owner => owner;

        public RowTopMargin this[int index] { get => margins[index]; set => margins[index] = value; }
        object IList.this[int index] { get => ((IList)margins)[index]; set => ((IList)margins)[index] = value; }

        public void Add(RowTopMargin item)
        {
            margins.Add(item);
            item.Parent = owner;
        }

        public int Add(object value)
        {
            var item = (RowTopMargin)value;
            item.Parent = owner;
            return ((IList)margins).Add(value);
        }

        public void Clear()
        {
            margins.Clear();
        }

        public bool Contains(RowTopMargin item)
        {
            return margins.Contains(item);
        }

        public bool Contains(object value)
        {
            return ((IList)margins).Contains(value);
        }

        public void CopyTo(RowTopMargin[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RowTopMargin> GetEnumerator()
        {
            return margins.GetEnumerator();
        }

        public int IndexOf(RowTopMargin item)
        {
            return margins.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            return ((IList)margins).IndexOf(value);
        }

        public void Insert(int index, RowTopMargin item)
        {
            margins.Insert(index, item);
        }

        public void Insert(int index, object value)
        {
            ((IList)margins).Insert(index, value);
        }

        public bool Remove(RowTopMargin item)
        {
            return margins.Remove(item);
        }

        public void Remove(object value)
        {
            ((IList)margins).Remove(value);
        }

        public void RemoveAt(int index)
        {
            margins.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
