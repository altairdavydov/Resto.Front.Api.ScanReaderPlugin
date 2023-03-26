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
using Resto.Front.Api.Data.Brd;
using Resto.Front.Api.Data.Security;
using Resto.Front.Api.Data.View;
using Resto.Front.Api.Exceptions;
using Resto.Front.Api.Extensions;
using Resto.Front.Api.UI;
using System.IO;
using Resto.Front.Api.Data.Settings;

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
             
            PluginContext.Operations.AddButtonToPluginsMenu("SamplePlugin: Password input example", x =>
            {
                string a = "asd";
                
                if(File.Exists("C:\\Users\\Altair\\AppData\\Roaming\\iiko\\CashServer\\PluginConfigs\\Resto.Front.Api.ScanReader\\123.txt"))
                {
                    a = "sdf";
                }

                x.vm.ShowOkPopup("Password input example", $"Test: {a}");
            });  
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
                        try
                        {
                            PhoneDto phoneDto = new PhoneDto();
                            phoneDto.PhoneValue = "+71234567890";
                            phoneDto.IsMain = true;
                            List<PhoneDto> phones = new List<PhoneDto>();
                            phones.Add(phoneDto);

                            //x.os.CreateClient(Guid.NewGuid(), "asdasd", phones, card, DateTime.Now, x.os.AuthenticateByPin("0000"));
                            Guid a = Guid.Parse("bafc06d3-b2a0-4326-b3a0-556b79d6b927");
                            x.os.AddClientToOrder(x.os.AuthenticateByPin("0000"), x.order, x.os.GetClientById(a));
                        }
                        catch (Exception ex) 
                        {
                            PluginContext.Log.Error($"TEST {ex.Message}");
                        }
                        ShowPopUp(card, x.vm);
                    }
                    else
                    {
                        iikoCardPOSRequests.GetToken();
                        if (iikoCardPOSRequests.GetGuestBalance(card))
                        {
                            ShowPopUp(card, x.vm);
                            //x.os.AddIikoCardReferrerToOrder();
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
                        addGuest = vm.ShowOkCancelPopup("Информация о госте", $"Баланс гостя {card}:   {balance.Substring(0, balance.Length - 3)}р.\r\nДобавить гостя в заказ?");
                    }
                }
                if (!found)
                {
                    vm.ShowOkPopup("Информация о госте", $"Гость c картой {card} не найден");
                    PluginContext.Log.Info($"Guest with card {card} not found");
                }
                if (addGuest)
                {

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
