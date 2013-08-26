using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using IBuildObjects;
using SilverlightSampleWithCaliburnMicro.ViewModels;

namespace SilverlightSampleWithCaliburnMicro
{
    public class SampleBootstrapper : Bootstrapper<MainWindowViewModel>
    {

       private IObjectBuilder _objectBoss;

       public SampleBootstrapper()
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
