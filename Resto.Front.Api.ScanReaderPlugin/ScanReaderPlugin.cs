using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Resto.Front.Api.Attributes;
using Resto.Front.Api.Attributes.JetBrains;
using Resto.Front.Api.Data.View;
using Resto.Front.Api.UI;

namespace Resto.Front.Api.ScanReaderPlugin
{
    /// <summary>
    /// Test plug-in to demonstrate opportunities of payment system api.
    /// Copy build result Resto.Front.Api.ScanReaderPlugin.dll to \Plugins\Resto.Front.Api.ScanReaderPlugin\ folder near iikoFront.exe
    /// </summary>
    [UsedImplicitly]
    [PluginLicenseModuleId(0021016318)]           //0021016318 ApiPayment         0021005918 TestDemo
    public sealed class ScanReaderPlugin : IFrontPlugin
    {
        private readonly CompositeDisposable subscriptions;

        public void Dispose()
        {
            subscriptions?.Dispose();
        }

        public ScanReaderPlugin()
        {     
            if(FileMethods.ReadConfig())
            {
                PluginContext.Log.Info("Started");
                iikoCardPOSRequests.GetToken();
            }
            else
            {
                PluginContext.Log.Warn("Because configFile is empty or was not read, the plugin was stopped\r\nCheck the ScanReader.config file and reload the plugin");
                PluginContext.Shutdown();
            }

            subscriptions = new CompositeDisposable
            {
               PluginContext.Notifications.OrderEditCardSlided.Subscribe(x => CheckCard(x))
            };         
        }

        public bool CheckCard((CardInputDialogResult card, Api.Data.Orders.IOrder order, IOperationService os, IViewManager vm) x)
        {
            if (x.card.Track2.Contains(" "))
            {
                string card = StringMethods.stringTransform(x.card.Track2);

                if (card != "")
                {
                    if (iikoCardPOSRequests.GetGuestBalance(card))
                    {
                        ShowPopUp(card, x);
                    }
                    else
                    {
                        iikoCardPOSRequests.GetToken();

                        if(iikoCardPOSRequests.GetGuestBalance(card))
                        {
                            ShowPopUp(card, x);
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

        public void ShowPopUp(string card, (CardInputDialogResult card, Api.Data.Orders.IOrder order, IOperationService os, IViewManager vm) x)
        {
            bool addGuest = false;

            if (Data.userWallets.Count > 0)
            {
                bool found = false;

                for (int i = 0; i < Data.userWallets.Count; i++)
                {
                    if (Data.userWallets[i].name == "S7")
                    {
                        found = true;
                        string balance = Data.userWallets[i].balance.ToString();
                        addGuest = x.vm.ShowOkCancelPopup("Информация о госте", $"Баланс гостя {card}: {balance.Substring(0, balance.Length - 3)}р.\r\nДобавить гостя в заказ?");
                    }
                }
                if (!found)
                {
                    x.vm.ShowOkPopup("Информация о госте", $"Гость c картой {card} не найден");
                    PluginContext.Log.Info($"Guest with card {card} not found");
                }
                if (addGuest)
                {
                    try
                    {
                        Guid defaultGuid = new Guid();
                        Guid GuestId = new Guid();
                        bool success = false;

                        for(int i = 0; i < 5; i++)
                        {
                            GuestId = StringMethods.GetCustomerId(x.os, card);

                            if (GuestId != defaultGuid)
                            {
                                success = true;
                                break;
                            }
                        }

                        if(success)
                        {
                            x.os.AddClientToOrder(x.os.AuthenticateByPin(Immutable.pin), x.order, x.os.GetClientById(GuestId));
                        }
                        else
                        {
                            x.vm.ShowOkPopup("Error", "Произошла ошибка плагина");
                            PluginContext.Log.Error($"returned empty id for card: {card}");
                        }
                    }
                    catch (Exception ex)
                    {
                        PluginContext.Log.Error($"AddClientToOrder error, message: {ex.Message}");
                        x.vm.ShowOkPopup("Error", "Произошла ошибка плагина");
                    }
                }
            }
            else
            {
                x.vm.ShowOkPopup("Информация о госте", $"Гость c картой {card} не найден");
                PluginContext.Log.Info($"Guest with card {card} not found");
            }
        }
    }
}
