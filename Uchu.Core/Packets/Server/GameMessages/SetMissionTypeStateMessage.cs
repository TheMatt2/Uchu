using RakDotNet;

namespace Uchu.Core
{
    public class SetMissionTypeStateMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0353;

        public MissionLockState LockState { get; set; } = MissionLockState.New;
        public string Subtype { get; set; }
        public string Type { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            var hasState = LockState != MissionLockState.New;

            stream.WriteBit(hasState);

            if (hasState)
                stream.WriteInt((int) LockState);

            stream.WriteUInt((uint) Subtype.Length);
            stream.WriteString(Subtype, Subtype.Length);
            stream.WriteUInt((uint) Type.Length);
            stream.WriteString(Type, Type.Length);
        }
    }
}