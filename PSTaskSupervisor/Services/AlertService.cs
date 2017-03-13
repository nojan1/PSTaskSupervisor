using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Media;
using GalaSoft.MvvmLight;

namespace PSTaskSupervisor.Services
{
    public class AlertService : ObservableObject
    {
        private static readonly TimeSpan ALERTINTERVAL = TimeSpan.Parse(ConfigurationManager.AppSettings["AlertInterval"]);

        private DateTimeOffset? lastAlert;
        private SoundPlayer soundPlayer;

        private bool alertPending;
        public bool AlertPending { get { return alertPending; } private set { Set(ref alertPending, value); } }

        public AlertService()
        {
            var soundPath = ConfigurationManager.AppSettings["AlertSoundPath"];
            if (!string.IsNullOrWhiteSpace(soundPath))
            {
                soundPlayer = new SoundPlayer(soundPath);
            }
        }

        public void Alert()
        {
            if (lastAlert == null || lastAlert.Value + ALERTINTERVAL <= DateTimeOffset.Now)
            {
                lastAlert = DateTimeOffset.Now;
                AlertPending = true;

                //ALERT ALERT ALERT ALERT ALERT
                if (soundPlayer != null)
                {
                    Task.Run(() =>
                    {
                        soundPlayer.PlaySync();
                    });
                }
            }
        }

        public void ClearAlert()
        {
            AlertPending = false;
        }
    }
}
