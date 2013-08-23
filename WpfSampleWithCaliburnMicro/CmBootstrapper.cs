
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
                                          x.AddRegistry<SampleRegistry>();
                                          x.AddUsing<IWindowManager, WindowManager>();
                                          x.AddUsing<IEventAggregator, EventAggregator>();
                                      });


        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _objectBoss.GetInstance(serviceType);
        }
    }
}
