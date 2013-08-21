
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IBuildObjects;

namespace WpfSampleWithCaliburnMicro.ViewModels
{
    public class MainWindowViewModel : Conductor<Screen>
    {
        private readonly SampleViewModel _sampleViewModel;
        private readonly IWindowManager _windowManager;
        private readonly IObjectBuilder _objectBuilder;

        public MainWindowViewModel(SampleViewModel sampleViewModel, IWindowManager windowManager, IObjectBuilder objectBuilder)
        {
            _sampleViewModel = sampleViewModel;
            _windowManager = windowManager;
            _objectBuilder = objectBuilder;
        }

        public void SeeSample()
        {
            ActivateItem(_sampleViewModel);
        }

        public void SeeAnotherSample()
        {
            var anotherSample = _objectBuilder.GetInstance<AnotherSampleViewModel>();
            _windowManager.ShowWindow(anotherSample);
        }
    }
}
