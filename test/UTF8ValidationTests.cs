namespace tests;
using System.Text;
using SimdUnicode;
using System.Diagnostics;

public class Utf8SIMDValidationTests
{


    private const int NumTrials = 1000;
    private readonly RandomUtf8 generator = new RandomUtf8(1234, 1, 1, 1, 1);
    private static readonly Random rand = new Random();

    // int[] outputLengths = { 128, 192, 256, 320, 384, 448, 512, 576, 640, 704, 768, 832, 896, 960, 1024, 1088, 1152, 1216, 1280, 1344, 1408, 1472, 1536, 1600, 1664, 1728, 1792, 1856, 1920, 1984, 2048, 2112, 2176, 2240, 2304, 2368, 2432, 2496, 2560, 2624, 2688, 2752, 2816, 2880, 2944, 3008, 3072, 3136, 3200, 3264, 3328, 3392, 3456, 3520, 3584, 3648, 3712, 3776, 3840, 3904, 3968, 4032, 4096, 4160, 4224, 4288, 4352, 4416, 4480, 4544, 4608, 4672, 4736, 4800, 4864, 4928, 4992, 5056, 5120, 5184, 5248, 5312, 5376, 5440, 5504, 5568, 5632, 5696, 5760, 5824, 5888, 5952, 6016, 6080, 6144, 6208, 6272, 6336, 6400, 6464, 6528, 6592, 6656, 6720, 6784, 6848, 6912, 6976, 7040, 7104, 7168, 7232, 7296, 7360, 7424, 7488, 7552, 7616, 7680, 7744, 7808, 7872, 7936, 8000, 8064, 8128, 8192, 8256, 8320, 8384, 8448, 8512, 8576, 8640, 8704, 8768, 8832, 8896, 8960, 9024, 9088, 9152, 9216, 9280, 9344, 9408, 9472, 9536, 9600, 9664, 9728, 9792, 9856, 9920, 9984, 10000 };
    static int[] outputLengths = { 128, 256,345, 512,968, 1024, 1000 }; // Example lengths






    [Fact]
    public void TestGoodSequences()
    {
        string[] goodSequences = {
        "a",
        "\xC3\xB1",
        "\xE2\x82\xA1",
        "\xF0\x90\x8C\xBC",
        "\xC2\x80",
        "\xF0\x90\x80\x80",
        "\xEE\x80\x80",
        "\xEF\xBB\xBF"
    };

        foreach (var seq in goodSequences)
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes(seq);
            unsafe
            {
                fixed (byte* pInput = input)
                {
                    byte* scalarResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByteScalar(pInput, input.Length);
                    Assert.True((IntPtr)(pInput + input.Length) == (IntPtr)scalarResult,
                                $"Failure in Scalar function: SimdUnicode.UTF8.GetPointerToFirstInvalidByte.Sequence: {seq}");

                    byte* SIMDResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByte(pInput, input.Length);
                    Assert.True((IntPtr)(pInput + input.Length) == (IntPtr)SIMDResult,
                                $"Failure in SIMD function: Utf8Utility.GetPointerToFirstInvalidByte.Sequence: {seq}");
                }
            }
        }
    }

    [Fact]
    public void TestBadSequences()
    {
        string[] badSequences = {
        "\xC3\x28",
        "\xA0\xA1",
        "\xE2\x28\xA1",
        "\xE2\x82\x28",
        "\xF0\x28\x8C\xBC",
        "\xF0\x90\x28\xBC",
        "\xF0\x28\x8C\x28",
        "\xC0\x9F",
        "\xF5\xFF\xFF\xFF",
        "\xED\xA0\x81",
        "\xF8\x90\x80\x80\x80",
        "123456789012345\xED",
        "123456789012345\xF1",
        "123456789012345\xC2",
        "\xC2\x7F",
        "\xCE",
        "\xCE\xBA\xE1",
        "\xCE\xBA\xE1\xBD",
        "\xCE\xBA\xE1\xBD\xB9\xCF",
        "\xCE\xBA\xE1\xBD\xB9\xCF\x83\xCE",
        "\xCE\xBA\xE1\xBD\xB9\xCF\x83\xCE\xBC\xCE",
        "\xDF",
        "\xEF\xBF",
        "\x80",
        "\x91\x85\x95\x9E",
        "\x6C\x02\x8E\x18",
        "\x25\x5b\x6e\x2c\x32\x2c\x5b\x5b\x33\x2c\x34\x2c\x05\x29\x2c\x33\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5d\x2c\x35\x2e\x33\x2c\x39\x2e\x33\x2c\x37\x2e\x33\x2c\x39\x2e\x34\x2c\x37\x2e\x33\x2c\x39\x2e\x33\x2c\x37\x2e\x33\x2c\x39\x2e\x34\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x20\x01\x01\x01\x01\x01\x02\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x23\x0a\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x7e\x7e\x0a\x0a\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5d\x2c\x37\x2e\x33\x2c\x39\x2e\x33\x2c\x37\x2e\x33\x2c\x39\x2e\x34\x2c\x37\x2e\x33\x2c\x39\x2e\x33\x2c\x37\x2e\x33\x2c\x39\x2e\x34\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x5d\x01\x01\x80\x01\x01\x01\x79\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01",
        "[[[[[[[[[[[[[[[\x80\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x010\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01",
        "\x20\x0b\x01\x01\x01\x64\x3a\x64\x3a\x64\x3a\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x5b\x30\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x80\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01\x01",
    };

        foreach (var seq in badSequences)
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes(seq);
            unsafe
            {
                fixed (byte* pInput = input)
                {
                    byte* scalarResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByteScalar(pInput, input.Length);
                    Assert.True((IntPtr)(pInput + input.Length) == (IntPtr)scalarResult,
                                $"Failure in Scalar function: SimdUnicode.UTF8.GetPointerToFirstInvalidByte.Sequence: {seq}");

                    byte* SIMDResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByte(pInput, input.Length);
                    Assert.True((IntPtr)(pInput + input.Length) == (IntPtr)SIMDResult,
                                $"Failure in SIMD function: Utf8Utility.GetPointerToFirstInvalidByte.Sequence: {seq}");

                }
            }
        }
    }

    [Fact]
    public void Node48995Test()
    {
        byte[] bad = new byte[] { 0x80 };
        Assert.False(ValidateUtf8(bad));
    }

    [Fact]
    public void NoErrorTest()
    {
        foreach (int outputLength in outputLengths)
        {
            for (int trial = 0; trial < NumTrials; trial++)
            {
                byte[] utf8 = generator.Generate(outputLength);
                bool isValidUtf8 = ValidateUtf8(utf8);
                string utf8HexString = BitConverter.ToString(utf8).Replace("-", " ");
                Assert.True(isValidUtf8, $"Failure NoErrorTest. Sequence: {utf8HexString}");
            }
        }
    }

    [Fact]
    public void NoErrorTestASCII()
    {
        RunTestForByteLength(1);
    }

    [Fact]
    public void NoErrorTest1Byte()
    {
        RunTestForByteLength(1);
    }

    [Fact]
    public void NoErrorTest2Bytes()
    {
        RunTestForByteLength(2);
    }

    [Fact]
    public void NoErrorTest3Bytes()
    {
        RunTestForByteLength(3);
    }

    [Fact]
    public void NoErrorTest4Bytes()
    {
        RunTestForByteLength(4);
    }

    private void RunTestForByteLength(int byteLength)
    {
        // int[] outputLengths = { 128, 256, 512, 1024, 1000 }; // Example lengths
        foreach (int outputLength in outputLengths)
        {
            for (int trial = 0; trial < NumTrials; trial++)
            {
                byte[] utf8 = generator.Generate(outputLength, byteLength);
                bool isValidUtf8 = ValidateUtf8(utf8);
                Assert.True(isValidUtf8, $"Failure for {byteLength}-byte UTF8 of length {outputLength} in trial {trial}");
            }
        }
    }

    [Fact]
    public void HeaderBitsErrorTest()
    {
        for (int trial = 0; trial < NumTrials; trial++)
        {

            byte[] utf8 = generator.Generate(512);
            for (int i = 0; i < utf8.Length; i++)
            {
                if ((utf8[i] & 0b11000000) != 0b10000000) // Only process leading bytes
                {
                    byte oldByte = utf8[i];
                    utf8[i] = 0b11111000; // Forcing a header bits error
                    Assert.False(ValidateUtf8(utf8));
                    Assert.True(InvalidateUtf8(utf8, i));
                    utf8[i] = oldByte; // Restore the original byte
                }
            }
        }
    }

    [Fact]
    public void TooShortErrorTest()
    {
        foreach (int outputLength in outputLengths)
        {
            for (int trial = 0; trial < NumTrials; trial++)
            {
            byte[] utf8 = generator.Generate(outputLength);

                for (int i = 0; i < utf8.Length; i++)
                {
                    if ((utf8[i] & 0b11000000) == 0b10000000) // Only process continuation bytes
                    {
                        byte oldByte = utf8[i];
                        utf8[i] = 0b11100000; // Forcing a too short error
                        Assert.False(ValidateUtf8(utf8));
                        Assert.True(InvalidateUtf8(utf8, i));
                        utf8[i] = oldByte; // Restore the original byte
                    }
                }
            }
        }
        
    }

    [Fact]
    public void TooLongErrorTest()
    {
        foreach (int outputLength in outputLengths)
        {
            for (int trial = 0; trial < NumTrials; trial++)
            {
                byte[] utf8 = generator.Generate(outputLength);

                for (int i = 0; i < utf8.Length; i++)
                {
                    if ((utf8[i] & 0b11000000) != 0b10000000) // Only process leading bytes
                    {
                        byte oldByte = utf8[i];
                        utf8[i] = 0b10000000; // Forcing a too long error
                        Assert.False(ValidateUtf8(utf8));
                        Assert.True(InvalidateUtf8(utf8, i));
                        utf8[i] = oldByte; // Restore the original byte
                    }
                }
            }
        }
    }

    [Fact]
    public void OverlongErrorTest()
    {
        for (int trial = 0; trial < NumTrials; trial++)
        {
            foreach (int outputLength in outputLengths)
            {
                byte[] utf8 = generator.Generate(outputLength);


                for (int i = 0; i < utf8.Length; i++)
                {
                    if (utf8[i] >= 0b11000000) // Only non-ASCII leading bytes can be overlong
                    {
                        byte old = utf8[i];
                        byte secondOld = utf8[i + 1];

                        if ((old & 0b11100000) == 0b11000000) // two-bytes case, change to a value less or equal than 0x7f
                        {
                            utf8[i] = 0b11000000;
                        }
                        else if ((old & 0b11110000) == 0b11100000) // three-bytes case, change to a value less or equal than 0x7ff
                        {
                            utf8[i] = 0b11100000;
                            utf8[i + 1] = (byte)(utf8[i + 1] & 0b11011111);
                        }
                        else if ((old & 0b11111000) == 0b11110000) // four-bytes case, change to a value less or equal than 0xffff
                        {
                            utf8[i] = 0b11110000;
                            utf8[i + 1] = (byte)(utf8[i + 1] & 0b11001111);
                        }

                        Assert.False(ValidateUtf8(utf8));
                        Assert.True(InvalidateUtf8(utf8, i));

                        utf8[i] = old;
                        utf8[i + 1] = secondOld;
                    }
                }
            }
        }
    }


//  This might seems redundant with but it actually failed PR #17.
//  The issue is fixed in PR#18 but I thought it a good idea to formally cover it as further changes are possible.
    // [Fact]
    // public void TooShortTest2()
    // {
    //     for (int trial = 0; trial < NumTrials; trial++)
    //     {
    //         foreach (int outputLength in outputLengths)
    //         {
    //             byte[] oneUTFunit = generator.Generate( howManyUnits:1 ,byteCountInUnit: 2);            
    //             //  PrintHexAndBinary(oneUTFunit);
    //             byte[] utf8 = generator.Generate(outputLength,byteCountInUnit: 1);            
    //             // for (int i = 0; i < utf8.Length; i++)
    //             // {
    //                 // if ((utf8[i] & 0b11000000) == 0b10000000) // Only process continuation bytes
    //                 // {
    //                     byte oldByte = utf8[outputLength - 1];
    //                     utf8[outputLength -1] = oneUTFunit[0];//0b11000000; // Forcing a too short error at the very end
    //                     // PrintHexAndBinary(utf8);
    //                     Assert.False(ValidateUtf8(utf8));
    //                     utf8[outputLength -1] = oldByte; // Restore the original byte
                    
    //             // }
    //         }
    //     }
    // }

    public static IEnumerable<object[]> TestData()
    {
        // var utf8CharacterLengths = new[] {  2, 3, 4 }; // UTF-8 characters can be 1-4 bytes.
        return outputLengths.SelectMany(
            outputLength => Enumerable.Range(0, outputLength),
            (outputLength, position) => new object[] { outputLength, position });
    }


    [Theory]
    [MemberData(nameof(TestData))]
    public void TooShortTestEnd(int outputLength, int position)
    {
        byte[] oneUTFunit = generator.Generate(howManyUnits: 1, byteCountInUnit: 2);
        byte[] utf8 = generator.Generate(outputLength, byteCountInUnit: 1);

        byte oldByte = utf8[position];
        utf8[position] = oneUTFunit[0]; // Force a condition
        
        Assert.False(ValidateUtf8(utf8)); // Test the condition
        
        utf8[position] = oldByte; // Restore
    }

    // public static IEnumerable<object[]> InvalidTestData()
    // {
    //     var random = new Random();
    //     foreach (var length in outputLengths)
    //     {
    //         for (int trial = 0; trial < NumTrials; trial++)
    //         {
    //             int position = random.Next(length  - 3); // Choose a random position
    //             byte invalidByte = (byte)random.Next(0xF5, 0x100); // Generate a random invalid byte

    //             yield return new object[] { length, position, invalidByte };
    //         }
    //     }
    // }

    public static IEnumerable<object[]> InvalidTestData()
{
    var invalidBytes = Enumerable.Range(0xF5, 0x100 - 0xF5).Select(i => (byte)i).ToArray(); // 0xF5 to 0xFF
    foreach (var length in outputLengths)
    {
        for (int position = 0; position < length; position++)
        {
            foreach (var invalidByte in invalidBytes)
            {
                yield return new object[] { length, position, invalidByte };
            }
        }
    }
}


    //corresponds to condition 5.4.1 in the paper
    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void Invalid0xf50xff(int outputLength, int position, byte invalidByte)
    {
        byte[] utf8 = generator.Generate(outputLength,1);

        // Initialize utf8 with some valid data, if necessary
        // Array.Fill(utf8, (byte)0x20); // Filling with spaces for simplicity

        utf8[position] = invalidByte; // Inject an invalid byte at a random position

        // PrintHexAndBinary(utf8);

        Assert.False(ValidateUtf8(utf8)); // Expect the validation to fail due to the invalid byte
    }


            // Prints both hexadecimal and binary representations of a byte array
    static void PrintHexAndBinary(byte[] bytes)
    {
        // Convert to hexadecimal
        string hexRepresentation = BitConverter.ToString(bytes).Replace("-", " ");
        Console.WriteLine($"Hex: {hexRepresentation}");

        // Convert to binary
        string binaryRepresentation = string.Join(" ", Array.ConvertAll(bytes, byteValue => Convert.ToString(byteValue, 2).PadLeft(8, '0')));
        Console.WriteLine($"Binary: {binaryRepresentation}");
    }


    [Fact]
    public void TooLargeErrorTest()
    {
        foreach (int outputLength in outputLengths)
        {

            for (int trial = 0; trial < NumTrials; trial++)
            {

                byte[] utf8 = generator.Generate(outputLength);

                for (int i = 0; i < utf8.Length; i++)
                {
                    if ((utf8[i] & 0b11111000) == 0b11110000) // Only in 4-bytes case
                    {
                        byte old = utf8[i];
                        utf8[i] += (byte)(((utf8[i] & 0b100) == 0b100) ? 0b10 : 0b100);

                        Assert.False(ValidateUtf8(utf8));
                        Assert.True(InvalidateUtf8(utf8, i));
                        utf8[i] = old;
                    }
                }
            }
        }
    }

    [Fact]
    public void SurrogateErrorTest()
    {
        foreach (int outputLength in outputLengths)
        {
            for (int trial = 0; trial < NumTrials; trial++)
            {

                byte[] utf8 = generator.Generate(outputLength);

                for (int i = 0; i < utf8.Length; i++)
                {
                    if ((utf8[i] & 0b11110000) == 0b11100000) // Only in 3-bytes case
                    {
                        byte old = utf8[i];
                        byte secondOld = utf8[i + 1];

                        utf8[i] = 0b11101101; // Leading byte for surrogate
                        for (int s = 0x8; s < 0xf; s++)
                        {
                            utf8[i + 1] = (byte)((utf8[i + 1] & 0b11000011) | (s << 2));

                            Assert.False(ValidateUtf8(utf8));
                            Assert.True(InvalidateUtf8(utf8, i));
                        }

                        utf8[i] = old;
                        utf8[i + 1] = secondOld;
                    }
                }
            }
        }
    }

    [Fact]
    public void BruteForceTest()
    {
        foreach (int outputLength in outputLengths)
        {
            for (int i = 0; i < NumTrials; i++)
            {

                // Generate random UTF-8 sequence
                byte[] utf8 = generator.Generate(rand.Next(outputLength));

                Assert.True(ValidateUtf8(utf8), "Initial UTF-8 validation (primary) failed.");

                Assert.True(ValidateUtf8Fuschia(utf8), "Initial UTF-8 validation (Fuschia) failed.");

                // Perform random bit flips
                for (int flip = 0; flip < 1000; flip++)
                {
                    if (utf8.Length == 0)
                    {
                        break;
                    }

                    byte[] modifiedUtf8 = (byte[])utf8.Clone();
                    int byteIndex = rand.Next(modifiedUtf8.Length);
                    int bitFlip = 1 << rand.Next(8);
                    modifiedUtf8[byteIndex] ^= (byte)bitFlip;

                    // Validate the modified sequence with both methods
                    bool isValidPrimary = ValidateUtf8(modifiedUtf8);
                    bool isValidFuschia = ValidateUtf8Fuschia(modifiedUtf8);

                    // Ensure both methods agree on the validation result
                    Assert.Equal(isValidPrimary, isValidFuschia);
                }
            }
        }
    }

    // credit: based on code from Google Fuchsia (Apache Licensed)
    public static bool ValidateUtf8Fuschia(byte[] data)
    {
        int pos = 0;
        int len = data.Length;
        uint codePoint;

        while (pos < len)
        {
            byte byte1 = data[pos];
            if (byte1 < 0b10000000)
            {
                pos++;
                continue;
            }
            else if ((byte1 & 0b11100000) == 0b11000000)
            {
                if (pos + 2 > len) return false;
                if ((data[pos + 1] & 0b11000000) != 0b10000000) return false;

                codePoint = (uint)((byte1 & 0b00011111) << 6 | (data[pos + 1] & 0b00111111));
                if (codePoint < 0x80 || 0x7ff < codePoint) return false;
                pos += 2;
            }
            else if ((byte1 & 0b11110000) == 0b11100000)
            {
                if (pos + 3 > len) return false;
                if ((data[pos + 1] & 0b11000000) != 0b10000000) return false;
                if ((data[pos + 2] & 0b11000000) != 0b10000000) return false;

                codePoint = (uint)((byte1 & 0b00001111) << 12 | (data[pos + 1] & 0b00111111) << 6 | (data[pos + 2] & 0b00111111));
                if (codePoint < 0x800 || 0xffff < codePoint || (0xd7ff < codePoint && codePoint < 0xe000)) return false;
                pos += 3;
            }
            else if ((byte1 & 0b11111000) == 0b11110000)
            {
                if (pos + 4 > len) return false;
                if ((data[pos + 1] & 0b11000000) != 0b10000000) return false;
                if ((data[pos + 2] & 0b11000000) != 0b10000000) return false;
                if ((data[pos + 3] & 0b11000000) != 0b10000000) return false;

                codePoint = (uint)((byte1 & 0b00000111) << 18 | (data[pos + 1] & 0b00111111) << 12 | (data[pos + 2] & 0b00111111) << 6 | (data[pos + 3] & 0b00111111));
                if (codePoint < 0x10000 || 0x10ffff < codePoint) return false;
                pos += 4;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    // Check that all functions agree on the result when the input might be invalid.
    private bool InvalidateUtf8(byte[] utf8, int badindex)
    {
        unsafe
        {
            fixed (byte* pInput = utf8)
            {
                byte* scalarResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByteScalar(pInput, utf8.Length);
                int scalarOffset = (int)(scalarResult - pInput);
                byte* simdResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByte(pInput, utf8.Length);
                int simdOffset = (int)(simdResult - pInput);
                int utf16CodeUnitCountAdjustment, scalarCountAdjustment;
                byte* dotnetResult = DotnetRuntime.Utf8Utility.GetPointerToFirstInvalidByte(pInput, utf8.Length, out utf16CodeUnitCountAdjustment, out scalarCountAdjustment);
                int dotnetOffset = (int)(dotnetResult - pInput);
                if (scalarOffset != simdOffset)
                {
                    Console.WriteLine("Suprisingly, scalarResult != simdResult {0} != {1}, badindex = {2}, length = {3}", scalarOffset, simdOffset, badindex, utf8.Length);
                }
                if (dotnetOffset != simdOffset)
                {
                    Console.WriteLine("Suprisingly, dotnetOffset != simdResult {0} != {1}, badindex = {2}, length = {3}", dotnetOffset, simdOffset, badindex, utf8.Length);
                }
                return (scalarResult == simdResult) && (simdResult == dotnetResult);
            }
        }
    }

    // check that all methods agree that the result is valid
    private bool ValidateUtf8(byte[] utf8)
    {
        unsafe
        {
            fixed (byte* pInput = utf8)
            {
                byte* scalarResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByteScalar(pInput, utf8.Length);
                if (scalarResult != pInput + utf8.Length)
                {
                    return false;
                }

                byte* simdResult = SimdUnicode.UTF8.GetPointerToFirstInvalidByte(pInput, utf8.Length);
                if (simdResult != pInput + utf8.Length)
                {
                    return false;
                }

                return true;
            }
        }
    }
}