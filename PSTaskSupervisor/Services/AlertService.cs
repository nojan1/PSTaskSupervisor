using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Media;

namespace PSTaskSupervisor.Services
{
    public class AlertService
    {
        private static readonly TimeSpan ALERTINTERVAL = TimeSpan.Parse(ConfigurationManager.AppSettings["AlertInterval"]);

        private DateTimeOffset? lastAlert;
        private SoundPlayer soundPlayer;

        public AlertService()
        {
            soundPlayer = new SoundPlayer(ConfigurationManager.AppSettings["AlertSoundPath"]);
        }

        public void Alert()
        {
            if(lastAlert == null || lastAlert.Value + ALERTINTERVAL <= DateTimeOffset.Now)
            {
                lastAlert = DateTimeOffset.Now;

                //ALERT ALERT ALERT ALERT ALERT
                Task.Run(() =>
                {
                    soundPlayer.PlaySync();
                });
            }
        }
    }
}
