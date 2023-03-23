using System;
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
            iikoCardPOSRequests.GetToken();


            subscriptions = new CompositeDisposable
            {
               PluginContext.Notifications.OrderEditCardSlided.Subscribe(x => Asdasd(x))
            };
             
            PluginContext.Operations.AddButtonToPluginsMenu("SamplePlugin: Password input example", x =>
            {
                x.vm.ShowOkPopup("Password input example", "Test Ok");
            });

            PluginContext.Log.Info("Started");      
        }


        public bool Asdasd((CardInputDialogResult card, Data.Orders.IOrder order, IOperationService os, IViewManager vm) x)
        {
            iikoCardPOSRequests.GetToken();

            //x.vm.ShowOkPopup("Номер карты", $"token {token} card{x.card.FullCardTrack}");
            return true;
        }

    }
}
