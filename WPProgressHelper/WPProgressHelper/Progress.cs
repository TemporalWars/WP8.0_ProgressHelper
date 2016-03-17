using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BenScharbach.WP8.Helpers.WPProgressHelper
{
    // TODO: Put in a Task to run progress.  Currently, I expect a task from calling party. - Ben
    /// <summary>
    /// The <see cref="Progress"/> class to communicate to Windows Phone.
    /// </summary>
    public class Progress<TGeneric> 
    {
        private ProgressIndicator _progressIndicator;
        private IEnumerable<TGeneric> _items;
        private double _progressValue;
        private double _progressIncrement;

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PhoneApplicationPage CurrentPage { get; private set; }       

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Progress(PhoneApplicationPage currentPage, IEnumerable<TGeneric> items)
        {
            if (currentPage == null) throw new ArgumentNullException("currentPage");
            if (items == null) throw new ArgumentNullException("items");           

            // set increment value.
            _progressIncrement = 1.0 / items.Count();

            // safe refs
            CurrentPage = currentPage;
            _items = items;

            // create Window-Phone ProgressIndicator on UI thread.
            CreateProgressIndicator();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateProgressIndicator()
        {
             // create the ProgressIndicator on UI thread.
            CurrentPage.Dispatcher.BeginInvoke(CreateProgressIndicatorCallback);
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TGeneric> Items()
        {
            foreach (var item in _items)
            {
                yield return item;
                SetProgress();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetProgress()
        {
            _progressValue += _progressIncrement;            
            CurrentPage.Dispatcher.BeginInvoke(() => _progressIndicator.Text = "Static " + _progressValue + "%");
            CurrentPage.Dispatcher.BeginInvoke(() => _progressIndicator.Value = _progressValue);
            CurrentPage.Dispatcher.BeginInvoke(() => SystemTray.SetProgressIndicator(CurrentPage, _progressIndicator));
            Thread.Sleep(10);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateProgressIndicatorCallback()
        {
            _progressIndicator = new ProgressIndicator() { IsVisible = true };
        }

        #region Private Classes

        /// <summary>
        /// 
        /// </summary>
        private class PopulateAsyncState
        {
            public IEnumerable<TGeneric> Items { get; set; }
            public ProgressIndicator ProgressIndicator { get; set; }
        }

        #endregion

    }
}
