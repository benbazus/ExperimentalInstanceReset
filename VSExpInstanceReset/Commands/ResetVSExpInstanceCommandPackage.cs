//------------------------------------------------------------------------------
// <copyright file="ResetVSExpInstanceCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace VSExpInstanceReset
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#1110", "#1112", Vsix.Version, IconResourceID = 1400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidResetVSExpInstanceCommandPackageString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ResetVSExpInstanceCommandPackage : Package
    {
        public ResetVSExpInstanceCommandPackage()
        {

        }

        #region Package Members

        protected override void Initialize()
        {
            Logger.Initialize(this,Vsix.Name );
            ResetVSExpInstanceCommand.Initialize(this);
            base.Initialize();
        }

        #endregion
    }
}


