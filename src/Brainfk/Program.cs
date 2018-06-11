using System;
using System.Diagnostics;
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

            var c = new Compiler.Frontend.Compiler (tree);
            var exec = c.Compile ("InMemoryAssembly");

            var sw = new Stopwatch ();
            sw.Start ();

            exec ();
            //interpreter.Run ();
            sw.Stop ();

            Console.WriteLine ($"Program interpreted in {sw.Elapsed}.");
        }
    }
}