﻿using RakDotNet.IO;

namespace Uchu.World
{
    public class PropertyContentsFromClient : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PropertyContentsFromClient;

        bool queryDB = false;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
            {
                queryDB = reader.ReadBit();
            }
        }
    }
}