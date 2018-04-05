using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;


//cała obsługa głównego okna (łączone przez Binding)
namespace Pzpp
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region property changed func
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion



        public MainWindowViewModel()//ustawienie początkowych wartości elementów
        {
            OpenComputersTableCommand = new RelayCommand(OpenComputersTable); //stworzenie relaycommand tak by można było przypiosać funkcje do elementów okna (naciśnięcie przycisku itp.
            OpenResponsesTableCommand = new RelayCommand(OpenResponsesTable);
            AddDeviceCommand = new RelayCommand(AddDevice);
            PingCommand = new RelayCommand(PingDevice);            
            CanPing = false;
            CanAdd = false;
            using (var db = new PingDataContext())// operowanie na bazie
            {
                db.Database.CreateIfNotExists(); // stworzenie bazy jeśli nie istnieje 
                var devicesAll = db.Devices.ToList(); // wpisanie wszyskich urządzeń do listy by potem poszczególne elementy dodać do właściwości ViewModelu
                foreach (var item in devicesAll)
                {
                    DevicesIP.Add(item.IP);
                    IdControl.Add(item.Id);
                    DevicesList.Add(item.Name+" "+item.IP);
                }

            }
            
        }
        #region properties

        #region pinging properties //własności obsługujące pingowanie
        private ObservableCollection<string> _DevicesList = new ObservableCollection<string>();//lista urządzeń (nazwy i ip wyświetlane w rozwijanej liście) ObservableCollection - kolekcja obserwowalna by mieć pewność że wiemy kiedy się zmienia. Jest private (można użyć tylko w tej klasie) obsługą na zewnątrz jej zajmuję się DeviceList
        public ObservableCollection<string> DevicesList
        {
            get
            {
                return _DevicesList;
            }
            set
            {
                _DevicesList = value; //przypisanie zmienionej wartości do _DeviceList
                //PropertyChanged(this, new PropertyChangedEventArgs(nameof(DevicesIP)));  alternatywne by nie rozpisywać się tak za każdym razem jest funkcja OnPropertyChanged (użyta poniżej)
                OnPropertyChanged(); // służy to odświeżania przy zmianie (by wartości w widoku były takie same jak tu)
            }

        }
        private ObservableCollection<string> DevicesIP = new ObservableCollection<string>();

        private int _SelectedDevice;
        public int SelectedDevice
        {
            get
            {
                return _SelectedDevice;
            }
            set
            {
                if (_SelectedDevice == value) //jeśli własność zostanie zmieniona na to samo nic nie rób
                    return;
                if (value != -1) //zmiana w tej własności w niektórych przypadkach zmienia wartość innej własności
                    CanPing = true;
                else
                    CanPing = false;
                _SelectedDevice = value;
                OnPropertyChanged();
            }
        }
        private List<int> IdControl = new List<int>();
        

        private bool _CanPing;
        public bool CanPing
        {
            get
            {
                return _CanPing;
            }
            set
            {
                if (_CanPing == value)
                    return;
                _CanPing = value;
                OnPropertyChanged();
            }
        }

        private string _PingStatusColor;
        public string PingStatusColor
        {
            get
            {
                return _PingStatusColor;
            }
            set
            {
                if (_PingStatusColor == value)
                    return;
                _PingStatusColor = value;
                OnPropertyChanged();
            }
        }

        private string _PingStatusText;
        public string PingStatusText
        {
            get
            {
                return _PingStatusText;
            }
            set
            {
                if (_PingStatusText == value)
                    return;
                _PingStatusText = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Adding Properties
        private bool _CanAdd;
        public bool CanAdd
        {
            get
            {
                return _CanAdd;
            }
            set
            {
                if (_CanAdd == value)
                    return;
                _CanAdd = value;
                OnPropertyChanged();
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                IPAddress = _IPAddress; // jest to dane by wywołać funkcje która sprawdza ip przy jego zmianie (jest tam całe sprawdzanie poprawności danych łącznie ze sprawdzeniem czy nazwa urządzenia jest wpisana)
                OnPropertyChanged();
            }
        }

        private string _IPAddress;
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }

            set
            {
               
                bool correct;
                string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$"; //regex do sprawdzenia poprawności danych
                Regex check = new Regex(Pattern);

                if (string.IsNullOrEmpty(value))
                    correct= false;
                else
                    correct =  check.IsMatch(value);
                if (correct)
                {

                    if (_Name == null||_Name=="") //sprawdza czy nazwa urządzenia jest wpisana jak nie to nie można dodać
                        CanAdd = false;
                    
                    else CanAdd = true;

                }
                else CanAdd = false;
                _IPAddress = value;
                OnPropertyChanged();

            }
        }

        private string _Description;
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (Description == value)
                    return;
                _Description = value;
                OnPropertyChanged();
            }
        }

       

        #endregion


        #endregion
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

        public ICommand OpenComputersTableCommand { get;private set; }
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
