using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaApp.Services
{
    public class StateService
    {
        private int _rpm;

        public int RPM
        {
            get => _rpm;
            private set
            {
                _rpm = value;
                OnChange?.Invoke();
            }
        }

        public event Action? OnChange;

        public void SetRPM(int rpm)
        {
            RPM = rpm;
        }
    }
}
