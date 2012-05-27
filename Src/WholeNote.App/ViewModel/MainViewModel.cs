using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Timers;
using System;

namespace WholeNote.App.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region Fields

        private ICommand _exitCommand;
        private Timer saveTimer;
        private string _storagePath;
        private string _tabContent;
        private bool _isDirty;
        private string _status;
        private DateTime _lastUpdated;
        private TimeSpan _autoSaveThreshold;
        private string _tabFileName;

        // Attempt auto save every X milliseconds
        private double _attemptAutoSaveTime;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"

                // Auto save every 2 seconds.
                _attemptAutoSaveTime = 1000;
                _autoSaveThreshold = new TimeSpan(0, 0, 0, 1);
                _storagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _tabFileName = "WholeNote.dat";

                saveTimer = new Timer(_attemptAutoSaveTime);
                saveTimer.Elapsed += SaveCurrentFile;
                saveTimer.Start();

                LoadFiles();
            }
        }

        #endregion

        #region Properties

        public string TabContent
        {
            get { return _tabContent; }
            set
            {
                _tabContent = value;
                _lastUpdated = DateTime.Now;
                _isDirty = true;
                RaisePropertyChanged("TabContent");
            }
        }

        /// <summary>
        /// Saves current data and exits application
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(() => Application.Current.Shutdown());
                }
                return _exitCommand;
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        #endregion

        #region Public Methods

        private void LoadFiles()
        {
            string fullPath = Path.Combine(_storagePath, _tabFileName);
            if (File.Exists(fullPath))
            {
                _tabContent = File.ReadAllText(fullPath);
                RaisePropertyChanged("TabContent");
            }
            Status = string.Format("Content loaded.");
        }

        private void SaveCurrentFile(object o, ElapsedEventArgs e)
        {
            if (!_isDirty) return;

            // Only save if something new was entered and the user has (probably) stopped typing
            // their thought.
            if (DateTime.Now - _lastUpdated < _autoSaveThreshold) return;

            string fullPath = Path.Combine(_storagePath, _tabFileName);
            File.WriteAllText(fullPath, _tabContent);
            _isDirty = false;
            Status = string.Format("File saved: {0}", DateTime.Now.ToLongTimeString());
        }

        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}