using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBuildObjects;
using SilverlightSampleWithCaliburnMicro.ViewModels;

namespace SilverlightSampleWithCaliburnMicro
{
    public class SampleRegistry : IRegistry
    {
        public Action<IConfiguration> GetConfiguration()
        {
            return x =>
                       {
                           x.Add<MainWindowViewModel>();
                           x.Add<SampleViewModel>().ForMessagging();
                           x.Add<HelloWorldViewModel>().ForMessagging();
                           x.Add<MessageSendingViewModel>();
                       };
        }
    }
}
