using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Text.RegularExpressions;

namespace MsmqInt.Mq
{
    public class Msmq : IMq
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressOrHostName">use . or 127.0.0.1 for local</param>
        /// <param name="queuName"></param>

        string ipAddressOrHostName;
        string queuName;
        const string ipaddress = "(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        MessageQueue messageQueu;
        public string path2Queu;
        PeekCompletedEventHandler receiveEventHandler;
        public Msmq(string ipAddressOrHostName, string queuName)
        {
            this.ipAddressOrHostName = ipAddressOrHostName;
            this.queuName = queuName;
            path2Queu = FormatPathToQueue();
        }

        private bool QueueExists(string machineName, string queueName, string path)
        {
            bool isIpAddress = Regex.Match(machineName, ipaddress).Success;

            if (machineName == ".")
            {
                return MessageQueue.Exists(path);
            }

            // MessageQueue.Exists doesn't work for remote machines
            var results = MessageQueue.GetPrivateQueuesByMachine(machineName);
            return results.Any(q => q.QueueName == string.Format(@"private$\{0}", queueName));
        }

        public void Init()
        {
            if (IsLocal)
            {
                if (!QueueExists(ipAddressOrHostName, queuName, path2Queu))
                    MessageQueue.Create(path2Queu);
            }
            messageQueu = new MessageQueue(path2Queu);
        }

        public void Dispose()
        {
            messageQueu.Dispose();
        }

        public T Receive<T>(TimeSpan receiveTimeout)
        {
            Message msg = messageQueu.Receive(receiveTimeout);
            return (T)msg.Body;
        }
        public void ReceiveAsync<T>(MqReceived<T> mqReceived)
        {
            try
            {
                receiveEventHandler = (source, args) =>
                {
                    var queue = (MessageQueue)source;
                    using (Message msg = queue.EndPeek(args.AsyncResult))
                    {
                        XmlMessageFormatter formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
                        msg.Formatter = formatter;
                        queue.ReceiveById(msg.Id);
                        T tMsg = (T)msg.Body;
                        mqReceived(tMsg);

                    }
                    queue.BeginPeek();
                };

                messageQueu.PeekCompleted += receiveEventHandler;
                messageQueu.BeginPeek();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        bool IsLocal
        {
            get
            {
                return ipAddressOrHostName == "." || ipAddressOrHostName == "127.0.0.1";
            }
        }
        string FormatPathToQueue()
        {
            // local
            if (IsLocal)
            {
                return string.Format(@".\private$\{0}", queuName);
            }

            // remote
            bool isIpAddress = Regex.Match(ipAddressOrHostName, ipaddress).Success;
            string transport = isIpAddress ? "TCP" : "OS";
            return string.Format(@"FormatName:DIRECT={0}:{1}\private$\{2}", transport, ipAddressOrHostName, queuName);
        }

        public void Send<T>(T msg)
        {
            messageQueu.Send(msg);
        }
    }
}
