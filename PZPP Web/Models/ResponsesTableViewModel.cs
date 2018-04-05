using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pzpp
{
    class ResponsesTableViewModel : INotifyPropertyChanged
    {
        #region property changed func
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion



        public ResponsesTableViewModel()
        {
            using (var db = new PingDataContext())
            {
                PingResponses = db.Responses.ToList();
            }
        }
        private List<Responses> _PingResponses;
        public List<Responses> PingResponses
        {
            get
            {
                return _PingResponses;
            }
            set
            {
                if (_PingResponses == value)
                    return;
                _PingResponses = value;
                OnPropertyChanged();
            }
        }

    }
}
