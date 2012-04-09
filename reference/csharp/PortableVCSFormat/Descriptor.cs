namespace PortableVCSFormat
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Specifies whether a <see cref="Descriptor"/> represents a file or a folder.
    /// </summary>
    public enum DescriptorType
    {
        /// <summary>
        /// Indicates that a <see cref="Descriptor"/> represents a file.
        /// </summary>
        File,

        /// <summary>
        /// Indicates that a <see cref="Descriptor"/> represents a folder.
        /// </summary>
        Folder,
    }

    /// <summary>
    /// Describes the filename and contents of a file or folder.
    /// </summary>
    public class Descriptor
    {
        private readonly string objectName;
        private readonly DescriptorType descriptorType;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Descriptor"/> class.
        /// </summary>
        /// <param name="objectName">The object name of the contents of the new descriptor.</param>
        /// <param name="descriptorType">The type of the new descriptor.</param>
        /// <param name="name">The name of the new descriptor.</param>
        public Descriptor(string objectName, DescriptorType descriptorType, string name)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                throw new ArgumentNullException("objectName");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.objectName = objectName;
            this.descriptorType = descriptorType;
            this.name = name;
        }

        /// <summary>
        /// Gets the object name of the contents of this descriptor.
        /// </summary>
        public string ObjectName
        {
            get { return this.objectName; }
        }

        /// <summary>
        /// Gets a value indicating the type of this descriptor.
        /// </summary>
        public DescriptorType DescriptorType
        {
            get { return this.descriptorType; }
        }

        /// <summary>
        /// Gets the name of this descriptor.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Descriptor"/> are equal.
        /// </summary>
        /// <param name="left">A <see cref="Descriptor"/>.</param>
        /// <param name="right">A <see cref="Descriptor"/>.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> represent the same descriptor; otherwise, false.</returns>
        public static bool operator ==(Descriptor left, Descriptor right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Descriptor"/> are not equal.
        /// </summary>
        /// <param name="left">A <see cref="Descriptor"/>.</param>
        /// <param name="right">A <see cref="Descriptor"/>.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> do not represent the same descriptor; otherwise, false.</returns>
        public static bool operator !=(Descriptor left, Descriptor right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Reads a descriptor from a stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> from which to read the descriptor.</param>
        /// <returns>The new descriptor.</returns>
        public static Descriptor ReadFrom(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            var objectNameLength = ReadLength(stream);
            var objectName = ReadText(stream, objectNameLength);
            var descriptorType = ReadType(stream);
            var nameLength = ReadLength(stream);
            var name = ReadText(stream, nameLength);

            return new Descriptor(objectName, descriptorType, name);
        }

        /// <summary>
        /// Writes a descriptor to a stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to which to write the descriptor.</param>
        public void WriteTo(Stream stream)
        {
            byte[] buffer;

            buffer = Encoding.ASCII.GetBytes(Convert.ToString(Encoding.UTF8.GetByteCount(this.objectName), 16));
            stream.Write(buffer, 0, buffer.Length);
            stream.WriteByte(0);

            buffer = Encoding.UTF8.GetBytes(this.objectName);
            stream.Write(buffer, 0, buffer.Length);
            stream.WriteByte(0);

            stream.WriteByte((byte)(this.DescriptorType == DescriptorType.File ? 'b' : 't'));
            stream.WriteByte(0);

            buffer = Encoding.ASCII.GetBytes(Convert.ToString(Encoding.UTF8.GetByteCount(this.name), 16));
            stream.Write(buffer, 0, buffer.Length);
            stream.WriteByte(0);

            buffer = Encoding.UTF8.GetBytes(this.name);
            stream.Write(buffer, 0, buffer.Length);
            stream.WriteByte(0);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Descriptor"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Descriptor"/>.</param>
        /// <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Descriptor"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Descriptor;
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return
                this.objectName == other.objectName &&
                this.descriptorType == other.descriptorType &&
                this.name == other.name;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Descriptor"/>.</returns>
        public override int GetHashCode()
        {
            int hash = 0x51ED270B;
            hash = (hash * -0x25555529) + this.objectName.GetHashCode();
            hash = (hash * -0x25555529) + this.descriptorType.GetHashCode();
            hash = (hash * -0x25555529) + this.name.GetHashCode();

            return hash;
        }

        private static int ReadLength(Stream stream)
        {
            var result = new StringBuilder();
            int c;
            while ((c = stream.ReadByte()) > 0)
            {
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'F') ||
                    (c >= 'a' && c <= 'f'))
                {
                    result.Append((char)c);
                }
                else
                {
                    throw new InvalidOperationException("Read non-hexadecimal digit.");
                }
            }

            if (c == -1)
            {
                throw new InvalidOperationException("EOF reached while reading descriptor.");
            }

            var length = Convert.ToInt32(result.ToString(), 16);
            if (length == 0)
            {
                throw new InvalidOperationException("Read zero length for name.");
            }

            return length;
        }

        private static string ReadText(Stream stream, int length)
        {
            var buffer = new byte[length];

            int read = stream.Read(buffer, 0, length);
            if (read != length)
            {
                throw new InvalidOperationException("EOF reached while reading descriptor.");
            }

            AssertNullByte(stream);

            return Encoding.UTF8.GetString(buffer);
        }

        private static DescriptorType ReadType(Stream stream)
        {
            var type = stream.ReadByte();
            if (type == -1)
            {
                throw new InvalidOperationException("EOF reached while reading descriptor.");
            }

            AssertNullByte(stream);

            switch (type)
            {
                case (int)'T':
                case (int)'t': return DescriptorType.Folder;
                case (int)'B':
                case (int)'b': return DescriptorType.File;
                default:
                    throw new InvalidOperationException("Unknown descriptor type read.");
            }
        }

        private static void AssertNullByte(Stream stream)
        {
            var next = stream.ReadByte();
            if (next == -1)
            {
                throw new InvalidOperationException("EOF reached while reading descriptor.");
            }
            else if (next != 0)
            {
                throw new InvalidOperationException("Expected null byte, got non-zero byte.");
            }
        }
    }
}
