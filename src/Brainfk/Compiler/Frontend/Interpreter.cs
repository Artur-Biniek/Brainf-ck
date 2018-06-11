using System;
using System.Linq;
using System.Collections.Generic;
using Brainfk.Compiler.Backend.Ast;

namespace Brainfk.Compiler.Frontend {
    internal sealed class Interpreter {
        private List<byte> _memory = new byte[128].ToList();
        private readonly BlockStatement _program;

        private int _p;
        private int __IP;

        public Interpreter (BlockStatement program) {
            _program = program;
        }

        public void Run () {
            Block (_program);
        }

        private void Block (BlockStatement block) {
            foreach (var st in block.Stmts) {
                Decode (st);
            }
        }

        private void Decode (Statement st) {
            switch (st) {
                case BlockStatement bs:
                    Block (bs);
                    break;
                case CharacterInStatement cis:
                    {
                        _memory[_p] = (byte) Console.ReadKey ().KeyChar;
                        break;
                    }
                case CharacterOutStatement _:
                    {
                        Console.Write ((char) _memory[_p]);
                        break;
                    }
                case DecrementStatement _:
                    {
                        _memory[_p]--;
                        break;
                    }
                case IncrementStatement _:
                    {
                        _memory[_p]++;
                        break;
                    }
                case LoopStatement ls:
                    Loop (ls);
                    break;
                case PointerDownStatement _:
                    {
                        _p--;
                        if (_p < 0) throw new InvalidOperationException ("Negative memory pointer");
                        break;
                    }
                case PointerUpStatement _:
                    {
                        _p++;
                        while (_p >= _memory.Count) {
                            _memory.Add (0);
                        }
                        break;
                    }
            }
        }

        private void Loop (LoopStatement loop) {
            while (_memory[_p] != 0) {
                Block (loop.Stmts);
            }
        }
    }
}