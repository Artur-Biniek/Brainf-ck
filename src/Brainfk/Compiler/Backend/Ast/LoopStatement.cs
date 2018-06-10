namespace Brainfk.Compiler.Backend.Ast
{
    internal sealed class LoopStatement : Statement
    {
        public BlockStatement Stmts { get; }

        public LoopStatement(BlockStatement stmts)
        {
            Stmts = stmts;
        }

        public override string ToString()
        {
            return $"(LOOP {Stmts})";
        }
    }
}
