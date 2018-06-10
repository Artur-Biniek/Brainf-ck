namespace Brainfk.Compiler.Backend
{
    internal interface IStream<T>
    {
        T LookAhead { get; }

        void Consume();
    }
}
