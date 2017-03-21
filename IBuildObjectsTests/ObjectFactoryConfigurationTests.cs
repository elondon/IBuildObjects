#region usings

using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace IBuildObjectsTests
{
    [TestClass]
    public class ObjectFactoryConfigurationTests
    {
        [TestInitialize]
        public void initialize()
        {
            ObjectFactory.ClearContainer();
        }

        [TestMethod]
        public void should_be_able_to_statically_configure()
        {
            ObjectFactory.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(ObjectFactory.ContainsUsing<IObjectBuilder, ObjectBoss>());
        }

        [TestMethod]
        public void should_initialize_and_contain_iobjectbuilder_as_a_singleton()
        {
            ObjectFactory.ClearContainer();
            ObjectFactory.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(ObjectFactory.ContainsUsing<IObjectBuilder, ObjectBoss>());
            Assert.IsTrue(ObjectFactory.GetSingletonCount() > 0);
        }

        [TestMethod]
        public void should_initialize_and_contain_instantiated_instance_for_objectbuilder()
        {
            ObjectFactory.Configure(x => x.Add<SimpleObjectType>());
            var ibo = ObjectFactory.GetInstance<IObjectBuilder>();
            Assert.AreEqual(ObjectFactory.Container, ibo);
        }

        [TestMethod]
        public void should_register_a_simple_object_type()
        {
            ObjectFactory.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(ObjectFactory.Contains<SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_an_interface_using_a_concrete_class()
        {
            ObjectFactory.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(ObjectFactory.ContainsUsing<ISimpleInterface, SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_an_interface_and_have_a_default_concrete_type()
        {
            ObjectFactory.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(ObjectFactory.Contains<ISimpleInterface>());
        }

        [TestMethod]
        public void should_add_all_types_from_a_registry()
        {
            ObjectFactory.Configure(x => x.AddRegistry<TestRegistry>());
            Assert.IsTrue(ObjectFactory.Contains<SimpleObjectType>() && ObjectFactory.Contains<AnotherSimpleObject>() && ObjectFactory.Contains<ObjectWithOneDependency>());
        }
    }
}
