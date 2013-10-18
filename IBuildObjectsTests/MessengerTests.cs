using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IBuildObjectsTests
{
    [TestClass]
    public class MessengerTests
    {
        [TestMethod]
        public void should_send_and_receive_messages()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<ReceiveMessage>().ForMessagging());

            var recieveMessage = objectBoss.GetInstance<ReceiveMessage>();

            Assert.IsTrue(recieveMessage.Count == 1);
            objectBoss.SendMessage(new AddMessage() { HowMuchToAdd = 5 });
            Assert.IsTrue(recieveMessage.Count == 6);
        }

        [TestMethod]
        public void should_only_receive_messages_if_registered_to_receive_messages()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x =>
            {
                x.Add<ReceiveMessage>().ForMessagging();
                x.Add<DoesNotReceiveMessage>();
            });

            var recieveMessage = objectBoss.GetInstance<ReceiveMessage>();
            var doesNotReceive = objectBoss.GetInstance<DoesNotReceiveMessage>();

            Assert.IsTrue(recieveMessage.Count == 1);
            Assert.IsTrue(doesNotReceive.Count == 1);

            objectBoss.SendMessage(new AddMessage() { HowMuchToAdd = 5 });

            Assert.IsTrue(recieveMessage.Count == 6);
            Assert.IsTrue(doesNotReceive.Count == 1);
        }

        [TestMethod]
        public void when_handler_weak_reference_is_gone_should_no_longer_receive_messages()
        {
            var objectBoss = new ObjectBoss();
            objectBoss.Configure(x => x.Add<ReceiveMessage>().ForMessagging());

            var recieveMessage = objectBoss.GetInstance<ReceiveMessage>();

            Assert.IsTrue(recieveMessage.Count == 1);
            objectBoss.SendMessage(new AddMessage() { HowMuchToAdd = 5 });
            Assert.IsTrue(recieveMessage.Count == 6);
            objectBoss.UnregisterTypeForMessaging(recieveMessage.GetType());

            Assert.IsNotNull(recieveMessage);
            objectBoss.SendMessage(new AddMessage() { HowMuchToAdd = 5 });
            Assert.IsTrue(recieveMessage.Count == 6);
        }
    }
}

