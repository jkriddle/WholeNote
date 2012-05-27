using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using WholeNote.App.ViewModel;
using System;

namespace WholeNote.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            MouseDown += delegate
            {
                try
                {
                    DragMove();
                }
                catch
                {
                    // Error occurs periodically if click is held??
                }
            };

            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}