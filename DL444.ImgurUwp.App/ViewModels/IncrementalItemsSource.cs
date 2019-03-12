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

        public IncrementalItemsSource() { }
        public IncrementalItemsSource(IEnumerable<T> items) : this()
        {
            if(items == null) { throw new ArgumentNullException(nameof(items)); }
            items = new List<T>(items);
        }

        public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            int lowerBound = pageIndex * pageSize; // Including
            int higherBound = lowerBound + pageSize; // Excluding

            while (items.Count < higherBound)
            {
                var newItems = await GetItemsFromSourceAsync(cancellationToken);
                foreach (var i in newItems)
                {
                    items.Add(i);
                }
            }

            return items.Skip(lowerBound).Take(pageSize);
        }

        protected abstract Task<IEnumerable<T>> GetItemsFromSourceAsync(CancellationToken cancellationToken);
    }
}
