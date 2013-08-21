
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IBuildObjects;
using WpfSampleWithCaliburnMicro.ViewModels;

namespace WpfSampleWithCaliburnMicro
{
    public class CmBootstrapper : Bootstrapper<MainWindowViewModel>
    {
        private IObjectBuilder _objectBoss;

        public CmBootstrapper()
        {
            Startup();
        }

        public void Startup()
        {
            
        }

        protected override void Configure()
        {
            _objectBoss = new ObjectBoss();
            _objectBoss.Configure(x =>
                                      {
                                          x.AddUsing<IWindowManager, WindowManager>();
                                          x.AddUsing<IEventAggregator, EventAggregator>();
                                          x.Add<MainWindowViewModel>();
                                          x.Add<SampleViewModel>();
                                          x.Add<AnotherSampleViewModel>();
                                      });
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _objectBoss.GetInstance(serviceType);
        }
    }
}
