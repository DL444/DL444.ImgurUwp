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
    static class ViewModelManager
    {
        static Dictionary<string, IManagedViewModel> viewModels { get; } = new Dictionary<string, IManagedViewModel>();
        public static IManagedViewModel GetViewModel(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            IManagedViewModel result;
            viewModels.TryGetValue(name, out result);
            return result;
        }
        public static T GetViewModel<T>(string name) where T : class, IManagedViewModel
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            IManagedViewModel result;
            if (viewModels.TryGetValue(name, out result))
            {
                return result as T;
            }
            else { return null; }
        }
        public static void AddOrUpdateViewModel(string name, IManagedViewModel viewModel)
        {
            if(name == null) { throw new ArgumentNullException(nameof(name)); }
            if(viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }

            if(!viewModels.TryAdd(name, viewModel))
            {
                viewModels[name] = viewModel;
            }
        }
        public static bool RemoveViewModel(string name)
        {
            return viewModels.Remove(name ?? throw new ArgumentNullException(nameof(name)));
        }
    }
}
