using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;

namespace MsmqInt.Mq
{
    public delegate void MqReceived<T>(T message);
    interface IMq : IDisposable
    {
        void Send<T>(T msg);
        void ReceiveAsync<T>(MqReceived<T> mqReceived);
        T Receive<T>(TimeSpan receiveTimeout);
        void Init();
    }
}
