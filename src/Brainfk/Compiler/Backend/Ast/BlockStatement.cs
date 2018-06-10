using System.Collections.Generic;

namespace Brainfk.Compiler.Backend.Ast
{
    internal sealed class BlockStatement : Statement
    {
        public IEnumerable<Statement> Stmts { get; }

        public BlockStatement(IEnumerable<Statement> stmts)
        {
            Stmts = stmts;
        }

        public override string ToString()
        {
            return $"({string.Join(" ", Stmts)})";
        }
    }
}
