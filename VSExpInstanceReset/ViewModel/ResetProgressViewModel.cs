using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VSExpInstanceReset.View;

namespace VSExpInstanceReset.ViewModel
{
    public class ResetProgressViewModel : INotifyPropertyChanged
    {
        private static bool _isProcessing = false;
        private ResetProgressView dialog;

        public ResetProgressViewModel()
        {

        }


        #region Public properties

        private string _title= "Resetting Experimental Instance";
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

        public EnvDTE80.DTE2 DTE { get; internal set; }
        public string FilePath { get; internal set; }
        public string Argument { get; internal set; }
        public string Version { get; internal set; }

        #endregion

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

            try
            {

                await System.Threading.Tasks.Task.Run(() =>
                {
                    StartProcess();
                });

                if (dialog != null && dialog.IsVisible)
                {
                    dialog.Close();
                    dialog = null;
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message.ToString());
                Message=ex.Message.ToString();
            }
            finally
            {
                _isProcessing = false;
                Message = Resources.Text.CreateExpCompleted;
            }


        }

        private void StartProcess()
        {
            _isProcessing = true;
            Message = "Resetting Experimental Instance...";

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

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = start;
                p.EnableRaisingEvents = true;

                p.OutputDataReceived += OutputDataReceived;
                p.ErrorDataReceived += ErrorDataReceived;
                p.Exited += Exited;

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
        }

        private void Exited(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process p = (System.Diagnostics.Process)sender;

            try
            {
                if (p.ExitCode == 0)
                    dialog.Close();
                else
                    Message = Resources.Text.CreateExpCompleted;
            }
            catch
            {
                Message = Resources.Text.CreateExpError;
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
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
