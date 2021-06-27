using System.Collections;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class ObjectiveDataToken : IProtocolToken
    {

        public bool _isUpLeft { get; private set; }

        public void Write(UdpKit.UdpPacket packet)
        {
            packet.WriteBool(_isUpLeft);
        }

        public void Read(UdpKit.UdpPacket packet)
        {
            _isUpLeft = packet.ReadBool();
        }

        public void SetIsUpLeft(bool value)
        {
            _isUpLeft = value;
        }
    }
}
