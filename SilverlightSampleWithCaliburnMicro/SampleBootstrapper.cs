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
using SilverlightSampleWithCaliburnMicro.ViewModels;

namespace SilverlightSampleWithCaliburnMicro
{
    public class SampleBootstrapper : Bootstrapper<MainViewModel>
    {

        public SampleBootstrapper()
        {

        }

        protected override void Configure()
        {

        }

        protected override object GetInstance(Type service, string key)
        {
            return null;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return null;
        }

        protected override void BuildUp(object instance)
        {

        }
    }
}
