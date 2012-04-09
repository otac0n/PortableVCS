namespace PortableVCSFormat.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class DescriptorTests
    {
        [Test]
        public void ReadFrom_FromANullStream_ThrowsException()
        {
            var stream = (Stream)null;

            Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void ReadFrom_FromAnEmptyStream_ThrowsException()
        {
            using (var stream = new MemoryStream())
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadFrom_FromAStreamWithAValidFolderDescriptor_ReturnsTheDescriptor()
        {
            var descriptorBytes = Encoding.ASCII.GetBytes("28\0e82fe33199f25c242213ada825358e91c4261753\0t\03\0foo\0");
            var expected = new Descriptor("e82fe33199f25c242213ada825358e91c4261753", DescriptorType.Folder, "foo");

            using (var stream = new MemoryStream(descriptorBytes))
            {
                var descriptor = Descriptor.ReadFrom(stream);

                Assert.That(descriptor, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ReadFrom_FromAStreamWithAValidFileDescriptor_ReturnsTheDescriptor()
        {
            var descriptorBytes = Encoding.ASCII.GetBytes("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0foo\0");
            var expected = new Descriptor("e82fe33199f25c242213ada825358e91c4261753", DescriptorType.File, "foo");

            using (var stream = new MemoryStream(descriptorBytes))
            {
                var descriptor = Descriptor.ReadFrom(stream);

                Assert.That(descriptor, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ReadFrom_FromAStreamWithAnInvalidDescriptorType_ThrowsException()
        {
            var descriptorBytes = Encoding.ASCII.GetBytes("28\0e82fe33199f25c242213ada825358e91c4261753\0q\03\0foo\0");

            using (var stream = new MemoryStream(descriptorBytes))
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadFrom_FromAStreamWithNonHexadecimalCharacters_ThrowsException()
        {
            var descriptorBytes = Encoding.ASCII.GetBytes("zz\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0foo\0");
            var expected = new Descriptor("e82fe33199f25c242213ada825358e91c4261753", DescriptorType.File, "foo");

            using (var stream = new MemoryStream(descriptorBytes))
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadFrom_FromAStreamWithAZeroLengthName_ThrowsException()
        {
            var descriptorBytes = Encoding.ASCII.GetBytes("0\0\0t\03\0foo\0");

            using (var stream = new MemoryStream(descriptorBytes))
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCase("2")]
        [TestCase("28")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c426175")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0fo")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0foo")]
        public void ReadFrom_WithAnEOFAtVariousPlaces_ThrowsException(string value)
        {
            var descriptorBytes = Encoding.ASCII.GetBytes(value);

            using (var stream = new MemoryStream(descriptorBytes))
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        [TestCase("28\x01e82fe33199f25c242213ada825358e91c4261753\0b\03\0foo\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\x01b\03\0foo\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\x013\0foo\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\x01foo\0")]
        [TestCase("28\0e82fe33199f25c242213ada825358e91c4261753\0b\03\0foo\x01")]
        public void ReadFrom_WithANonNullByteWhereNullBytesAreExpected_ThrowsException(string value)
        {
            var descriptorBytes = Encoding.ASCII.GetBytes(value);

            using (var stream = new MemoryStream(descriptorBytes))
            {
                Assert.That(() => Descriptor.ReadFrom(stream), Throws.InstanceOf<InvalidOperationException>());
            }
        }

        [Test]
        public void ReadFrom_StreamThatHasBeenWrittenTo_RoundTripsTheValue()
        {
            var expected = new Descriptor("e82fe33199f25c242213ada825358e91c4261753", DescriptorType.File, "foo");

            using (var stream = new MemoryStream())
            {
                expected.WriteTo(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var actual = Descriptor.ReadFrom(stream);
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
