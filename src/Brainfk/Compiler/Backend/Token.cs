namespace Brainfk.Compiler.Backend
{
    internal enum Token
    {
        EOF,        // End Of File
        PTDN,       // '<' 
        PTUP,       // '>'
        INC,        // '+'
        DEC,        // '-'
        CIN,        // ','
        COUT,       // '.'
        OPEN,       // '['
        CLSE,       // ']'
    }
}
