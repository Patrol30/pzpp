using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pzpp
{
    class ComputersViewModel: INotifyPropertyChanged
    {
        #region property changed func
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion

        public ComputersViewModel()
        {
            DeleteCommand = new RelayCommand(Delete);
            using (var db = new PingDataContext())
            {
                Computers = new ObservableCollection<Devices>(db.Devices.ToList()); //przypisanie tabelki z bazy do programu (na ten sposób wpadłem puźniej dlatego w innych jest to zrobione przez przegląd listy - MainWindowViewModel)
            }
        }

        private ObservableCollection<Devices> _Computers = new ObservableCollection<Devices>(); //tabelka komputerów
        public ObservableCollection<Devices> Computers
        {
            get
            {
                return _Computers;
            }
            set
            {
                if (_Computers == value)
                    return;
                _Computers = value;
                OnPropertyChanged();
            }
        }


        private int _SelectedDevice;//zaznaczony wiersz tabelki
        public int SelectedDevice
        {
            get
            {
                return _SelectedDevice;
            }
            set
            {
                if (_SelectedDevice == value)
                    return;
                _SelectedDevice = value;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteCommand { get; private set; }
        private void Delete()//funkcja usuwania rekordów z bazy (usuwa też powiązane kaskadowo)
        {
            using (var db = new PingDataContext())
            {
                Devices device = new Devices() { Id = _Computers[_SelectedDevice].Id };//sprawdzenie id zaznaczoneko rekordu i stworzenie jego obiektu               
                db.Devices.Attach(device);//przyłączenie tego obiektu do istniejącego w bazie (id jest tylko jedno)
                db.Devices.Remove(device);//usunięcie rekordu
                db.SaveChanges();
                Computers = new ObservableCollection<Devices>(db.Devices.ToList());//aktualizacja tabelki
            }
        }




    }    
}
