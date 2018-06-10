namespace Brainfk.Compiler.Backend
{
    internal class Lexer : IStream<Token>
    {
        private readonly string _program;
        private int _p;

        public Token LookAhead
        {
            get
            {
                while (true)
                {
                    if (_p >= _program.Length) return Token.EOF;

                    switch (_program[_p])
                    {
                        case '<': return Token.PTDN;
                        case '>': return Token.PTUP;
                        case '-': return Token.DEC;
                        case '+': return Token.INC;
                        case ',': return Token.CIN;
                        case '.': return Token.COUT;
                        case '[': return Token.OPEN;
                        case ']': return Token.CLSE;
                    }

                    _p++;
                }
            }
        }

        public Lexer(string program)
        {
            _program = program;
            _p = 0;
        }

        public void Consume()
        {
            _p++;
        }
    }
}
