using System;
using System.Linq;
using HandleObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IHandleObjectsTests
{
    [TestClass]
    public class IHandleObjectsTests
    {
        [TestMethod]
        public void should_instantiate_and_contain_itself_in_config_as_ihandleobjects()
        {
            var objectBoss = new ObjectBoss();
            Assert.IsTrue(objectBoss.ContainsUsing<IHandleObjects, ObjectBoss>());
        }

        [TestMethod]
        public void should_instantiate_and_contain_one_object()
        {
            var objectBoss = new ObjectBoss();
            Assert.IsTrue(objectBoss.GetObjectCount() > 0);
        }

        [TestMethod]
        public void should_register_a_simple_object_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Add<SimpleObjectType>();
        }

        [TestMethod]
        public void should_return_parameterless_object_instance()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Add<SimpleObjectType>();
            var simpleObject = objectBoss.GetInstance<SimpleObjectType>();
            Assert.IsNotNull(simpleObject);
            Assert.IsInstanceOfType(simpleObject, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void should_inject_simple_object_type_into_object_with_one_dependency()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Add<SimpleObjectType>();
            objectBoss.Add<ObjectWithOneDependency>();
            var objectWithOneDependecy = objectBoss.GetInstance<ObjectWithOneDependency>();
            Assert.IsInstanceOfType(objectWithOneDependecy, typeof(ObjectWithOneDependency));
            Assert.IsNotNull(objectWithOneDependecy.SimpleObjectType);
            Assert.IsInstanceOfType(objectWithOneDependecy.SimpleObjectType, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void should_wire_up_objects_for_complex_object_with_two_dependencies()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Add<SimpleObjectType>();
            objectBoss.Add<ObjectWithOneDependency>();
            objectBoss.Add<ComplexObjectWithTwoDependencies>();
            var complexObject = objectBoss.GetInstance<ComplexObjectWithTwoDependencies>();
            Assert.IsInstanceOfType(complexObject, typeof(ComplexObjectWithTwoDependencies));
            Assert.IsNotNull(complexObject.SimpleObjectType);
            Assert.IsInstanceOfType(complexObject.SimpleObjectType, typeof(SimpleObjectType));
            Assert.IsNotNull(complexObject.OneDependencyObject);
            Assert.IsInstanceOfType(complexObject.OneDependencyObject, typeof(ObjectWithOneDependency));
            Assert.IsNotNull(complexObject.OneDependencyObject.SimpleObjectType);
            Assert.IsInstanceOfType(complexObject.OneDependencyObject.SimpleObjectType, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void should_get_all_instances_of_an_interface()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.AddUsing<ISimpleInterface, SimpleObjectType>();
            objectBoss.AddUsing<ISimpleInterface, AnotherSimpleObject>();
            var allInstances = objectBoss.GetAllInstances<ISimpleInterface>().ToList();
            Assert.IsTrue(allInstances.Count == 2);
            var simpleObject1 = allInstances[0];
            Assert.IsNotNull(simpleObject1);
            Assert.IsTrue(simpleObject1.Name == "SimpleObject1");
            var simpleObject2 = allInstances[1];
            Assert.IsNotNull(simpleObject2);
            Assert.IsTrue(simpleObject2.Name == "SimpleObject2");
        }

        [TestMethod]
        public void should_retrieve_concrete_classes_of_interfaces_by_key()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.AddUsing<ISimpleInterface, SimpleObjectType>("object1");
            objectBoss.AddUsing<ISimpleInterface, AnotherSimpleObject>("object2");
            var object1 = objectBoss.GetInstance<ISimpleInterface>("object1");
            var object2 = objectBoss.GetInstance<ISimpleInterface>("object2");
            Assert.IsNotNull(object1);
            Assert.IsTrue(object1.Name == "SimpleObject1");
            Assert.IsNotNull(object2);
            Assert.IsTrue(object2.Name == "SimpleObject2");

        }

        [TestMethod]
        public void should_retrieve_a_default_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.AddUsingDefaultType<ISimpleInterface, SimpleObjectType>();
            var simpleType = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(simpleType);
            Assert.IsTrue(simpleType.Name == "SimpleObject1");
        }

        [TestMethod]
        public void should_retrieve_default_type_when_multiple_types_are_defined()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.AddUsing<ISimpleInterface, SimpleObjectType>();
            objectBoss.AddUsingDefaultType<ISimpleInterface, AnotherSimpleObject>();
            var anotherSimpleObject = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(anotherSimpleObject);
            Assert.IsTrue(anotherSimpleObject.Name == "SimpleObject2");
        }
    }

    /// <summary>
    /// Test Helper classes.
    /// </summary>
    /// 
     
    public interface ISimpleInterface
    {
        string Name { get; set; }
    }

    public interface IComplexInterface
    {
        SimpleObjectType SimpleObjectType { get; set; }
        ObjectWithOneDependency OneDependencyObject { get; set; }
    }

    public class SimpleObjectType : ISimpleInterface
    {
        public string Name { get; set; }

        public SimpleObjectType()
        {
            Name = "SimpleObject1";
        }
    }

    public class AnotherSimpleObject : ISimpleInterface
    {
        public string Name { get; set; }

        public AnotherSimpleObject()
        {
            Name = "SimpleObject2";
        }
    }

    public class ObjectWithOneDependency
    {
        public SimpleObjectType SimpleObjectType;

        public ObjectWithOneDependency(SimpleObjectType simpleObjectType)
        {
            SimpleObjectType = simpleObjectType;
        }
    }

    public class ComplexObjectWithTwoDependencies
    {
        public SimpleObjectType SimpleObjectType { get; set; }
        public ObjectWithOneDependency OneDependencyObject { get; set; }

        public ComplexObjectWithTwoDependencies(SimpleObjectType simpleObject, ObjectWithOneDependency objectWithOneDependency)
        {
            SimpleObjectType = simpleObject;
            OneDependencyObject = objectWithOneDependency;
        }
    }
}
