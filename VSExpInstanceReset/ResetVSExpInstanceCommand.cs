//------------------------------------------------------------------------------
// <copyright file="ResetVSExpInstanceCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.VisualStudio;
using EnvDTE80;

namespace VSExpInstanceReset
{

    internal sealed class ResetVSExpInstanceCommand
    {
        private static readonly EnvDTE.vsStatusAnimation _animation = EnvDTE.vsStatusAnimation.vsStatusAnimationFind;
        public static DTE2 DTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
        private static bool _isProcessing = false;

        public string Version { get; set; }


        private ResetVSExpInstanceCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.ServiceProvider = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidList.guidResetVSExpInstanceCommandPackageCmdSet, PackageIds.ResetExpCacheId);
                var menuItem = new MenuCommand(this.ResetExpInstance, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static ResetVSExpInstanceCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get;
        }

        public static void Initialize(Package package)
        {
            Instance = new ResetVSExpInstanceCommand(package);
        }



        private void ResetExpInstance(object sender, EventArgs e)
        {
            bool isReset= System.Windows.Forms.MessageBox.Show("This will Reset the Visual Studio Experiment Instance",
           "Reset Experimental Instance", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes;



            string filePath = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe";
            string arguments = @" /Reset /VSInstance=14.0/ RootSuffix=Exp";
           // string folder= @"C:\Windows\system32\Cmd.exe /C" + @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe " +" /Reset /VSInstance=14.0 /RootSuffix=Exp && PAUSE";
           

            if (!isReset)
                return;


            StartExecuting( filePath, arguments);

        }

    
        public void StartExecuting(  string path, string argument)
        {
            if (_isProcessing)
                return;

            ThreadPool.QueueUserWorkItem(o =>
            {
                _isProcessing = true;

                try
                {
                    Logger.Log(Resources.Text.CreateExpResetMessage);
                    DTE.StatusBar.Text = Resources.Text.CreateExpResetMessage;
                    DTE.StatusBar.Animate(true, _animation);
                    DTE.StatusBar.Progress(true, "Please waiting. resetting exp instance", 10 , 100); ;

                    Execute(path, argument);

                }
                catch(Exception ex)
                {
                    Logger.Log(ex.Message.ToString());
                    DTE.StatusBar.Clear();
                }
                finally
                {
                    DTE.StatusBar.Animate(false, _animation);
                    _isProcessing = false;
                    DTE.StatusBar.Progress(false);
                    DTE.StatusBar.Text = Resources.Text.CreateExpCompleted;
                }

            });
        }

        private void Execute(string path, string argument)
        {

            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(path)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false,
                Arguments = argument,
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
                    DTE.StatusBar.Clear();
                else
                    DTE.StatusBar.Text = Resources.Text.CreateExpCompleted;
            }
            catch
            {
                DTE.StatusBar.Text = Resources.Text.CreateExpError;
            }
        }

        private void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            Logger.Log(e.Data);
            Console.Write(e.Data);
            DTE.StatusBar.Progress(true, "Please waiting, resetting exp instance", 50, 100); ;
        }

        private void ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.Data))
                return;

            Logger.Log(e.Data);
            Console.Write(e.Data);
        }

    }
}

