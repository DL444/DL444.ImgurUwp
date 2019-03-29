using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;

namespace DL444.ImgurUwp.App.ViewModels
{
    public abstract class IncrementalItemsSource<T> : IIncrementalSource<T>
    {
        protected List<T> items = new List<T>();
        bool hasMoreItems = true;
        int producedItems = 0;

        public IncrementalItemsSource() { }
        public IncrementalItemsSource(IEnumerable<T> items) : this()
        {
            if(items == null) { throw new ArgumentNullException(nameof(items)); }
            this.items = new List<T>(items);
        }

        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            int lowerBound = pageIndex * pageSize; // Including
            int higherBound = lowerBound + pageSize; // Excluding

            while (items.Count < higherBound && hasMoreItems)
            {
                var newItems = await GetItemsFromSourceAsync(cancellationToken);
                if(newItems == null || !newItems.Any())
                {
                    hasMoreItems = false;
                    producedItems += items.Count;
                    return items.Skip(lowerBound).Take(pageSize);
                }
                else
                {
                    foreach (var i in newItems)
                    {
                        items.Add(i);
                    }
                    producedItems += pageSize;
                }
            }

            return items.Skip(lowerBound).Take(pageSize);
        }
        protected abstract Task<IEnumerable<T>> GetItemsFromSourceAsync(CancellationToken cancellationToken);

        public bool Replace(Func<T, bool> predicate, T newItem)
        {
            var item = items.Skip(producedItems).FirstOrDefault(predicate);
            if (item != null)
            {
                item = newItem;
                return true;
            }
            else { return false; }
        }
        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if(item != null)
            {
                return items.Remove(item);
            }
            else { return false; }
        }
    }

    class IncrementalLoadingCollection<TSource, IType> : Microsoft.Toolkit.Uwp.IncrementalLoadingCollection<TSource, IType> where TSource : IIncrementalSource<IType>
    {
        public new TSource Source => base.Source;

        public IncrementalLoadingCollection(int itemsPerPage = 20, Action onStartLoading = null, Action onEndLoading = null, Action<Exception> onError = null) 
            : base(itemsPerPage, onStartLoading, onEndLoading, onError) { }
        public IncrementalLoadingCollection(TSource source, int itemsPerPage = 20, Action onStartLoading = null, Action onEndLoading = null, Action<Exception> onError = null)
            : base(source, itemsPerPage, onStartLoading, onEndLoading, onError) { }

        public bool Replace(Func<IType, bool> predicate, IType newItem)
        {
            var item = this.FirstOrDefault(predicate);
            if (item != null)
            {
                item = newItem;
                return true;
            }
            else if (Source is IncrementalItemsSource<IType> incSource)
            {
                return incSource.Replace(predicate, newItem);
            }
            else
            {
                return false;
            }
        }
        public bool Remove(Func<IType, bool> predicate)
        {
            var item = this.FirstOrDefault(predicate);
            if(item == null)
            {
                if(Source is IncrementalItemsSource<IType> incSource)
                {
                    return incSource.Remove(predicate);
                }
                else { return false; }
            }
            else
            {
                return this.Remove(item);
            }
        }
    }

    public class StaticIncrementalSource<T> : IncrementalItemsSource<T>
    {
        public StaticIncrementalSource(IEnumerable<T> items) : base(items) { }

        protected override async Task<IEnumerable<T>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
