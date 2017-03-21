#region usings

using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace IBuildObjectsTests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void should_instantiate_and_contain_itself_in_config_as_ibuildobjects()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(objectBoss.ContainsUsing<IObjectBuilder, ObjectBoss>());
        }

        [TestMethod]
        public void should_instanciate_and_contain_iobjectbuilder_as_a_singleton()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());

            Assert.IsTrue(objectBoss.ContainsUsing<IObjectBuilder, ObjectBoss>());
            Assert.IsTrue(objectBoss.GetSingletonCount() > 0);
        }

        [TestMethod]
        public void should_instantiate_and_contain_instantiated_instance_for_objectbuilder()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());

            var ibo = objectBoss.GetInstance<IObjectBuilder>();
            Assert.AreEqual(objectBoss, ibo);
        }

        [TestMethod]
        public void should_register_a_simple_object_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(objectBoss.Contains<SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_a_simple_object_by_method_parameter()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add(typeof(SimpleObjectType)));
            Assert.IsTrue(objectBoss.Contains<SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_an_interface_using_a_concrete_class()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.ContainsUsing<ISimpleInterface, SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_an_interface_by_method_parameter()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing(typeof(ISimpleInterface), typeof(SimpleObjectType)));
            Assert.IsTrue(objectBoss.ContainsUsing<ISimpleInterface, SimpleObjectType>());
        }

        [TestMethod]
        public void should_register_an_interface_and_have_a_default_concrete_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.Contains<ISimpleInterface>());
        }
        
        [TestMethod]
        public void should_add_all_types_from_a_registry()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddRegistry<TestRegistry>());
            Assert.IsTrue(objectBoss.Contains<SimpleObjectType>() && objectBoss.Contains<AnotherSimpleObject>() && objectBoss.Contains<ObjectWithOneDependency>());
        }

        [TestMethod]
        public void should_not_overwrite_config_when_additional_config_is_defined()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.Contains<ISimpleInterface>());
            objectBoss.Configure(x => x.Add<AnotherSimpleObject>());

            Assert.IsTrue(objectBoss.Contains<ISimpleInterface>());
            Assert.IsTrue(objectBoss.Contains<AnotherSimpleObject>());
        }

        [TestMethod]
        public void should_get_instance_of_a_type_that_was_defined_twice()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>());
            Assert.IsTrue(objectBoss.Contains<ISimpleInterface>());
            var simpleObject = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(simpleObject);
        }
    }
}
