using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reactive.Disposables;
using System.Security.AccessControl;
using System.Windows.Forms;
using System.Windows.Markup;
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
            PluginContext.Log.Info("Started");
            iikoCardPOSRequests.GetToken();

            subscriptions = new CompositeDisposable
            {
               PluginContext.Notifications.OrderEditCardSlided.Subscribe(x => CheckCard(x))
            };
             
            PluginContext.Operations.AddButtonToPluginsMenu("SamplePlugin: Password input example", x =>
            {
                x.vm.ShowOkPopup("Password input example", "Test: Ok");
            });  
        }


        public bool CheckCard((CardInputDialogResult card, Data.Orders.IOrder order, IOperationService os, IViewManager vm) x)
        {
            if (x.card.Track2.Contains(" "))
            {
                string card = StringMethods.stringTransform(x.card.Track2);

                if (card != "")
                {
                    if (iikoCardPOSRequests.GetGuestBalance(card))
                    {
                        ShowPopUp(card, x.vm);
                    }
                    else
                    {
                        iikoCardPOSRequests.GetToken();
                        if (iikoCardPOSRequests.GetGuestBalance(card))
                        {
                            ShowPopUp(card, x.vm);
                        }
                        else
                        {
                            x.vm.ShowOkPopup("Error", $"Произошла ошибка плагина");
                        }
                    }
                }
            }
            return true;
        }

        public void ShowPopUp(string card, IViewManager vm)
        {
            if (Immutable.userWallets.Count > 0)
            {
                bool found = false;
                for (int i = 0; i < Immutable.userWallets.Count; i++)
                {
                    if (Immutable.userWallets[i].name == "S7")
                    {
                        found = true;
                        string balance = Immutable.userWallets[i].balance.ToString();
                        vm.ShowOkPopup("Информация о госте", $"Баланс гостя {card}: {balance.Substring(0, balance.Length - 3)}р.");
                    }
                }
                if (!found)
                {
                    vm.ShowOkPopup("Информация о госте", $"Гость c картой {card} не найден");
                    PluginContext.Log.Info($"Guest with card {card} not found");
                }
            }
            else
            {
                vm.ShowOkPopup("Информация о госте", $"Гость c картой {card} не найден");
                PluginContext.Log.Info($"Guest with card {card} not found");
            }
        }
    }
}
