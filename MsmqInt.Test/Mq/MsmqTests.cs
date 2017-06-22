using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsmqInt.Mq;
using MsmqInt.Test.Mq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;

namespace MsmqInt.Mq.Tests
{
    [TestClass()]
    public class MsmqTests
    {
        string testQueuName = "test";
        string testHost = ".";
        Msmq msmq = null;
        Student student = new Student(12, "Ali");
        ManualResetEvent mre;

        [TestInitialize]
        public void TestFixture()
        {
            msmq = new Msmq(testHost, testQueuName);
            mre = new ManualResetEvent(false);
            try
            {
                MessageQueue.Delete(msmq.path2Queu);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("The queue does not exist o"));
            }
            msmq.Init();
            Assert.IsTrue(MessageQueue.Exists(msmq.path2Queu));
        }

        [TestMethod()]
        public void InitTest()
        {

            //partially tested in init
        }


        [TestMethod()]
        public void SendTest()
        {
            msmq.Send<Student>(student);
        }

        [TestMethod()]
        public void ReceiveTest()
        {
            SendTest();
            Student receivedStudent = msmq.Receive<Student>(TimeSpan.FromSeconds(1));
            Assert.AreEqual(receivedStudent.age, student.age);
            Assert.AreEqual(receivedStudent.name, student.name);
        }

        Student currentReceivedMsg;
        private void MqReceived(Student student)
        {
            currentReceivedMsg = student;
            mre.Set();
        }

        [TestMethod()]
        public void ReceiveAsyncTest()
        {
            SendTest();
            msmq.ReceiveAsync<Student>(MqReceived);
            mre.WaitOne();
            Assert.AreEqual(currentReceivedMsg.age, student.age);
            Assert.AreEqual(currentReceivedMsg.name, student.name);
        }
        [TestMethod()]
        public void DisposeTest()
        {
            //...
        }
    }
}