using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VSExpInstanceReset.View;

namespace VSExpInstanceReset.ViewModel
{
    public class ResetProgressViewModel : INotifyPropertyChanged
    {
        private  bool _isProcessing = false;
        private  bool _isError = false;
        private ResetProgressView dialog;

        public ResetProgressViewModel()
        {
            IsVisible = Visibility.Collapsed;
            ProgressbarVisible = Visibility.Visible;
        }


        #region Public properties

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _progressbarVisible;
        public Visibility ProgressbarVisible
        {
            get
            {
                return _progressbarVisible;
            }

            set
            {
                if (value != _progressbarVisible)
                {
                    _progressbarVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        public EnvDTE80.DTE2 DTE { get; internal set; }
        public string FilePath { get; internal set; }
        public string Argument { get; internal set; }
        public string Version { get; internal set; }

        #endregion




        private void Cancel()
        {
            dialog.Close();
            dialog = null;
        }

        internal async void StartResettingExpInstance()
        {
            if (_isProcessing)
                return;

            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(async () =>
            {
                await ResetExecute();

            }), DispatcherPriority.SystemIdle, null);

        }

        private async Task ResetExecute()
        {
            var hwnd = new IntPtr(DTE.MainWindow.HWnd);
            var window = (Window)System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual;

            dialog = new ResetProgressView();
            dialog.Owner = window;
            dialog.DataContext = this;
            dialog.Show();


            await Task.Run(() =>
            {
                StartProcess();
            });

            if (!_isError)
            {
                if (dialog != null && dialog.IsVisible)
                {
                    dialog.Close();
                    dialog = null;
                }
            }
        }

        private void StartProcess()
        {
            _isProcessing = true;
            Title = Resources.Text.ExpInstanceTitle;
            Message = Resources.Text.ExpInstanceMessage;

            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(FilePath)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                Arguments = Argument,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            try
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo = start;
                    p.EnableRaisingEvents = true;

                    p.OutputDataReceived += OutputDataReceived;
                    p.ErrorDataReceived += ErrorDataReceived;

                    p.Start();

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _isError = true;
                Logger.Log(ex.Message.ToString());
                Message = ex.Message.ToString();
                IsVisible = Visibility.Visible;
                ProgressbarVisible = Visibility.Collapsed;
            }
            finally
            {
                _isProcessing = false;
            }
        }


        private void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;
            Logger.Log(e.Data);
        }

        private void ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            Logger.Log(e.Data);
            Message = e.Data;
            _isError = true;
            IsVisible = Visibility.Visible;
            ProgressbarVisible = Visibility.Collapsed;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
