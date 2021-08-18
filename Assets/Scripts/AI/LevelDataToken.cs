using System.Collections;
using UnityEngine;
using Bolt;

namespace ThePackt
{
    public class LevelDataToken : IProtocolToken
    {

        public int _level  { get; private set; }

        public void Write(UdpKit.UdpPacket packet)
        {
            packet.WriteInt(_level);
        }

        public void Read(UdpKit.UdpPacket packet)
        {
            _level = packet.ReadInt();
        }

        public void SetLevel(int value)
        {
            _level = value;
        }
    }
}

