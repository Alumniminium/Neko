using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace NKO
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
                return;

            var path = args[0];
            if(path.EndsWith(".nko",true,CultureInfo.InvariantCulture))
            {
                var dir = Path.GetFileNameWithoutExtension(path);
                var relativeOutPath = Path.GetDirectoryName(path);
                var outPath = Path.Combine(relativeOutPath,dir);
                Directory.CreateDirectory(outPath);
                Unpack(path,outPath);
            }
            else
            {
                var relativeOutPath = Path.GetDirectoryName(path);
                var outPath = Path.GetFileName(relativeOutPath) + ".nko";
                Pack(args,outPath);
            }
        }

        private static void Pack(string[] inputFiles, string outputFile)
        {
            Console.Write($"Packing {inputFiles.Length} files...");
            using var outputStream = File.OpenWrite(outputFile);
            using var compressionStream = new BrotliStream(outputStream, CompressionLevel.Optimal);
            using var outputWriter = new BinaryWriter(compressionStream);

            outputWriter.Write(inputFiles.Length); // First step, we write how many files are stored in our .nko file

            foreach (var inputFile in inputFiles)
            {
                using var inputStream = File.OpenRead(inputFile);

                outputWriter.Write(inputStream.Length); // Second step, we write how big the file is
                var fileName = Path.GetFileName(inputFile);
                outputWriter.Write(fileName); // Third step, we write the filename
                while (inputStream.Position != inputStream.Length) // Fourth step, we write the file
                {
                    var bytesLeft = inputStream.Length - inputStream.Position;
                    var bufferSize = Math.Min(bytesLeft, 1024 * 1024 * 16); // Give me a buffer of the smallest size, either 16mb or w/e is left
                    var buffer = new byte[bufferSize];

                    inputStream.Read(buffer, 0, buffer.Length);
                    compressionStream.Write(buffer, 0, buffer.Length);
                }
            }
            Console.WriteLine($" done!");
        }
        private static void Unpack(string inputFile, string outputPath)
        {
            Console.Write($"Unpacking {Path.GetFileName(inputFile)}...");
            using var inputStream = File.OpenRead(inputFile);
            using var compressionStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            var inputReader = new BinaryReader(compressionStream);

            var fileCount = inputReader.ReadInt32(); // First step, we read how many files are inside the .nko

            for (int i = 0; i < fileCount; i++)
            {
                var fileSize = inputReader.ReadInt64(); // Second step, we read the filesize of the file we want to extract
                var fileName = inputReader.ReadString(); // Third step, we read the filename of it
                using var outputStream = File.OpenWrite(Path.Combine(outputPath, fileName)); // Fourth Step, we create output stream and write it

                while (outputStream.Position != fileSize)
                {
                    var bytesLeft = fileSize - outputStream.Position;
                    var bufferSize = (int)Math.Min(bytesLeft, 1024 * 1024 * 16);
                    var buffer = new byte[bufferSize];
                    compressionStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, buffer.Length);
                }
            }
            Console.WriteLine($" done!");
        }
    }
}