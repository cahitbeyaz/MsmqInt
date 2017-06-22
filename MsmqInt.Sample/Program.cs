using MsmqInt.Mq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsmqInt.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //local
            string testQueuName = "test";
            string testHost = ".";
            Msmq msmq = new Msmq(testHost, testQueuName);
            msmq.Init();
            msmq.Send<string>("hello world");
            string msgReceived = msmq.Receive<string>(TimeSpan.FromSeconds(5));

            Console.WriteLine(msgReceived);
            Console.ReadLine();


            //remote queu
            testHost = "10.28.32.2";
            Msmq msmqRemote = new Msmq(testHost, testQueuName);
            msmqRemote.Init();
            msmqRemote.Send<string>("hello world");

        }
    }
}
