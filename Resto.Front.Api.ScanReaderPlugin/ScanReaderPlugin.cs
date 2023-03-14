﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Security.AccessControl;
using System.Windows.Forms;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.Security;
using Resto.Front.Api.Data.View;
using Resto.Front.Api.Exceptions;
using Resto.Front.Api.UI;

namespace Resto.Front.Api.ScanReaderPlugin
{
    /// <summary>
    /// Test plug-in to demonstrate opportunities of payment system api.
    /// Copy build result Resto.Front.Api.ScanReaderPlugin.dll to \Plugins\Resto.Front.Api.ScanReaderPlugin\ folder near iikoFront.exe
    /// </summary>
    [UsedImplicitly]
    [PluginLicenseModuleId(0021005918)]
    public sealed class ScanReaderPlugin : IFrontPlugin
    {
        private readonly CompositeDisposable subscriptions;

        public void Dispose()
        {
            subscriptions?.Dispose();
        }

        public ScanReaderPlugin()
        {
            subscriptions = new CompositeDisposable
            {
               PluginContext.Notifications.OrderEditCardSlided.Subscribe(x => x.vm.ShowYesNoPopup(x.card.Track1, x.card.Track2 + " " + x.card.FullCardTrack))
            };
             
            PluginContext.Operations.AddButtonToPluginsMenu("SamplePlugin: Password input example", x =>
            {
                x.vm.ShowOkPopup("Password input example", "asdasd");
            });

            PluginContext.Log.Warn("asdasd123");      
        }
    }
}