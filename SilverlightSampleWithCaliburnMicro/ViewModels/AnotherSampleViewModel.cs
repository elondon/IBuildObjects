
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IBuildObjects;

namespace SilverlightSampleWithCaliburnMicro.ViewModels
{
    public class AnotherSampleViewModel  : Screen
    {
        private readonly IObjectBuilder _objectBuilder;

        public AnotherSampleViewModel(IObjectBuilder objectBuilder)
        {
            _objectBuilder = objectBuilder;
        }

        public void SendMessage()
        {
            _objectBuilder.SendMessage(new TalkMessage("This message has been received by all classes registered for messaging looking for this message!"));
        }
    }
}
