using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Base.Abstract
{
    public class ModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (object.Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
        }
        public void UpdateProperAction<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "", Action ac = null)
        {
            if (object.Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
            if (ac != null)
                ac();
        }

        private string content;
        public string Content
        {
            get => content;
            set => UpdateProper(ref content, value);
        }

        public virtual void UpdateContent()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string info = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion 
    }
}
