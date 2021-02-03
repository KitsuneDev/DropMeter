﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropMeter.PluginInterface
{
    [Serializable]
    public delegate void MessageHandler<TEventArgs>(string messageId, TEventArgs e);
    //public delegate void HandleMessage(string message);
    public interface DMPlugin
    {
        string DisplayName { get; }

        string Slug { get; }


        //TODO: Pass Plugin's Helper
        bool Initialize(IPluginHelper helper);

        void Terminate();
    }

    public class IMessageReceivedData
    {
        public string id { get; set; }
        public object data { get; set; }

    }
    public interface IPluginHelper
    {
        /// <summary>
        /// Sends the message from PLG to the VIEWs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        void EmitMessage(string id, object parameters);
        /// <summary>
        /// Messages sent from VIEWs to PLG.
        /// </summary>
        event MessageHandler<IMessageReceivedData> OnMessageReceived;
    }
}