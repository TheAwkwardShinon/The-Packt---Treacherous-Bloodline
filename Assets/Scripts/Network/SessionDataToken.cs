using System.Collections;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class SessionDataToken : IProtocolToken
    {

        public string _sessionName { get; private set; }

        public void Write(UdpKit.UdpPacket packet)
        {
            packet.WriteString(_sessionName);
        }

        public void Read(UdpKit.UdpPacket packet)
        {
            _sessionName = packet.ReadString();
        }

        public void SetSessionName(string value)
        {
            _sessionName = value;
        }
    }
}
