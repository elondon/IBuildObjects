#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using IBuildObjects;

#endregion

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

    public class ObjectWithPrimitives
    {
        public bool IsCool { get; set; }
        public int MyAge { get; set; }
        public ComplexObjectWithTwoDependencies ComplexObject { get; set; }

        public ObjectWithPrimitives(bool isCool, int myAge, ComplexObjectWithTwoDependencies complexObject)
        {
            IsCool = isCool;
            MyAge = myAge;
            ComplexObject = complexObject;
        }
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

    public class ObjectTypeWithInjectedEnumerables
    {
        public IEnumerable<ISimpleInterface> SimpleInterfaces;

        public ObjectTypeWithInjectedEnumerables(IEnumerable<ISimpleInterface> simpleInterfaces)
        {
            SimpleInterfaces = simpleInterfaces;
        }
    }

    public class ObjectThatInheritsFromIEnumerable : IEnumerable<ISimpleInterface>
    {
        public IEnumerable<ISimpleInterface> SimpleInterfaces;

        public ObjectThatInheritsFromIEnumerable(IEnumerable<ISimpleInterface> simpleInterfaces)
        {
            SimpleInterfaces = simpleInterfaces;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ISimpleInterface> GetEnumerator()
        {
            return SimpleInterfaces.GetEnumerator();
        }
    }
    
    public class ObjectWithFuncInjected
    {
        public Func<ISimpleInterface> FuncSimpleInterface;

        public ObjectWithFuncInjected(Func<ISimpleInterface> funcSimpleInterface)
        {
            FuncSimpleInterface = funcSimpleInterface;
        }
    }

    public class ObjectWithMultipleConstuctors
    {
        public SimpleObjectType SimpleObjectType { get; set; }
        public ObjectWithOneDependency OneDependencyObject { get; set; }

        public ObjectWithMultipleConstuctors(SimpleObjectType simpleObjectType)
        {
            SimpleObjectType = simpleObjectType;
        }

        public ObjectWithMultipleConstuctors(SimpleObjectType simpleObjectType,
            ObjectWithOneDependency oneDependencyObject)
        {
            SimpleObjectType = simpleObjectType;
            OneDependencyObject = oneDependencyObject;
        }
    }

    public class ObjectWithMultipleConstructorsWithSameNumberOfParams
    {
        public SimpleObjectType SimpleObjectType { get; set; }
        public ObjectWithOneDependency OneDependencyObject { get; set; }

        public ObjectWithMultipleConstructorsWithSameNumberOfParams(SimpleObjectType simpleObjectType)
        {
            SimpleObjectType = simpleObjectType;
        }

        public ObjectWithMultipleConstructorsWithSameNumberOfParams(ObjectWithOneDependency oneDependencyObject)
        {
            OneDependencyObject = oneDependencyObject;
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

    public class DoesNotReceiveMessage
    {
        public int Count;

        public DoesNotReceiveMessage()
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
