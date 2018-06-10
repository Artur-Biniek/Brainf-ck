using Brainfk.Compiler.Backend.Ast;
using System;
using System.Collections.Generic;

namespace Brainfk.Compiler.Backend
{

    /// <summary>
    /// program: block EOF;
    /// block: stmt*;
    /// stmt: loopStmt | simpleStmt;
    /// loopStmt: OPEN block CLSE;
    /// simpleStmt: PTUP | PTDN | INC | DEC | CIN | COUT;
    /// EVERYTHINGELSE -> skip();
    /// </summary>
    internal class Parser
    {
        private readonly IStream<Token> _tokens;

        public Parser(IStream<Token> tokens)
        {
            _tokens = tokens;
        }

        public BlockStatement Parse()
        {
            var block = Block();
            Match(Token.EOF);

            return block;
        }

        private void Match(Token tkn)
        {
            if (_tokens.LookAhead == tkn)
            {
                _tokens.Consume();
            }
            else
            {
                throw new System.Exception($"Expected {tkn} but found {_tokens.LookAhead}.");
            }
        }

        private BlockStatement Block()
        {
            var stmts = new List<Statement>();

            while (true)
            {
                switch (_tokens.LookAhead)
                {
                    case Token.OPEN:
                        stmts.Add(Loop());
                        break;

                    case Token.CIN:
                    case Token.COUT:
                    case Token.INC:
                    case Token.DEC:
                    case Token.PTUP:
                    case Token.PTDN:
                        stmts.Add(Simple());
                        break;

                    default:
                        return new BlockStatement(stmts);
                }
            }

        }

        private Statement Simple()
        {
            Statement stmt;
            switch (_tokens.LookAhead)
            {
                case Token.CIN: stmt = new CharacterInStatement(); Match(Token.CIN); break;
                case Token.COUT: stmt = new CharacterOutStatement(); Match(Token.COUT); break;
                case Token.INC: stmt = new IncrementStatement(); Match(Token.INC); break;
                case Token.DEC: stmt = new DecrementStatement(); Match(Token.DEC); break;
                case Token.PTUP: stmt = new PointerUpStatement(); Match(Token.PTUP); break;
                case Token.PTDN: stmt = new PointerDownStatement(); Match(Token.PTDN); break;
                default: throw new Exception($"Token {_tokens.LookAhead} came as complete surprise to me while parsing simple statement.");
            }
            return stmt;
        }
        private LoopStatement Loop()
        {
            Match(Token.OPEN);
            var loop = new LoopStatement(Block());
            Match(Token.CLSE);

            return loop;
        }
    }
}
