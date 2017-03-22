IBuildObjects is an easy to use, lightweight tool for dependency injection, wiring up object graphs, and messaging. I created it because I don't need the advanced features and complexity offered by my favorite DI tools in most of my smaller and medium sized applications. I will certainly add to it as becomes necessary, but it started as most libraries start: To fulfil my own use-cases in a re-usable way for my own projects. If you want to use it in your own projects, feel free under the MIT license.

**For a detailed tutorial of the library, see my blog page at:**  http://ericlondon.blogspot.com/p/ibuildobjects.html

**IBuildObjects**

    public interface IObjectBuilder
    {
            void Configure(Action<IConfiguration> configuration);
            int GetObjectCount();
            T GetInstance<T>();
            T GetInstance<T>(string key);
            object GetInstance(Type type);
            IEnumerable<T> GetAllInstances<T>();
            IEnumerable<T> GetAllInstances<T>(string key);
            IEnumerable<object> GetAllInstances(Type type);
            void SendMessage(Message message);
    }
    
**Configuration**


    _objectBoss = new ObjectBoss();
    _objectBoss.Configure(x =>
    {
    	x.AddRegistry<SampleRegistry>();
    	x.AddUsing<IWindowManager, WindowManager>();
    	x.AddUsing<IEventAggregator, EventAggregator>();
    });


**Registry Class**

    public class SampleRegistry : IRegistry
    {
    	public Action<IConfiguration> GetConfiguration()
    	{
    		return x =>
    		{
    			x.Add<MainWindowViewModel>();
    			x.Add<SampleViewModel>().ForMessagging();
    			x.Add<HelloWorldViewModel>().ForMessagging();
    			x.Add<AnotherSampleViewModel>();
    		};
    	}
    }
    
**Configuration with Method Chanining**
    
    
    public class SampleRegistry : IRegistry
    {
    	public Action<IConfiguration> GetConfiguration()
            {
    		return x =>
    		{
    			x.Add<MainWindowViewModel>().Singleton().ForMessagging();
                            x.Add<SampleViewModel>().ForMessagging();
                            x.Add<HelloWorldViewModel>().ForMessagging();
                            x.Add<MessageSendingViewModel>()
                                   .Singleton()
                                   .WithCustomConstructor(new Dictionary<string, object>()
                                   {
                                        {"DebugLogging", true}
                                   });
                           };
            }
    }


    
**Dependency Injection**

    public class MainWindowViewModel : Conductor<Screen>
    {
        private readonly SampleViewModel _sampleViewModel;
        private readonly HelloWorldViewModel _helloWorldViewModel;
        private readonly IWindowManager _windowManager;
        private readonly IObjectBuilder _objectBuilder;
    
        public MainWindowViewModel(SampleViewModel sampleViewModel, HelloWorldViewModel     
            helloWorldViewModel, IWindowManager windowManager, IObjectBuilder objectBuilder)
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
    
 **Messaging**
 
    public class TalkMessage : Message
    {
    	public string WhatToSay { get; protected set; }
        
        public TalkMessage(string whatToSay)
    	{
        		WhatToSay = whatToSay;
        }
    }
        
    public class MessageSendingViewModel  : Screen
    {
    	private readonly IObjectBuilder _objectBuilder;
        
    	public MessageSendingViewModel(IObjectBuilder objectBuilder)
    	{
    		_objectBuilder = objectBuilder;
    	}
        
    	public void SendMessage()
    	{
    		_objectBuilder.SendMessage(new TalkMessage("This message has been received by all classes registered for messaging looking for this message!"));
    	}
    }
            
    public class SampleViewModel : Screen
    {
    	private string _message;
        
    	public SampleViewModel()
    	{
        
    	}
        
    	public string Message
    	{
    		get { return _message; }
            set
            {
             		_message = value;
                    NotifyOfPropertyChange(() => Message);
            }
    	}
        
    	public void ReceiveTalkMessage(TalkMessage message)
    	{
    		Execute.OnUIThread(() => Message = message.WhatToSay);
    	}
    }
    
**Custom Constructors**
    
    objectBoss.Configure(x => x.Add<ObjectWithPrimitives>()
    	.WithCustomConstructor(new Dictionary<string, object>()
            {
            	{"isCool", true},
    		{"myAge", 31},
    		{"complexObject", complexObject}
    	}));
    	
    	
    	
**Life Cycle**

Documentation pending.

**The MIT License (MIT)**

[OSI Approved License]

The MIT License (MIT)

Copyright (c) 2013 Eric London

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. 

    
    
