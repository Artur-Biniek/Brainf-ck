using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Brainfk.Compiler.Backend.Ast;

namespace Brainfk.Compiler.Frontend {
    internal class Compiler {
        private readonly BlockStatement _block;
        private readonly Type LIST_TYPE;
        private readonly ConstructorInfo LIST_CTOR;
        private readonly MethodInfo LIST_ADD;
        private readonly MethodInfo LIST_GETITEM;
        private readonly MethodInfo List_SETITEM;
        private MethodInfo LIST_COUNT;
        private readonly MethodInfo CNSL_WRITECHAR;
        private readonly MethodInfo CNSL_READCHAR;

        private MethodInfo _getCharMehthodInfo;

        public ILGenerator _il { get; private set; }

        private TypeBuilder _type;

        public Compiler (BlockStatement block) {
            _block = block;

            LIST_TYPE = typeof (List<char>);

            LIST_CTOR = LIST_TYPE.GetConstructors () [0];
            LIST_ADD = LIST_TYPE.GetMethod ("Add");
            LIST_GETITEM = LIST_TYPE.GetProperty ("Item").GetMethod;
            List_SETITEM = LIST_TYPE.GetProperty ("Item").SetMethod;
            LIST_COUNT = LIST_TYPE.GetProperty ("Count").GetMethod;

            var cnslType = typeof (Console);
            CNSL_WRITECHAR = cnslType.GetMethod ("Write", new [] { typeof (char) });
            CNSL_READCHAR = cnslType.GetMethod ("ReadKey", Type.EmptyTypes);
        }

        public Action Compile (string name) {

            _type = CreateType (name);
            _getCharMehthodInfo = CreateGetCharMethod (_type);
            _il = CreateEntryMethod ();

            EmitBlock (_il, _block);

            _il.Emit (OpCodes.Ret);

            var ti = _type.CreateTypeInfo ();
            var t = ti.AsType ();

            return (Action) t.GetMethod ("Entry").CreateDelegate (typeof (Action));
        }

        private TypeBuilder CreateType (string name) {
            var asm = AssemblyBuilder.DefineDynamicAssembly (new AssemblyName (name), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule (name);
            return mod.DefineType ("CompiledProgram", TypeAttributes.Public | TypeAttributes.Class);
        }

        private MethodInfo CreateGetCharMethod (TypeBuilder typ) {
            var getc = typ.DefineMethod ("GetChar", MethodAttributes.Public | MethodAttributes.Static);
            getc.SetReturnType (typeof (char));

            var getcil = getc.GetILGenerator ();
            var tmpLoc = getcil.DeclareLocal (typeof (ConsoleKeyInfo));
            getcil.EmitCall (OpCodes.Call, CNSL_READCHAR, null);
            getcil.Emit (OpCodes.Stloc_0);
            getcil.Emit (OpCodes.Ldloca_S, tmpLoc);

            getcil.EmitCall (OpCodes.Call, typeof (ConsoleKeyInfo).GetProperty ("KeyChar").GetMethod, null);
            getcil.Emit (OpCodes.Ret);

            return getc.GetRuntimeBaseDefinition ();
        }

        private ILGenerator CreateEntryMethod () {

            var main = _type.DefineMethod ("Entry", MethodAttributes.Public | MethodAttributes.Static);
            var il = main.GetILGenerator ();

            var memory = il.DeclareLocal (LIST_TYPE);
            var pointer = il.DeclareLocal (typeof (int));

            il.Emit (OpCodes.Newobj, LIST_CTOR);
            il.Emit (OpCodes.Stloc_0);

            il.Emit (OpCodes.Ldloc_0);
            il.Emit (OpCodes.Ldc_I4_0);
            il.Emit (OpCodes.Dup);
            il.Emit (OpCodes.Stloc_1);
            il.EmitCall (OpCodes.Call, LIST_ADD, null);

            return il;
        }

        private void EmitBlock (ILGenerator il, BlockStatement block) {
            foreach (var st in block.Stmts) {
                EmitStatement (il, st);
            }
        }

        private void EmitStatement (ILGenerator il, Statement st) {
            switch (st) {
                case BlockStatement bs:
                    EmitBlock (il, bs);
                    break;

                case CharacterInStatement cis:
                    {
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.EmitCall (OpCodes.Call, _getCharMehthodInfo, null);
                        il.EmitCall (OpCodes.Call, List_SETITEM, null);

                        break;
                    }

                case CharacterOutStatement _:
                    {
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.EmitCall (OpCodes.Call, LIST_GETITEM, null);
                        il.EmitCall (OpCodes.Call, CNSL_WRITECHAR, null);

                        break;
                    }
                case DecrementStatement _:
                    {
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.EmitCall (OpCodes.Call, LIST_GETITEM, null);
                        il.Emit (OpCodes.Ldc_I4_1);
                        il.Emit (OpCodes.Sub);
                        il.EmitCall (OpCodes.Call, List_SETITEM, null);

                        break;
                    }
                case IncrementStatement _:
                    {
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldloc_1);
                        il.EmitCall (OpCodes.Call, LIST_GETITEM, null);
                        il.Emit (OpCodes.Ldc_I4_1);
                        il.Emit (OpCodes.Add);
                        il.EmitCall (OpCodes.Call, List_SETITEM, null);
                        break;
                    }
                case LoopStatement ls:
                    EmitLoop (il, ls);
                    break;

                case PointerDownStatement _:
                    {
                        il.Emit (OpCodes.Ldloc_1);
                        il.Emit (OpCodes.Ldc_I4_1);
                        il.Emit (OpCodes.Sub);
                        il.Emit (OpCodes.Stloc_1);
                        break;
                    }
                case PointerUpStatement _:
                    {
                        var whileCond = il.DefineLabel ();
                        var whileStart = il.DefineLabel ();

                        il.Emit (OpCodes.Ldloc_1);
                        il.Emit (OpCodes.Ldc_I4_1);
                        il.Emit (OpCodes.Add);
                        il.Emit (OpCodes.Stloc_1);

                        il.Emit (OpCodes.Br_S, whileCond);
                        il.MarkLabel (whileStart);

                        il.Emit (OpCodes.Ldloc_0);
                        il.Emit (OpCodes.Ldc_I4_0);
                        il.EmitCall (OpCodes.Call, LIST_ADD, null);

                        il.MarkLabel (whileCond);
                        il.Emit (OpCodes.Ldloc_1);
                        il.Emit (OpCodes.Ldloc_0);
                        il.EmitCall (OpCodes.Call, LIST_COUNT, null);

                        il.Emit (OpCodes.Bge_S, whileStart);

                        break;
                    }
            }
        }

        private void EmitLoop (ILGenerator il, LoopStatement ls) {
            var whileCond = il.DefineLabel ();
            var whileStart = il.DefineLabel ();

            il.Emit (OpCodes.Br, whileCond);
            il.MarkLabel (whileStart);

            EmitBlock (il, ls.Stmts);

            il.MarkLabel (whileCond);
            il.Emit (OpCodes.Ldloc_0);
            il.Emit (OpCodes.Ldloc_1);
            il.EmitCall (OpCodes.Call, LIST_GETITEM, null);

            il.Emit (OpCodes.Brtrue, whileStart);
        }
    }
}