
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IBuildObjects;

namespace WpfSampleWithCaliburnMicro.ViewModels
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
            _objectBuilder.SendMessage(new TalkMessage("Look at me! I can talk!"));
        }
    }
}
