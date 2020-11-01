﻿using RakDotNet.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public class DownloadPropertyDataMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DownloadPropertyData;

        public ushort ZoneID;
        public ushort VendorMapID;

        public string OwnerName;
        public long OwnerObjID;

        public string SpawnName;

        public Vector3 SpawnPosition;

        public float MaxBuildHeight;

        public List<string> Paths;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write<long>(0);

            writer.Write<int>(25166); // Template ID

            writer.Write(ZoneID);
            writer.Write(VendorMapID);

            writer.Write<uint>(0);

            writer.Write<uint>(0); // String length - property name
            writer.Write<uint>(0); // String length - property description

            writer.Write((uint)OwnerName.Length);
            writer.WriteString(OwnerName, OwnerName.Length, true);

            writer.Write(OwnerObjID);

            writer.Write<uint>(0); // - type
            writer.Write<uint>(0); // - zone code
            writer.Write<uint>(0); // - minimum price
            writer.Write<uint>(1); // - rent duration

            writer.Write<ulong>(0); // - timestamp

            writer.Write<uint>(0);

            writer.Write<ulong>(0);

            writer.Write((uint)SpawnName.Length);
            writer.WriteString(SpawnName, SpawnName.Length, true);

            writer.Write<uint>(0); // String length
            writer.Write<uint>(0); // String length

            writer.Write<uint>(0); // - duration type
            writer.Write<uint>(0);
            writer.Write<uint>(1);

            writer.Write<sbyte>(0);

            writer.Write<ulong>(0);

            writer.Write<uint>(1);

            writer.Write<uint>(0); // String length

            writer.Write<ulong>(0);

            writer.Write<uint>(0);
            writer.Write<uint>(0);

            writer.Write(SpawnPosition.X);
            writer.Write(SpawnPosition.Y);
            writer.Write(SpawnPosition.Z);

            writer.Write(MaxBuildHeight);

            writer.Write<ulong>(0); // - timestamp

            writer.Write<sbyte>(0);

            writer.Write((uint)Paths.Count / 3);

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            foreach (var item in Paths)
            {
                if (float.TryParse(item, out float value))
                {
                    writer.Write(value);
                }
            }
        }
    }
}