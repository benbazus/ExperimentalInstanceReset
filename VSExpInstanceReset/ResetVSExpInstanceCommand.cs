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
using VSExpInstanceReset.ViewModel;

namespace VSExpInstanceReset
{

    internal sealed class ResetVSExpInstanceCommand
    {
        public static DTE2 DTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;

        public string _version;
        public string _filePath;

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


        public string Argument
        {
            get
            {
                return @" /Reset /VSInstance=" + _version + "/ RootSuffix=Exp"; ;
            }

        }

        public static void Initialize(Package package)
        {

            Instance = new ResetVSExpInstanceCommand(package);
        }



        private void ResetExpInstance(object sender, EventArgs e)
        {



            string message = "Do you wish to reset Experimental Instance?";
            string title = Resources.Text.ExpInstanceCloseInstance;

           int result=VsShellUtilities.ShowMessageBox(
                 this.ServiceProvider,
                 message,
                 title,
                 OLEMSGICON.OLEMSGICON_INFO,
                 OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                 OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            if(result.Equals(7))
            {
                return;
            }

            ProcessFolderPath();
            DeleteExpAppDataFiles();
            //Delete the registry HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\14.0Exp
            //Delete the registry HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\14.0Exp_Config


            ResetProgressViewModel vm = new ResetProgressViewModel();
            vm.DTE = DTE;
            vm.FilePath = _filePath; 
            vm.Argument = Argument;
            vm.StartResettingExpInstance();
        }

        public void DeleteExpAppDataFiles()
        {
            string appdataFolder=string.Empty;
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                appdataFolder =  Path.Combine(appData, "Microsoft\\VisualStudio\\"+ _version + "Exp\\");

                Logger.Log(appdataFolder);

                if (Directory.Exists(appdataFolder))
                {
                    RecursiveDeleteExpFiles(appdataFolder, true); 
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Log("UnAuthorizedAccessException: Unable to access file. " + ex.Message);
            }
        }

    
        public  void RecursiveDeleteExpFiles(string filePpath, bool recursive)
        {
            if (recursive)
            {
                var subfolders = Directory.GetDirectories(filePpath);
                foreach (var s in subfolders)
                {
                    RecursiveDeleteExpFiles(s, recursive);
                }
            }

            var files = Directory.GetFiles(filePpath);
            foreach (var file in files)
            {
                var attr = File.GetAttributes(file);

                if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    attr = RemoveAttribute(attr, FileAttributes.Hidden);
                    File.SetAttributes(filePpath, attr);
                }

                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, attr ^ FileAttributes.ReadOnly);
                }

                Logger.Log(file);
                File.Delete(file);
            }

            Logger.Log(filePpath);
            Directory.Delete(filePpath);
        }

        private  FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        private void ProcessFolderPath()
        {
            var shell = (IVsShell)ServiceProvider.GetService(typeof(SVsShell));
            object root;

            if (shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out root) == VSConstants.S_OK)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                _version = GetNumbers(Path.GetFileName(root.ToString()));

                _filePath = Path.Combine(appData, "Microsoft Visual Studio " + _version + "\\VSSDK\\VisualStudioIntegration\\Tools\\Bin\\CreateExpInstance.exe");
                Logger.Log("Visual Studio Experimental Instance location: " + _filePath);
            }
        }


        private string GetNumbers(string input)
        {
            return Regex.Replace(input, "[^0-9.]", "");
        }

    }
}

