
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public class Compiler
    {
        public static int lineno = 1;

        private static StreamWriter sw;

        private static int tempnr;

        public static List<int> syntaxErrors = new List<int>();

        public static List<SyntaxTree> code = new List<SyntaxTree>();

        public static List<Symbol> symbols = new List<Symbol>();

        /// <summary>
        /// Return value:
        ///     0 - compiled successfully
        ///     1 - file error (missing argument or unable to open)
        ///     2 - syntax error
        ///     3 - semantic error
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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

            if (syntaxErrors.Count > 0)
            {
                Console.WriteLine($"\n  {syntaxErrors.Count} syntax errors detected.");

                foreach (var line in syntaxErrors)
                    Console.WriteLine($"  Syntax error at line: {line}");

                return 2;
            }

            if (!parsedProperly)
            {
                Console.WriteLine($"  Syntax error at line: {lineno}");
                return 2;
            }

            if (!CheckSymbols() | !CheckCode()) // purpously used | instead of || to call both functions and avoid short-circuiting
                return 3;

            sw = new StreamWriter(file + ".il");
            GenProlog();
            GenSymbols();
            GenCode();
            GenEpilog();
            sw.Close();
            Console.WriteLine("  compilation successful\n");

            return 0;
        }

        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }

        public static string NewTemp(char type)
        {
            if (type == 'd')
                EmitCode($".locals init (float64 t{++tempnr})");
            else if (type == 'i')
                EmitCode($".locals init (int32 t{++tempnr})");
            else if (type == 'b')
                EmitCode($".locals init (int32 t{++tempnr})");
            else if (type == 's')
                EmitCode($".locals init (string t{++tempnr})");

            return $"t{tempnr}";
        }

        private static bool CheckSymbols()
        {
            bool proper = true;

            foreach (var symbol in symbols.GroupBy(x => x.ident).Where(g => g.Count() > 1))
            {
                proper = false;
                Console.WriteLine($"  Semantic error at line {symbol.First().line}: Two idents defined with the same name.");
            }

            return proper;
        }

        private static bool CheckCode()
        {
            bool proper = true;

            foreach (var tree in code)
            {
                try
                {
                    tree.Check();
                }
                catch (Exception e)
                {
                    proper = false;
                    Console.WriteLine(e.Message);
                }
            }

            return proper;
        }

        private static void GenCode()
        {
            for (int i = 0; i < code.Count; ++i)
            {
                code[i].GenCode();
                EmitCode();
            }
        }

        private static void GenSymbols()
        {
            foreach (var symbol in symbols)
            {
                switch (symbol.type)
                {
                    case 'b':
                        EmitCode($".locals init (int32 {symbol.ident})");
                        break;
                    case 'd':
                        EmitCode($".locals init (float64 {symbol.ident})");
                        break;
                    case 'i':
                        EmitCode($".locals init (int32 {symbol.ident})");
                        break;
                }
            }
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

        public static void AddNewNode(SyntaxTree tree)
        {
            code.Add(tree);
        }

        public static void AddNewSymbol(Symbol symbol)
        {
            symbols.Add(symbol);
        }
    }

    public abstract class SyntaxTree
    {
        public char type;
        public int line;

        public abstract void Check();

        public abstract string GenCode();
    }

    public class Symbol : SyntaxTree
    {
        public readonly string ident;

        public Symbol(int ln, string i)
        {
            line = ln;
            type = ' ';
            ident = i;
        }

        public Symbol(int ln, char t, string i)
        {
            line = ln;
            type = t;
            ident = i;
        }

        public override string GenCode()
        {
            return ident;
        }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: ident undeclared");

            type = Compiler.symbols.First(x => x.ident == ident).type;
        }
    }

    class WriteString : SyntaxTree
    {
        private string value;

        public WriteString(int ln, string v) { line = ln; value = v; }

        public override void Check() { }

        public override string GenCode()
        {
            Compiler.EmitCode($"ldstr {value}");
            Compiler.EmitCode($"call void [mscorlib]System.Console::Write(string)");

            return null;
        }
    }

    class WriteExpression : SyntaxTree
    {
        private SyntaxTree exp;

        public WriteExpression(int ln, SyntaxTree e) { line = ln; exp = e; }

        public override void Check() { exp.Check(); }

        public override string GenCode()
        {
            var ident = exp.GenCode();
            if (exp.type == 'i')
            {
                Compiler.EmitCode($"ldloc {ident}");
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(int32)");
            }
            else if (exp.type == 'd')
            {
                Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
                Compiler.EmitCode("ldstr \"{0:0.000000}\"");
                Compiler.EmitCode($"ldloc {ident}");
                Compiler.EmitCode("box [mscorlib]System.Double");
                Compiler.EmitCode("call string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider, string, object)");
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(string)");
            }
            else if (exp.type == 'b')
            {
                // todo write expression type boolean
            }

            return null;
        }
    }

    class Read : SyntaxTree
    {
        private readonly string ident;
        private char identType;

        public Read(int ln, string id) { line = ln; ident = id; }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: {ident} undeclared.");

            identType = Compiler.symbols.First(x => x.ident == ident).type;
        }

        public override string GenCode()
        {
            Compiler.EmitCode("call string [mscorlib]System.Console::ReadLine()");

            if (identType == 'i')
            {
                Compiler.EmitCode($"call int32 [mscorlib]System.Int32::Parse(string)");
            }
            if (identType == 'd')
            {
                Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo[mscorlib] System.Globalization.CultureInfo::get_InvariantCulture()");
                Compiler.EmitCode("call float64 [mscorlib]System.Double::Parse(string, class [mscorlib]System.IFormatProvider)");
            }
            if (identType == 'b')
            {
                Compiler.EmitCode($"call int32 [mscorlib]System.Int32::Parse(string)"); // todo wczytywanie do bool
            }

            Compiler.EmitCode($"stloc {ident}");

            return null;
        }
    }

    class Assign : SyntaxTree
    {
        private string ident;
        private char identType;
        private SyntaxTree exp;

        public Assign(int ln, SyntaxTree e, string id) { line = ln; ident = id; exp = e; }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: {ident} undeclared.");

            identType = Compiler.symbols.First(x => x.ident == ident).type;

            exp.Check();

            if (identType != exp.type)
                if (identType != 'd' && exp.type != 'i')
                    throw new ArgumentException($"  Semantic error at line {line}: Cannot assign expression type to ident type.");
        }

        public override string GenCode()
        {
            string t1, t2;
            t1 = exp.GenCode();
            if (identType == 'd' && exp.type == 'i')
            {
                t2 = Compiler.NewTemp('s');
                Compiler.EmitCode($"{t2} = sitofp i32 {t1} to double");
            }
            else
                t2 = t1;
            //Compiler.EmitCode("store {0} {1}, {0}* %{2}{3}", id[0] == '@' ? "i32" : "double", t2, id[0] == '@' ? 'i' : 'r', id[1]);
            return ident;
        }
    }

    class Return : SyntaxTree
    {
        public Return(int ln) { line = ln; }

        public override void Check() { }

        public override string GenCode()
        {
            Compiler.EmitCode("ret");
            return null;
        }
    }

    class Or : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;

        public Or(int ln, SyntaxTree l, SyntaxTree r, Tokens k) { line = ln; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type != 'b' || right.type != 'b')
                throw new ArgumentException($"  Semantic error at line {line}: For logical operations both arguments have to be of type bool.");

            type = 'b';
        }

        public override string GenCode()
        {
            return null;
        }
    }

    class And : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;

        public And(int ln, SyntaxTree l, SyntaxTree r) { line = ln; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type != 'b' || right.type != 'b')
                throw new ArgumentException($"  Semantic error at line {line}: For logical operations both arguments have to be of type bool.");

            type = 'b';
        }

        public override string GenCode()
        {
            return null;
        }
    }

    class Relational : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public Relational(int ln, SyntaxTree l, SyntaxTree r, Tokens k) { line = ln; left = l; right = r; kind = k; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if ((left.type == 'b') ^ (right.type == 'b'))
                throw new ArgumentException($"  Semantic error at line {line}: For relational operations both arguments or none of them have to be of type bool.");

            type = 'b';
        }

        public override string GenCode()
        {
            return null;
        }
    }

    class AdditiveMultiplicative : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public AdditiveMultiplicative(int ln, SyntaxTree l, SyntaxTree r, Tokens k) { line = ln; left = l; right = r; kind = k; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type == 'b' || right.type == 'b')
                throw new ArgumentException($"  Semantic error at line {line}: Bool is not a proper type for additive and multiplivative operations.");

            type = left.type == 'i' && right.type == 'i' ? 'i' : 'd';
        }

        public override string GenCode()
        {
            string tw, t1, t2, t3, t4, tt;

            t1 = left.GenCode();
            //if (left.type != type)
            {
                t2 = Compiler.NewTemp('i');
                Compiler.EmitCode($"{t2} = sitofp i32 {t1} to double");
            }
            //else
            t2 = t1;
            t3 = right.GenCode();
            //if (right.type != type)
            {
                t4 = Compiler.NewTemp('i');
                Compiler.EmitCode($"{t4} = sitofp i32 {t3} to double");
            }
            //else
            t4 = t3;

            tw = Compiler.NewTemp('i');
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

    class Bit : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public Bit(int ln, SyntaxTree l, SyntaxTree r, Tokens k) { line = ln; left = l; right = r; kind = k; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type != 'i' || right.type != 'i')
                throw new ArgumentException($"  Semantic error at line {line}: For bit operations both arguments have to be of type int.");

            type = 'i';
        }

        public override string GenCode()
        {
            return null;
        }
    }

    class Unary : SyntaxTree
    {
        private SyntaxTree tree;
        private Tokens kind;

        public Unary(int ln, SyntaxTree t, Tokens k) { line = ln; tree = t; kind = k; }

        public override void Check()
        {
            tree.Check();

            if (kind == Tokens.Minus)
            {
                if (tree.type == 'b')
                    throw new ArgumentException($"  Semantic error at line {line}: Unary minus does not accept arguments of type bool.");

                type = tree.type;
            }
            else if (kind == Tokens.BitNeg)
            {
                if (tree.type != 'i')
                    throw new ArgumentException($"  Semantic error at line {line}: Bitwise negation accepts only arguments of type int.");

                type = tree.type;
            }
            else if (kind == Tokens.IntCast)
            {
                type = 'i';
            }
            else if (kind == Tokens.DoubleCast)
            {
                type = 'd';
            }
        }

        public override string GenCode()
        {
            return null;
        }
    }

    class BoolValue : SyntaxTree
    {
        private bool val;

        public BoolValue(int ln, bool v) { line = ln; val = v; }

        public override void Check()
        {
            type = 'b';
        }

        public override string GenCode()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0###############}", val);
        }
    }

    class IntValue : SyntaxTree
    {
        private int val;

        public IntValue(int ln, int v) { line = ln; val = v; }

        public override void Check()
        {
            type = 'i';
        }

        public override string GenCode()
        {
            return val.ToString();
        }
    }

    class DoubleValue : SyntaxTree
    {
        private double val;

        public DoubleValue(int ln, double v) { line = ln; val = v; }

        public override void Check()
        {
            type = 'd';
        }

        public override string GenCode()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0###############}", val);
        }
    }
}
