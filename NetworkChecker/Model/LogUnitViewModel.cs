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
        bool bCheckConnect = true;
        DateTime preDateTime = DateTime.Now;

        int Index = 0;
        public ObservableCollection<LogUnit> Items { get; set; }
        public ObservableCollection<LogUnit> StatItems { get; set; }
        public bool IsDataLoaded { get; set; }


        public LogUnitViewModel()
        {
            this.Items = new ObservableCollection<LogUnit>();
            this.StatItems = new ObservableCollection<LogUnit>();
        }

        public void Add(string content, bool bConn = true)
        {
            LogUnit unit = new LogUnit();
            unit.Index = Index;
            unit.Content = content;
            unit.Time = DateTime.Now.ToLongTimeString();
            unit.IsConnected = bConn;

            this.Items.Add(unit);

            Index++;
        }

        public void Insert(string content, bool bConn = true)
        {
            LogUnit unit = new LogUnit();
            unit.Index = Index;
            unit.Content = content;
            unit.Time = DateTime.Now.ToLongTimeString();
            unit.IsConnected = bConn;

            this.Items.Insert(0, unit);

            if(bCheckConnect != bConn) //이 상태면 접속상태가 변경된 시점, 시간 저장
            {
                bCheckConnect = bConn;

                LogUnit statUnit = new LogUnit();
                statUnit.Index = Index;
                if(bConn)
                {
                    TimeSpan span = DateTime.Now - preDateTime;            

                    string info = DiffTime(span);
                    statUnit.Content = content  +", 오류시간: " + info;
                }
                else
                {
                    preDateTime = DateTime.Now;
                    statUnit.Content = content;
                }
                
                statUnit.Time = DateTime.Now.ToLongTimeString();
                statUnit.IsConnected = bConn;

                this.StatItems.Insert(0, statUnit);
            }

            Index++;
        }

        public void Clear()
        {
            this.Items.Clear();
            //this.StatItems.Clear();
        }

        public void SelectedClear(LogUnit start, LogUnit End)
        {
            int st = Math.Min(start.Index, End.Index);
            int ed = Math.Max(start.Index, End.Index);

            var removeItems = this.Items.Where(x => x.Index >= st && x.Index <= ed).ToList();
            foreach(LogUnit unit in removeItems)
            {
                this.Items.Remove(unit);
            }
        }

        public void Add(LogUnit unit)
        {
            this.Items.Add(unit);
        }

        private string DiffTime(TimeSpan span)
        {
            int day = span.Days;
            int hour = span.Hours;
            int minute = span.Minutes;
            int second = span.Seconds;

            string msg = "";
            if (day != 0) msg += day.ToString() + "일 ";
            if (hour != 0) msg += hour.ToString() + "시 ";
            if (minute != 0) msg += minute.ToString() + "분 ";
            if (second != 0) msg += second.ToString() + "초 ";

            return msg;
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
