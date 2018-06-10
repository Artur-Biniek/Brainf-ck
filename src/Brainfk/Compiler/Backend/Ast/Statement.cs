namespace Brainfk.Compiler.Backend.Ast
{
    internal abstract class Statement
    {
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
