using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuildObjects;
using IBuildObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IHandleObjectsTests
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

