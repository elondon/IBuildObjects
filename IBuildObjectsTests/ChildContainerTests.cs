using System;
using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBuildObjectsTests
{
    [TestClass]
    public class ChildContainerTests
    {
        [TestMethod]
        public void should_get_child_container()
        {
            var objectBoss = new ObjectBoss();
            var child = objectBoss.GetChildContainer();
            Assert.IsNotNull(child);
        }

        [TestMethod]
        public void should_register_with_child_container_and_get_instance()
        {
            var objectBoss = new ObjectBoss();
            var child = objectBoss.GetChildContainer();
            child.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(child.Contains<SimpleObjectType>());
            var simpleObject = child.GetInstance<SimpleObjectType>();
            Assert.IsNotNull(simpleObject);
        }

        [TestMethod]
        public void should_bubble_up_to_parent_to_get_instance()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.ContainsUsing<ISimpleInterface, SimpleObjectType>());

            var child = objectBoss.GetChildContainer();
            Assert.IsFalse(child.ContainsUsing<ISimpleInterface, SimpleObjectType>());

            var simpleObject = child.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(simpleObject);
        }

        [TestMethod]
        public void should_get_instance_based_on_child_config_if_defined()
        {
            var objectBoss = new ObjectBoss();
            var child = objectBoss.GetChildContainer();
            child.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(child.ContainsUsing<ISimpleInterface, SimpleObjectType>());

            var simpleObject = child.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(simpleObject);
            Assert.IsInstanceOfType(simpleObject, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void child_config_should_override_parent_config()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.ContainsUsing<ISimpleInterface, SimpleObjectType>());

            var child = objectBoss.GetChildContainer();
            child.Configure(x => x.AddUsing<ISimpleInterface, AnotherSimpleObject>());
            Assert.IsTrue(child.ContainsUsing<ISimpleInterface, AnotherSimpleObject>());

            var rootSimpleInterface = objectBoss.GetInstance<ISimpleInterface>();
            var childSimpleInterface = child.GetInstance<ISimpleInterface>();
            
            Assert.IsInstanceOfType(rootSimpleInterface, typeof(SimpleObjectType));
            Assert.IsInstanceOfType(childSimpleInterface, typeof(AnotherSimpleObject));
        }

        [TestMethod]
        public void should_register_singleton_on_root_and_get_instance_from_child()
        {
            var objectBoss = new ObjectBoss();
            var child = objectBoss.GetChildContainer();

            objectBoss.Configure(x => x.Add<SimpleObjectType>().Singleton());
            var simpleObject1 = objectBoss.GetInstance<SimpleObjectType>();
            var simpleObject2 = objectBoss.GetInstance<SimpleObjectType>();
            Assert.IsTrue(simpleObject1.Id == simpleObject2.Id);

            var simpleObject3 = child.GetInstance<SimpleObjectType>();
            Assert.IsTrue(simpleObject2.Id == simpleObject3.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(IBuildObjectsException))]
        public void should_throw_when_registering_singleton_on_child_container()
        {
            var objectBoss = new ObjectBoss();
            var child = objectBoss.GetChildContainer();
            child.Configure(x => x.Add<SimpleObjectType>().Singleton());

        }
    }
}
