
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SilverlightSampleWithCaliburnMicro.ViewModels
{
    public class SampleViewModel : Screen
    {
        private string _message;

        public SampleViewModel()
        {

        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public void ReceiveTalkMessage(TalkMessage message)
        {
            Execute.OnUIThread(() => Message = message.WhatToSay);
        }
    }
}
