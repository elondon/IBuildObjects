using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace SilverlightSampleWithCaliburnMicro.ViewModels
{
    public class HelloWorldViewModel : Screen
    {
        public HelloWorldViewModel()
        {
            
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public void ReceiveMessage(TalkMessage talkMessage)
        {
            Message = talkMessage.WhatToSay;
        }
    }
}
