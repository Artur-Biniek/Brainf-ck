using System;
using Brainfk.Compiler.Backend;
using Brainfk.Compiler.Frontend;

namespace Brainfk {
    class Program {
        static void Main (string[] args) {
            var prg = TestPrograms.Mandelbrot;

            //Console.WriteLine (prg);

            var lex = new Lexer (prg);
            var parser = new Parser (lex);
            var tree = parser.Parse ();

            //Console.WriteLine (tree);

            var interpreter = new Interpreter (tree);

            interpreter.Run ();
        }
    }
}