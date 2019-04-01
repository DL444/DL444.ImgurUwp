using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    /// <summary>
    /// Provides a unified framework for view model access and caching.
    /// </summary>
    class ViewModelCacheManager
    {
        private static readonly object lck = new object();
        private static ViewModelCacheManager _instance;
        public static ViewModelCacheManager Instance
        {
            get
            {
                lock(lck)
                {
                    if(_instance == null)
                    {
                        _instance = new ViewModelCacheManager();
                    }
                    return _instance;
                }
            }
        }
        public static void InitializeInstance(TimeSpan cacheTtl, int maxCacheSize)
        {
            if(maxCacheSize < 0) { throw new ArgumentOutOfRangeException($"Argument {nameof(maxCacheSize)} is not in expected range."); }
            if(cacheTtl.Ticks < 0) { throw new ArgumentOutOfRangeException($"Argument {nameof(cacheTtl)} is not in expected range."); }
            _instance = new ViewModelCacheManager(cacheTtl, maxCacheSize);
        }

        ViewModelCacheManager() : this(new TimeSpan(0, 15, 0), 10) { }
        ViewModelCacheManager(TimeSpan cacheTtl, int maxCacheSize)
        {
            CacheTTL = cacheTtl;
            MaxCacheSize = maxCacheSize;
            cacheStack = new List<CachingViewModel>(MaxCacheSize + 1);
        }

        private List<CachingViewModel> cacheStack;
        public TimeSpan CacheTTL { get; }
        public int MaxCacheSize { get; }

        public void Push(CachingViewModel vm)
        {
            if(vm == null) { throw new ArgumentNullException(nameof(vm)); }
            CleanUpStack();
            for(int i = 0; i < cacheStack.Count; i++)
            {
                if (cacheStack[i].EqualTo(vm)) { cacheStack[i] = vm; }
            }
            cacheStack.Add(vm);
        }
        public CachingViewModel Peek()
        {
            CleanUpStack();
            return cacheStack.LastOrDefault();
        }
        public T Peek<T>() where T : CachingViewModel
        {
            var c = Peek();
            if(c is T t) { return t; }
            else { return null; }
        }
        public CachingViewModel Pop()
        {
            CleanUpStack();
            if(cacheStack.Count > 0)
            {
                var c = Peek();
                cacheStack.RemoveAt(cacheStack.Count - 1);
                return c;
            }
            else { return null; }
        }
        public T Pop<T>() where T : CachingViewModel
        {
            CleanUpStack();
            var c = Peek();
            if(c is T t)
            {
                cacheStack.RemoveAt(cacheStack.Count - 1);
                return t;
            }
            else
            {
                return null;
            }
        }

        public void InvalidateCache<T>() where T : CachingViewModel
        {
            cacheStack.RemoveAll(x => x is T);
            CleanUpStack();
        }
        public void InvalidateCache<T>(Predicate<T> condition) where T : CachingViewModel
        {
            for(int i = 0; i < cacheStack.Count; )
            {
                if(cacheStack[i] is T t && condition(t) == true)
                {
                    cacheStack.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        void CleanUpStack()
        {
            while(cacheStack.Count > 0 && (cacheStack.Count > MaxCacheSize || (DateTime.Now - cacheStack[0].CreatedTime) > CacheTTL))
            {
                cacheStack.RemoveAt(0);
            }
        }
    }

    abstract class CachingViewModel
    {
        public DateTime CreatedTime { get; }
        public virtual bool EqualTo(object item) => false;

        protected CachingViewModel()
        {
            CreatedTime = DateTime.Now;
        }
    }

    interface IListViewPersistent
    {
        string ScrollPosition { get; }
        void SetScrollPosition(Windows.UI.Xaml.Controls.ListViewBase listView);
        Task RecoverScrollPosition(Windows.UI.Xaml.Controls.ListViewBase listView);
    }
}
