using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkChecker.Model
{
    public class LogUnitViewModel : INotifyPropertyChanged
    {
        int Index = 0;
        public ObservableCollection<LogUnit> Items { get; set; }
        public bool IsDataLoaded { get; set; }


        public LogUnitViewModel()
        {
            this.Items = new ObservableCollection<LogUnit>();
        }

        public void Add(string content)
        {
            LogUnit unit = new LogUnit();
            unit.Index = Index.ToString();
            unit.Content = content;
            unit.Time = DateTime.Now.ToLongTimeString();

            this.Items.Add(unit);

            Index++;
        }

        public void Insert(string content)
        {
            LogUnit unit = new LogUnit();
            unit.Index = Index.ToString();
            unit.Content = content;
            unit.Time = DateTime.Now.ToLongTimeString();

            this.Items.Insert(0, unit);

            Index++;
        }

        public void Clear()
        {
            this.Items.Clear();
        }

        public void Add(LogUnit unit)
        {
            this.Items.Add(unit);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
