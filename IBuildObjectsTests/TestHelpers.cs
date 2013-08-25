using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBuildObjects;

namespace IBuildObjectsTests
{
    /// <summary>
    /// Test Helper classes.
    /// </summary>
    /// 

    public interface ISimpleInterface
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }

    public interface IComplexInterface
    {
        Guid Id { get; set; }
        SimpleObjectType SimpleObjectType { get; set; }
        ObjectWithOneDependency OneDependencyObject { get; set; }
    }

    public class SimpleObjectType : ISimpleInterface
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public SimpleObjectType()
        {
            Name = "SimpleObject1";
            Id = Guid.NewGuid();
        }
    }

    public class AnotherSimpleObject : ISimpleInterface
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public AnotherSimpleObject()
        {
            Name = "SimpleObject2";
            Id = Guid.NewGuid();
        }
    }

    public class ObjectWithOneDependency
    {
        public SimpleObjectType SimpleObjectType;
        public Guid Id { get; set; }

        public ObjectWithOneDependency(SimpleObjectType simpleObjectType)
        {
            SimpleObjectType = simpleObjectType;
            Id = Guid.NewGuid();
        }
    }

    public class ComplexObjectWithTwoDependencies : IComplexInterface
    {
        public SimpleObjectType SimpleObjectType { get; set; }
        public ObjectWithOneDependency OneDependencyObject { get; set; }
        public Guid Id { get; set; }

        public ComplexObjectWithTwoDependencies(SimpleObjectType simpleObject, ObjectWithOneDependency objectWithOneDependency)
        {
            SimpleObjectType = simpleObject;
            OneDependencyObject = objectWithOneDependency;
            Id = Guid.NewGuid();
        }
    }

    public class ComplexObjectWithInterfaceDependencies
    {
        public Guid Id { get; set; }
        public ISimpleInterface SimpleInterface { get; set; }
        public IComplexInterface ComplexInterface { get; set; }

        public ComplexObjectWithInterfaceDependencies(ISimpleInterface simpleInterface, IComplexInterface complexInterface)
        {
            Id = Guid.NewGuid();
            SimpleInterface = simpleInterface;
            ComplexInterface = complexInterface;
        }
    }

    public class TestRegistry : IRegistry
    {
        public Action<IConfiguration> GetConfiguration()
        {
            return x =>
                   {
                       x.Add<SimpleObjectType>();
                       x.Add<AnotherSimpleObject>();
                       x.Add<ObjectWithOneDependency>();
                   };
        }
    }

    /// <summary>
    /// Test Helper classes.
    /// </summary>

    public class ReceiveMessage
    {
        public int Count;

        public ReceiveMessage()
        {
            Count = 1;
        }

        public void Add(AddMessage message)
        {
            Count += message.HowMuchToAdd;
        }
    }

    public class AddMessage : Message
    {
        public int HowMuchToAdd { get; set; }
    }
}
