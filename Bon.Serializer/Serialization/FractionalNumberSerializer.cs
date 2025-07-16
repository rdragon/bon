using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bon.Serializer.Serialization
{
    public static class FractionalNumberSerializer
    {
        private const byte ZERO = 0;
        private const byte FLOAT = 253;
        private const byte DOUBLE = 252;

        public static void Write(BinaryWriter writer, double? value)
        {
            if (value is not { } number)
            {
                writer.Write(NativeWriter.NULL);
            }
            else if (number == 0)
            {
                writer.Write(ZERO);
            }
            else
            {
                writer.Write(DOUBLE);
                writer.Write(number);
            }
        }

        public static void Write(BinaryWriter writer, float? value)
        {
            if (value is not { } number)
            {
                writer.Write(NativeWriter.NULL);
            }
            else if (number == 0)
            {
                writer.Write(ZERO);
            }
            else
            {
                writer.Write(FLOAT);
                writer.Write(number);
            }
        }

        public static double? Read(BinaryReader reader)
        {
            return reader.ReadByte() switch
            {
                ZERO => 0,
                FLOAT => reader.ReadSingle(),
                DOUBLE => reader.ReadDouble(),
                _ => null
            };
        }
    }
}
