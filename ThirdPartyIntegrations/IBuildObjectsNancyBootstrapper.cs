/// <summary>
/// Nancy is a lightweight web framework for .NET based on Sinatra. It's used to build light-weight and fast RESTful services.
/// This is the service location bootstrapper for Nancy. It will not compile if included in the IBuildObjects solution because
/// IBuildObjects doesn't and shouldn't reference Nancy binaries. However, this bootstrapper is required for Nancy to work with
/// IBuildObjects so I have included it here for reference.
/// </summary>

public class IBuildObjectsNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<IObjectBuilder>, IDisposable
{
    readonly IObjectBuilder _objectBuilder;

    public IBuildObjectsNancyBootstrapper(IObjectBuilder objectBuilder)
    {
        _objectBuilder = objectBuilder;
    }

    protected override IDiagnostics GetDiagnostics()
    {
        return ApplicationContainer.GetInstance<IDiagnostics>();
    }

    protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
    {
        return ApplicationContainer.GetAllInstances<IApplicationStartup>();
    }

    protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(IObjectBuilder container, Type[] requestStartupTypes)
    {
        return requestStartupTypes.Select(container.GetInstance).Cast<IRequestStartup>().ToArray();
    }

    protected override IEnumerable<IRegistrations> GetRegistrationTasks()
    {
        return ApplicationContainer.GetAllInstances<IRegistrations>();
    }

    protected override INancyEngine GetEngineInternal()
    {
        return ApplicationContainer.GetInstance<INancyEngine>();
    }

    protected override IObjectBuilder GetApplicationContainer()
    {
        return _objectBuilder;
    }

    protected override void RegisterBootstrapperTypes(IObjectBuilder applicationContainer)
    {
        ApplicationContainer.Configure(x =>
        {
            x.Add<INancyModuleCatalog>().BindTo(this);
            x.AddUsing<IFileSystemReader, DefaultFileSystemReader>().Singleton();
        });
    }

    protected override void RegisterTypes(IObjectBuilder container, IEnumerable<TypeRegistration> typeRegistrations)
    {
        container.Configure(x =>
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                x.AddUsing(typeRegistration.RegistrationType, typeRegistration.ImplementationType);
            }
        });
    }

    protected override void RegisterCollectionTypes(IObjectBuilder container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
    {
        container.Configure(x =>
        {
            foreach (var typeRegistration in collectionTypeRegistrationsn)
            {
                foreach (var implementation in typeRegistration.ImplementationTypes)
                {
                    x.AddUsing(typeRegistration.RegistrationType, implementation);
                }
            }
        });
    }

    protected override void RegisterInstances(IObjectBuilder container, IEnumerable<InstanceRegistration> instanceRegistrations)
    {
        foreach (var instance in instanceRegistrations)
        {
            container.Configure(x =>
            {
                x.Add(instance.RegistrationType).BindTo(instance.Implementation);
            });
        }
    }

    protected override IObjectBuilder CreateRequestContainer(NancyContext context)
    {
        return ApplicationContainer.GetChildContainer();
    }

    protected override void RegisterRequestContainerModules(IObjectBuilder container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
    {
        container.Configure(x =>
        {
            foreach (var moduleRegistration in moduleRegistrationTypes)
            {
                x.AddUsing(typeof(INancyModule), moduleRegistration.ModuleType).Singleton();
            }
        });
    }

    protected override IEnumerable<INancyModule> GetAllModules(IObjectBuilder container)
    {
        return container.GetAllInstances<INancyModule>();
    }

    protected override INancyModule GetModule(IObjectBuilder container, Type moduleType)
    {
        return container.GetInstance(moduleType) as INancyModule;
    }
}