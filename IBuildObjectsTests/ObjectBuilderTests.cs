using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBuildObjectsTests
{
    [TestClass]
    public class BuildObjectsTests
    {
        [TestMethod]
        public void should_contain_an_object_definition_when_one_is_defined()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());
            Assert.IsTrue(objectBoss.Contains<SimpleObjectType>());
        }

        [TestMethod]
        public void should_contain_an_interface_definition_when_one_is_defined()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>();
            });
            Assert.IsTrue(objectBoss.Contains<ISimpleInterface>());
        }

        [TestMethod]
        public void should_contain_an_interface_using_a_derived_class()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>();
            });

            Assert.IsTrue(objectBoss.ContainsUsing<ISimpleInterface, SimpleObjectType>());
        }

        [TestMethod]
        public void should_contain_a_class_definition_defined_with_a_key()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>("testkey");
            });

            Assert.IsTrue(objectBoss.Contains("testkey"));
        }

        [TestMethod]
        public void should_return_parameterless_object_instance()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());
            var simpleObject = objectBoss.GetInstance<SimpleObjectType>();
            Assert.IsNotNull(simpleObject);
            Assert.IsInstanceOfType(simpleObject, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void should_inject_simple_object_type_into_object_with_one_dependency()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
                {
                    x.Add<SimpleObjectType>();
                    x.Add<ObjectWithOneDependency>();
                });

            var objectWithOneDependecy = objectBoss.GetInstance<ObjectWithOneDependency>();
            Assert.IsInstanceOfType(objectWithOneDependecy, typeof(ObjectWithOneDependency));
            Assert.IsNotNull(objectWithOneDependecy.SimpleObjectType);
            Assert.IsInstanceOfType(objectWithOneDependecy.SimpleObjectType, typeof(SimpleObjectType));
        }

        [TestMethod]
        public void should_wire_up_objects_for_complex_object_with_two_dependencies()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.Add<SimpleObjectType>();
                x.Add<ObjectWithOneDependency>();
                x.Add<ComplexObjectWithTwoDependencies>();
            });
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
            objectBoss.Configure(x =>
                {
                    x.AddUsing<ISimpleInterface, SimpleObjectType>();
                    x.AddUsing<ISimpleInterface, AnotherSimpleObject>();
                });

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
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>("object1");
                x.AddUsing<ISimpleInterface, AnotherSimpleObject>("object2");
            });

            var object1 = objectBoss.GetInstance<ISimpleInterface>("object1");
            var object2 = objectBoss.GetInstance<ISimpleInterface>("object2");
            Assert.IsNotNull(object1);
            Assert.IsTrue(object1.Name == "SimpleObject1");
            Assert.IsNotNull(object2);
            Assert.IsTrue(object2.Name == "SimpleObject2");

        }

        [TestMethod]
        public void should_retreive_an_object_based_purely_on_key()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>("object1");
            });

            var object1 = objectBoss.GetInstance("object1") as ISimpleInterface;
            Assert.IsNotNull(object1);
            Assert.IsTrue(object1.Name == "SimpleObject1");
        }

        [TestMethod]
        public void should_retrieve_a_default_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsingDefaultType<ISimpleInterface, SimpleObjectType>());

            var simpleType = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(simpleType);
            Assert.IsTrue(simpleType.Name == "SimpleObject1");
        }

        [TestMethod]
        public void should_retrieve_default_type_when_multiple_types_are_defined()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>();
                x.AddUsingDefaultType<ISimpleInterface, AnotherSimpleObject>();
            });

            var anotherSimpleObject = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsNotNull(anotherSimpleObject);
            Assert.IsTrue(anotherSimpleObject.Name == "SimpleObject2");
        }

        [TestMethod]
        public void should_get_multiple_instances_of_objects_that_are_not_defined_as_singleton()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>());
            var simpleObject1 = objectBoss.GetInstance<SimpleObjectType>();
            var simpleObject2 = objectBoss.GetInstance<SimpleObjectType>();
            Assert.IsTrue(objectBoss.GetRegisteredClassCount() == 2);
            Assert.IsNotNull(simpleObject1);
            Assert.IsNotNull(simpleObject2);
            Assert.IsTrue(simpleObject1.Id != simpleObject2.Id);
        }

        [TestMethod]
        public void should_add_object_as_singleton_and_get_the_same_instance_whenever_it_is_retrieved()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<SimpleObjectType>().Singleton());
            var simpleObject1 = objectBoss.GetInstance<SimpleObjectType>();
            var simpleObject2 = objectBoss.GetInstance<SimpleObjectType>();
            Assert.IsTrue(objectBoss.GetSingletonCount() == 2);
            Assert.IsNotNull(simpleObject1);
            Assert.IsNotNull(simpleObject2);
            Assert.IsTrue(simpleObject1.Id == simpleObject2.Id);
        }

        [TestMethod]
        public void should_be_able_to_add_an_interface_as_a_singleton()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.AddUsing<ISimpleInterface, SimpleObjectType>().Singleton());
            var simpleObject1 = objectBoss.GetInstance<ISimpleInterface>();
            var simpleObject2 = objectBoss.GetInstance<ISimpleInterface>();
            Assert.IsTrue(objectBoss.GetSingletonCount() == 2);
            Assert.IsNotNull(simpleObject1);
            Assert.IsNotNull(simpleObject2);
            Assert.IsTrue(simpleObject1.Id == simpleObject2.Id);
        }

        [TestMethod]
        public void should_inject_interfaces_as_dependencies()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsingDefaultType<ISimpleInterface, SimpleObjectType>();
                x.AddUsingDefaultType<IComplexInterface, ComplexObjectWithTwoDependencies>();
                x.Add<ComplexObjectWithInterfaceDependencies>();
            });

            var complexObjectWithInterfaceDependencies = objectBoss.GetInstance<ComplexObjectWithInterfaceDependencies>();
            Assert.IsNotNull(complexObjectWithInterfaceDependencies.ComplexInterface);
            Assert.IsNotNull(complexObjectWithInterfaceDependencies.SimpleInterface);
            Assert.IsInstanceOfType(complexObjectWithInterfaceDependencies.ComplexInterface, typeof(IComplexInterface));
            Assert.IsInstanceOfType(complexObjectWithInterfaceDependencies.SimpleInterface, typeof(ISimpleInterface));
        }

        [TestMethod]
        public void should_inject_singleton_interface_and_get_instance_singleton_interface_and_they_should_be_the_same()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsingDefaultType<ISimpleInterface, SimpleObjectType>();
                x.AddUsingDefaultType<IComplexInterface, ComplexObjectWithTwoDependencies>().Singleton();
                x.Add<ComplexObjectWithInterfaceDependencies>();
            });

            var complexObjectWithInterfaceDependencies = objectBoss.GetInstance<ComplexObjectWithInterfaceDependencies>();
            var complexInterface = objectBoss.GetInstance<IComplexInterface>();

            Assert.IsNotNull(complexObjectWithInterfaceDependencies);
            Assert.IsNotNull(complexInterface);
            Assert.IsTrue(complexInterface.Id == complexObjectWithInterfaceDependencies.ComplexInterface.Id);
        }

        [TestMethod]
        public void should_be_able_to_configure_a_custom_constructor_with_primitives()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<ObjectWithPrimitives>()
                .WithCustomConstructor(new Dictionary<string, object>()
                                       {
                                           {"isCool", true},
                                           {"myAge", 31}
                                       }));

            var primitiveObject = objectBoss.GetInstance<ObjectWithPrimitives>();

            Assert.AreEqual(primitiveObject.IsCool, true);
            Assert.AreEqual(primitiveObject.MyAge, 31);
        }

        [TestMethod]
        public void should_be_able_to_configure_a_constructor_with_primitives_and_inject_complex_type()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.Add<ObjectWithPrimitives>()
                    .WithCustomConstructor(new Dictionary<string, object>()
                                                                {
                                                                    {"isCool", true},
                                                                    {"myAge", 31}
                                                                });
                x.Add<ComplexObjectWithTwoDependencies>();

            });

            var primitiveObject = objectBoss.GetInstance<ObjectWithPrimitives>();

            Assert.AreEqual(primitiveObject.IsCool, true);
            Assert.AreEqual(primitiveObject.MyAge, 31);
            Assert.IsNotNull(primitiveObject.ComplexObject);
            Assert.IsNotNull(primitiveObject.ComplexObject.OneDependencyObject);
            Assert.IsNotNull(primitiveObject.ComplexObject.SimpleObjectType);
        }

        [TestMethod]
        public void should_be_able_to__do_custom_wiring()
        {
            var objectBoss = new ObjectBoss();

            var complexObject = new ComplexObjectWithTwoDependencies(new SimpleObjectType(),
                new ObjectWithOneDependency(new SimpleObjectType()))
            {
                Id = Guid.NewGuid(),
                OneDependencyObject = { Id = Guid.NewGuid() },
                SimpleObjectType = { Id = Guid.NewGuid() }
            };


            objectBoss.Configure(x =>
            {
                x.Add<ObjectWithPrimitives>()
                    .WithCustomConstructor(new Dictionary<string, object>()
                                                                {
                                                                    {"isCool", true},
                                                                    {"myAge", 31},
                                                                    {"complexObject",complexObject}
                                                                });
                x.Add<ComplexObjectWithTwoDependencies>();

            });

            var primitiveObject = objectBoss.GetInstance<ObjectWithPrimitives>();

            Assert.AreEqual(primitiveObject.IsCool, true);
            Assert.AreEqual(primitiveObject.MyAge, 31);

            Assert.AreEqual(primitiveObject.MyAge, 31);
            Assert.IsNotNull(primitiveObject.ComplexObject);
            Assert.IsNotNull(primitiveObject.ComplexObject.OneDependencyObject);
            Assert.IsNotNull(primitiveObject.ComplexObject.SimpleObjectType);

            Assert.AreEqual(complexObject.Id, primitiveObject.ComplexObject.Id);
            Assert.AreEqual(complexObject.OneDependencyObject.Id, primitiveObject.ComplexObject.OneDependencyObject.Id);
            Assert.AreEqual(complexObject.SimpleObjectType.Id, primitiveObject.ComplexObject.SimpleObjectType.Id);
        }

        [TestMethod]
        public void should_get_crushed_by_threads_and_work()
        {
            var simpleObjects = new List<SimpleObjectType>();
            var complexDependentObjects = new List<ComplexObjectWithInterfaceDependencies>();

            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
                {
                    x.AddUsingDefaultType<ISimpleInterface, SimpleObjectType>();
                    x.AddUsingDefaultType<IComplexInterface, ComplexObjectWithTwoDependencies>().Singleton();
                    x.Add<ComplexObjectWithInterfaceDependencies>();
                    x.Add<SimpleObjectType>();
                });

            Parallel.For(0, 10000, i =>
                {
                    simpleObjects.Add(objectBoss.GetInstance<SimpleObjectType>());
                    complexDependentObjects.Add(objectBoss.GetInstance<ComplexObjectWithInterfaceDependencies>());
                    Thread.Sleep(2);
                });
            Parallel.For(0, 10000, i =>
            {
                simpleObjects.Add(objectBoss.GetInstance<SimpleObjectType>());
                complexDependentObjects.Add(objectBoss.GetInstance<ComplexObjectWithInterfaceDependencies>());
                Thread.Sleep(4);
            });
            Parallel.For(0, 10000, i =>
            {
                simpleObjects.Add(objectBoss.GetInstance<SimpleObjectType>());
                complexDependentObjects.Add(objectBoss.GetInstance<ComplexObjectWithInterfaceDependencies>());
                Thread.Sleep(6);
            });

            Assert.IsTrue(simpleObjects.Count == 30000);
            Assert.IsTrue(complexDependentObjects.Count == 30000);
        }

        [TestMethod]
        public void should_be_able_to_bind_to_an_instance_using_interface_and_get_that_instance()
        {
            var simpleObject = new SimpleObjectType();
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.AddUsing<ISimpleInterface, SimpleObjectType>().BindTo(simpleObject);
            });
            
            var soi = objectBoss.GetInstance<ISimpleInterface>();
            Assert.AreEqual(simpleObject.Id, soi.Id);
        }

        [TestMethod]
        public void should_be_able_to_bind_to_a_concrete_instance_and_get_that_instance()
        {
            var simpleObject = new SimpleObjectType();
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.Add<SimpleObjectType>().BindTo(simpleObject);
            });

            var soc = objectBoss.GetInstance<SimpleObjectType>();
            Assert.AreEqual(simpleObject.Id, soc.Id);
        }
    }
}
