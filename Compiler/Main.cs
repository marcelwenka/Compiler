
using System;
using System.IO;
using System.Collections.Generic;

namespace Compiler
{
    public class Compiler
    {
        public static int lineno = 1;

        private static StreamWriter sw;

        private static int nr;

        public static List<int> errorLines = new List<int>();

        public static List<SyntaxTree> code = new List<SyntaxTree>();

        public static List<Symbol> symbols = new List<Symbol>();

        public static int Main(string[] args)
        {
            string file;
            FileStream source;

            Console.WriteLine("Code Generator for Mini");

            if (args.Length >= 1)
                file = args[0];
            else
            {
                Console.WriteLine("No source file specified");
                return 1;
            }

            try
            {
                var sr = new StreamReader(file);
                string str = sr.ReadToEnd();
                sr.Close();
                source = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return 1;
            }

            Scanner scanner = new Scanner(source);
            Parser parser = new Parser(scanner);

            var parsedProperly = parser.Parse();
            source.Close();

            if (errorLines.Count == 0)
            {
                if (!parsedProperly)
                    Console.WriteLine($"  syntax error at line: {lineno}");

                sw = new StreamWriter(file + ".ll");
                GenCode();
                sw.Close();
                Console.WriteLine("  compilation successful\n");
            }
            else
            {
                Console.WriteLine($"\n  {errorLines.Count} syntax errors detected\n");

                foreach (var err in errorLines)
                {
                    Console.WriteLine($"  syntax error at line: {err}");
                }
            }

            return errorLines.Count == 0 ? 0 : 2;
        }

        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }

        public static void EmitCode(string instr, params object[] args)
        {
            sw.WriteLine(instr, args);
        }

        public static string NewTemp()
        {
            return string.Format($"%t{++nr}");
        }

        private static void GenCode()
        {
            EmitCode("; prolog");
            EmitCode("@int_res = constant [15 x i8] c\"  Result:  %d\\0A\\00\"");
            EmitCode("@double_res = constant [16 x i8] c\"  Result:  %lf\\0A\\00\"");
            EmitCode("@end = constant [20 x i8] c\"\\0AEnd of execution\\0A\\0A\\00\"");
            EmitCode("declare i32 @printf(i8*, ...)");
            EmitCode("define void @main()");
            EmitCode("{");
            for (char c = 'a'; c <= 'z'; ++c)
            {
                EmitCode($"%i{c} = alloca i32");
                EmitCode($"store i32 0, i32* %i{c}");
                EmitCode($"%r{c} = alloca double");
                EmitCode($"store double 0.0, double* %r{c}");
            }
            EmitCode();

            for (int i = 0; i < code.Count; ++i)
            {
                code[i].GenCode();
                EmitCode();
            }
            EmitCode("}");
        }

        private static void GenProlog()
        {
            EmitCode(".assembly extern mscorlib { }");
            EmitCode(".assembly calculator { }");
            EmitCode(".method static void main()");
            EmitCode("{");
            EmitCode(".entrypoint");
            EmitCode(".try");
            EmitCode("{");
            EmitCode();

            EmitCode("// prolog");
            EmitCode(".locals init ( float64 temp )");
            for (char c = 'a'; c <= 'z'; ++c)
            {
                EmitCode($".locals init ( int32 _i{c} )");
                EmitCode($".locals init ( float64 _r{c} )");
            }
            EmitCode();
        }

        private static void GenEpilog()
        {
            EmitCode("leave EndMain");
            EmitCode("}");
            EmitCode("catch [mscorlib]System.Exception");
            EmitCode("{");
            EmitCode("callvirt instance string [mscorlib]System.Exception::get_Message()");
            EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
            EmitCode("leave EndMain");
            EmitCode("}");
            EmitCode("EndMain: ret");
            EmitCode("}");
        }
    }

    public abstract class Symbol : Expression
    {
        public readonly char type;
        public readonly string ident;
        public readonly int line;

        public Symbol(char t, string i)
        {
            type = t;
            ident = i;
            Compiler.symbols.Add(this);
        }

        public override char CheckType() { return type; }
    }

    class BoolIdent : Symbol
    {
        public BoolIdent(string i) : base('b', i) { }

        public override string GenCode()
        {
            throw new NotImplementedException();
        }
    }

    class IntIdent : Symbol
    {
        public IntIdent(string i) : base('i', i) { }

        public override string GenCode()
        {
            throw new NotImplementedException();
        }
    }

    class RealIdent : Symbol
    {
        public RealIdent(string i) : base('r', i) { }

        public override string GenCode()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SyntaxTree
    {
        public int line;

        public abstract char CheckType();

        public abstract string GenCode();
    }

    public abstract class Expression : SyntaxTree
    {
        public char type;
    }

    class WriteString : SyntaxTree
    {
        private string value;

        public WriteString(string v) { value = v; }

        public override char CheckType() { return ' '; }

        public override string GenCode()
        {
            Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([15 x i8]* @int_res to i8*), i32)");

            return null;
        }

    }

    class WriteExpression : SyntaxTree
    {
        private Expression exp;

        public WriteExpression(SyntaxTree e) { exp = (Expression)e; }

        public override char CheckType() { exp.CheckType(); return ' '; }

        public override string GenCode()
        {
            string t;
            t = exp.GenCode();
            if (exp.type == 'i')
                Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([15 x i8]* @int_res to i8*), i32 {t})");
            else
                Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([16 x i8]* @double_res to i8*), double {t})");
            return null;
        }

    }

    class Read : SyntaxTree
    {
        private readonly string ident;

        public Read(string i) { ident = i; }

        public override char CheckType() { return ' '; }

        public override string GenCode()
        {
            Compiler.EmitCode("call string [mscorlib]System.Console::ReadLine()");
            Compiler.EmitCode($"stloc.s {ident}");

            return null;
        }
    }

    class Assign : SyntaxTree
    {
        private string id;
        private SyntaxTree exp;

        public Assign(string i, SyntaxTree e, int l) { id = i; exp = e; line = l; }

        public override char CheckType()
        {
            exp.CheckType();
            //if (id[0] == '@' && exp.type != 'i')
            //    throw new ErrorException($"  line {line,3}:  semantic error - cannot convert real to int", false);
            return ' ';
        }

        public override string GenCode()
        {
            string t1, t2;
            t1 = exp.GenCode();
            if (id[0] == '$' /*&& exp.type != 'r'*/)
            {
                t2 = Compiler.NewTemp();
                Compiler.EmitCode($"{t2} = sitofp i32 {t1} to double");
            }
            else
                t2 = t1;
            Compiler.EmitCode("store {0} {1}, {0}* %{2}{3}", id[0] == '@' ? "i32" : "double", t2, id[0] == '@' ? 'i' : 'r', id[1]);
            return null;
        }
    }

    class Exit : SyntaxTree
    {
        public override char CheckType() { return ' '; }  // operacja pusta - typy są sprawdzane tylko dla wyrażeń

        public override string GenCode()
        {
            Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([20 x i8]* @end to i8*))");
            Compiler.EmitCode("ret void");
            return null;
        }
    }

    class BinaryOp : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public BinaryOp(SyntaxTree l, SyntaxTree r, Tokens k, int ln) { left = l; right = r; kind = k; line = ln; }

        public override char CheckType()
        {
            left.CheckType();
            right.CheckType();
            //type = left.type == 'i' && right.type == 'i' ? 'i' : 'r';
            //return type;
            return ' ';
        }

        public override string GenCode()
        {
            string tw, t1, t2, t3, t4, tt;

            t1 = left.GenCode();
            //if (left.type != type)
            {
                t2 = Compiler.NewTemp();
                Compiler.EmitCode($"{t2} = sitofp i32 {t1} to double");
            }
            //else
                t2 = t1;
            t3 = right.GenCode();
            //if (right.type != type)
            {
                t4 = Compiler.NewTemp();
                Compiler.EmitCode($"{t4} = sitofp i32 {t3} to double");
            }
            //else
                t4 = t3;

            tw = Compiler.NewTemp();
            //tt = type == 'i' ? "i32" : "double";
            switch (kind)
            {
                case Tokens.Plus:
            //        Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == 'i' ? "add i32" : "fadd double", t2, t4);
                    break;
                case Tokens.Minus:
            //        Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == 'i' ? "sub i32" : "fsub double", t2, t4);
                    break;
                case Tokens.Multiply:
            //        Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == 'i' ? "mul i32" : "fmul double", t2, t4);
                    break;
                case Tokens.Divide:
            //        Compiler.EmitCode("{0} = {1} {2}, {3}", tw, type == 'i' ? "sdiv i32" : "fdiv double", t2, t4);
                    break;
                default:
                    break;
                    //throw new ErrorException($"  line {line,3}:  internal gencode error", false);
            }
            return tw;
        }
    }

    class IntValue : SyntaxTree
    {
        private int val;

        public IntValue(int v) { val = v; }

        public override char CheckType()
        {
        //    type = 'i';
            return 'i';
        }

        public override string GenCode()
        {
            return val.ToString();
        }
    }

    class RealValue : SyntaxTree
    {
        private double val;

        public RealValue(double v) { val = v; }

        public override char CheckType()
        {
        //    type = 'r';
            return 'r';
        }

        public override string GenCode()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0###############}", val);
        }
    }
}
