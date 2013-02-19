using System;
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
    }

    /// <summary>
    /// Test Helper classes.
    /// </summary>
    public class SimpleObjectType
    {
        public string Name { get; set; }

        public SimpleObjectType()
        {
            Name = "I am a simple object";
        }
    }

    public class ObjectWithOneDependency
    {
        public readonly SimpleObjectType SimpleObjectType;

        public ObjectWithOneDependency(SimpleObjectType simpleObjectType)
        {
            SimpleObjectType = simpleObjectType;
        }
    }

    public class ComplexObjectWithTwoDependencies
    {
        public readonly SimpleObjectType SimpleObjectType;
        public readonly ObjectWithOneDependency OneDependencyObject;

        public ComplexObjectWithTwoDependencies(SimpleObjectType simpleObject, ObjectWithOneDependency objectWithOneDependency)
        {
            SimpleObjectType = simpleObject;
            OneDependencyObject = objectWithOneDependency;
        }
    }
}
