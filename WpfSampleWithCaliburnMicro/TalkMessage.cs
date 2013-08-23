using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBuildObjects;

namespace WpfSampleWithCaliburnMicro
{
    public class TalkMessage : Message
    {
        public string WhatToSay { get; protected set; }

        public TalkMessage(string whatToSay)
        {
            WhatToSay = whatToSay;
        }
    }
}
