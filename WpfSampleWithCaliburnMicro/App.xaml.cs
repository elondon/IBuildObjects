using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSampleWithCaliburnMicro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CmBootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new CmBootstrapper();
        }
    }
}
