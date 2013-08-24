
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
        private readonly HelloWorldViewModel _helloWorldViewModel;
        private readonly IWindowManager _windowManager;
        private readonly IObjectBuilder _objectBuilder;

        public MainWindowViewModel(SampleViewModel sampleViewModel, HelloWorldViewModel helloWorldViewModel, IWindowManager windowManager, IObjectBuilder objectBuilder)
        {
            _sampleViewModel = sampleViewModel;
            _helloWorldViewModel = helloWorldViewModel;
            _windowManager = windowManager;
            _objectBuilder = objectBuilder;
        }

        protected override void OnActivate()
        {
            HelloWorld();
        }

        public void HelloWorld()
        {
            _helloWorldViewModel.Message = "Hello, World!";
            ActivateItem(_helloWorldViewModel);
        }

        public void SeeSample()
        {
            _sampleViewModel.Message = "This is a sample view model!";
            ActivateItem(_sampleViewModel);
        }

        public void SeeAnotherSample()
        {
            var anotherSample = _objectBuilder.GetInstance<AnotherSampleViewModel>();
            _windowManager.ShowWindow(anotherSample);
        }
    }
}
