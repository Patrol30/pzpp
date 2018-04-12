using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Input;

namespace PZPP_Web.Controllers
{
    public class MainWindowController : Controller
    {
        // GET: MainWindow
        public ActionResult Index()
        {
            return View();
        }
        #region Commands

        public ICommand PingCommand { get; private set; } //służy do wywołania funkcji przez binding musi być przypisana do funkcji w konstruktorze 
        private void PingDevice() //funkcja pingowania
        {
            Ping pinger = new Ping();
            try
            {
                if (_SelectedDevice != -1)
                {
                    var id = IdControl[_SelectedDevice];//bierze id zaznaczonego urządzenia 
                    PingReply reply = pinger.Send(DevicesIP[SelectedDevice]);//przypisanie jakie ip pingujemy 
                    using (var db = new PingDataContext())
                    {
                        if (reply.Status == IPStatus.Success)//jeśli ping się powiódł 
                        {
                            PingStatusColor = "Green";
                            PingStatusText = "TRUE";
                            db.Responses.Add(new Responses() //sapisanie donych do bazy
                            {
                                Device_Id = id,
                                Success = true,
                                PingTime = reply.RoundtripTime,//czas pingu (ms)
                                Time = DateTime.Now//Data pingu
                            });
                        }
                        else//jak się nie udał
                        {
                            PingStatusColor = "Red";
                            PingStatusText = "False";
                            db.Responses.Add(new Responses()
                            {

                                Device_Id = id,
                                Success = false,
                                Time = DateTime.Now


                            });
                        }
                        db.SaveChanges(); //zapis danych do bazy (zostaje przesłane do bazy SQL)
                    }
                }
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }

        }

        public ICommand AddDeviceCommand { get; private set; }

        private void AddDevice() // dodawanie urządzenia 
        {
            using (var db = new PingDataContext())
            {
                db.Devices.Add(new Devices()
                {
                    IP = _IPAddress,
                    Name = _Name,
                    Description = _Description
                });

                db.SaveChanges();
                DevicesList.Add(_Name + " " + _IPAddress); //dodanie nowego urządzenia do listy
                DevicesIP.Add(_IPAddress);//dodanie ip
                IdControl.Add(db.Devices.ToList().Last().Id);//dodanie id nowego elementu (bierze ostatnie id z bazy - to dodane)
                Name = "";
                IPAddress = "";
                Description = "";
            }
        }

        public ICommand OpenComputersTableCommand { get; private set; }
        private void OpenComputersTable()//funkcja która otwiera tabelkę urządzeń
        {
            Computers computers = new Computers();
            computers.ShowDialog(); // nie można kożystać z głównego okna puki to jest otwarty (Dialog)
            using (var db = new PingDataContext()) // wyczyszczenie i wpisanie danych na nowo (można w tym oknie usuwać dane więc po zamknięciu trzeba zaktualizować)
            {
                DevicesIP.Clear();
                IdControl.Clear();
                DevicesList.Clear();
                var devicesAll = db.Devices.ToList();
                foreach (var item in devicesAll)
                {
                    DevicesIP.Add(item.IP);
                    IdControl.Add(item.Id);
                    DevicesList.Add(item.Name + " " + item.IP);
                }

            }
        }
        public ICommand OpenResponsesTableCommand { get; private set; }
        private void OpenResponsesTable()
        {
            ResponsesTable responses = new ResponsesTable();
            responses.ShowDialog();
        }
        #endregion

    }
}