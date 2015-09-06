﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.SimpleClock.Views;
namespace Wox.Plugin.SimpleClock.Commands
{

    public class AlarmCommand : CommandHandlerBase
    {
        public AlarmCommand(PluginInitContext context, CommandHandlerBase parent): base(context, parent)
        {
            _subCommands.Add(new AlarmSetCommand(context, this));
            _subCommands.Add(new AlarmTimerCommand(context, this));
            _subCommands.Add(new AlarmListCommand(context, this));
            _subCommands.Add(new AlarmEditCommand(context, this));
            _subCommands.Add(new AlarmDeleteCommand(context, this));
            

            System.Timers.Timer alarmTimer = new System.Timers.Timer(5000);
            alarmTimer.Elapsed += AlarmTimer_Elapsed;
            alarmTimer.Start();
        }

        List<ClockSettingsStorage.StoredAlarm> _alarms;
        private void AlarmTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_alarms == null)
                _alarms = ClockSettingsStorage.Instance.Alarms;
            var toFire = _alarms.Where(r => !r.Fired).Where(r => r.AlarmTime < DateTime.Now);
            if (toFire.Count() == 0) return;
            var alarmToFire = toFire.First();
            if (alarmToFire == null) return;
            alarmToFire.Fired = true;
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => {
                var window = new AlarmNotificationWindow(alarmToFire.AlarmTime, alarmToFire.Name, ClockSettingsStorage.Instance.AlarmTrackPath);
                window.Show();
                window.Focus();
            }));
            ClockSettingsStorage.Instance.Save();
        }

        public override string CommandAlias
        {
            get{ return "alarm"; }
        }

        public override string CommandDescription
        {
            get
            {
                return "Allows to set an alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Alarm";
                
            }
        }
        
        public override string GetIconPath()
        {
            return "Images\\alarm-blue.png";
        }
      

    }
    
}