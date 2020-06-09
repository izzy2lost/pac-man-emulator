using System;
using System.Collections.Generic;
using Xunit;

namespace JustinCredible.ZilogZ80.Tests
{
    public class DEC_MIX_Tests : BaseTest
    {
        public static IEnumerable<object[]> GetData()
        {
            var list = new List<object[]>();

            list.Add(new object[] { 0 });
            list.Add(new object[] { 1 });
            list.Add(new object[] { 2 });
            list.Add(new object[] { 27 });
            list.Add(new object[] { -33 });
            list.Add(new object[] { -62 });

            return list;
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test_DEC_MIX_NoFlags(int offset)
        {
            var rom = AssembleSource($@"
                org 00h
                DEC (IX {(offset < 0 ? '-' : '+')} {Math.Abs(offset)})
                HALT
            ");

            var registers = new CPURegisters()
            {
                IX = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477 + offset] = 0x44;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
                Flags = new ConditionFlags()
                {
                    Subtract = false,
                },
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x43, state.Memory[0x2477 + offset]);

            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.AuxCarry);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Subtract);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(4 + 23, state.Cycles);
            Assert.Equal(0x03, state.Registers.PC);
        }

        // TODO: This should detect overflow and not parity
        // TODO: Also detect Half-carry.
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test_DEC_MIX_ParityFlag(int offset)
        {
            var rom = AssembleSource($@"
                org 00h
                DEC (IX {(offset < 0 ? '-' : '+')} {Math.Abs(offset)})
                HALT
            ");

            var registers = new CPURegisters()
            {
                IX = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477 + offset] = 0x45;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
                Flags = new ConditionFlags()
                {
                    Subtract = false,
                },
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x44, state.Memory[0x2477 + offset]);

            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.AuxCarry);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Subtract);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(4 + 23, state.Cycles);
            Assert.Equal(0x03, state.Registers.PC);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test_DEC_MIX_SignFlag(int offset)
        {
            var rom = AssembleSource($@"
                org 00h
                DEC (IX {(offset < 0 ? '-' : '+')} {Math.Abs(offset)})
                HALT
            ");

            var registers = new CPURegisters()
            {
                IX = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477 + offset] = 0x81;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
                Flags = new ConditionFlags()
                {
                    Subtract = false,
                },
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x80, state.Memory[0x2477 + offset]);

            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.AuxCarry);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Subtract);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(4 + 23, state.Cycles);
            Assert.Equal(0x03, state.Registers.PC);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test_DEC_MIX_ZeroFlag(int offset)
        {
            var rom = AssembleSource($@"
                org 00h
                DEC (IX {(offset < 0 ? '-' : '+')} {Math.Abs(offset)})
                HALT
            ");

            var registers = new CPURegisters()
            {
                IX = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477 + offset] = 0x01;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
                Flags = new ConditionFlags()
                {
                    Subtract = false,
                },
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x00, state.Memory[0x2477 + offset]);

            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.AuxCarry);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Subtract);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(4 + 23, state.Cycles);
            Assert.Equal(0x03, state.Registers.PC);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test_DEC_MIX_NoCarryFlag(int offset)
        {
            var rom = AssembleSource($@"
                org 00h
                DEC (IX {(offset < 0 ? '-' : '+')} {Math.Abs(offset)})
                HALT
            ");

            var registers = new CPURegisters()
            {
                IX = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477 + offset] = 0x00;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
                Flags = new ConditionFlags()
                {
                    Subtract = false,
                },
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0xFF, state.Memory[0x2477 + offset]);

            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.AuxCarry);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Subtract);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(4 + 23, state.Cycles);
            Assert.Equal(0x03, state.Registers.PC);
        }
    }
}