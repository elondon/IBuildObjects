using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBuildObjects;
using WpfSampleWithCaliburnMicro.ViewModels;

namespace WpfSampleWithCaliburnMicro
{
    public class SampleRegistry : IRegistry
    {
        public Action<IConfiguration> GetConfiguration()
        {
            return x =>
                       {
                           x.Add<MainWindowViewModel>();
                           x.Add<SampleViewModel>().ForMessagging();
                           x.Add<AnotherSampleViewModel>();
                       };
        }
    }
}
