
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

        private static int labelNumber = 0;

        public static int loopDepth = 0;

        public static List<int> syntaxErrors = new List<int>();

        public static List<SyntaxTree> code = new List<SyntaxTree>();

        public static List<Symbol> symbols = new List<Symbol>();

        public static List<(string, string)> loopLabels = new List<(string, string)>();

        public static List<(string, string)> ifLabels = new List<(string, string)>();

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

        public static string NewLabel()
        {
            return $"t{++labelNumber}";
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
                        EmitCode($".locals init (bool {symbol.ident})");
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

        public static (string, string) AddNewLoopBlock()
        {
            var labels = (NewLabel(), NewLabel());

            loopLabels.Add(labels);

            return labels;
        }

        public static (string, string) AddNewIfBlock()
        {
            var labels = (NewLabel(), NewLabel());

            ifLabels.Add(labels);

            return labels;
        }

        public static (string, string) GetLoopBlock(int depth = 1)
        {
            return loopLabels[loopLabels.Count - depth];
        }

        public static (string, string) PopLoopBlock()
        {
            var currentLabels = loopLabels.Last();
            loopLabels.RemoveAt(loopLabels.Count - 1);
            return currentLabels;
        }

        public static (string, string) GetIfBlock()
        {
            return ifLabels[ifLabels.Count - 1];
        }

        public static (string, string) PopIfBlock()
        {
            var currentLabels = ifLabels.Last();
            ifLabels.RemoveAt(ifLabels.Count - 1);
            return currentLabels;
        }
    }

    public abstract class SyntaxTree
    {
        public char type;
        public int line;

        public SyntaxTree(int ln) { line = ln; }

        public abstract void Check();

        public abstract void GenCode();
    }

    public class Symbol : SyntaxTree
    {
        public readonly string ident;

        public Symbol(int ln, string i) : base(ln)
        {
            type = ' ';
            ident = i;
        }

        public Symbol(int ln, char t, string i) : base(ln)
        {
            type = t;
            ident = i;
        }

        public override void GenCode()
        {
            Compiler.EmitCode($"ldloc {ident}");
        }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: {ident} undeclared");

            type = Compiler.symbols.First(x => x.ident == ident).type;
        }
    }

    class WriteString : SyntaxTree
    {
        private string value;

        public WriteString(int ln, string v) : base(ln) { value = v; }

        public override void Check() { }

        public override void GenCode()
        {
            Compiler.EmitCode($"ldstr {value}");
            Compiler.EmitCode($"call void [mscorlib]System.Console::Write(string)");
        }
    }

    class WriteExpression : SyntaxTree
    {
        private SyntaxTree exp;

        public WriteExpression(int ln, SyntaxTree e) : base(ln) { exp = e; }

        public override void Check() { exp.Check(); }

        public override void GenCode()
        {
            if (exp.type == 'i')
            {
                exp.GenCode();
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(int32)");
            }
            else if (exp.type == 'd')
            {
                Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
                Compiler.EmitCode("ldstr \"{0:0.000000}\"");
                exp.GenCode();
                Compiler.EmitCode("box [mscorlib]System.Double");
                Compiler.EmitCode("call string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider, string, object)");
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(string)");
            }
            else if (exp.type == 'b')
            {
                exp.GenCode();
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(bool)");
            }
        }
    }

    class Read : SyntaxTree
    {
        private readonly string ident;

        public Read(int ln, string id) : base(ln) { ident = id; }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: {ident} undeclared.");

            type = Compiler.symbols.First(x => x.ident == ident).type;
        }

        public override void GenCode()
        {
            Compiler.EmitCode("call string [mscorlib]System.Console::ReadLine()");

            if (type == 'i')
            {
                Compiler.EmitCode($"call int32 [mscorlib]System.Int32::Parse(string)");
            }
            if (type == 'd')
            {
                Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo[mscorlib] System.Globalization.CultureInfo::get_InvariantCulture()");
                Compiler.EmitCode("call float64 [mscorlib]System.Double::Parse(string, class [mscorlib]System.IFormatProvider)");
            }
            if (type == 'b')
            {
                Compiler.EmitCode($"call bool [mscorlib]System.Boolean::Parse(string)");
            }

            Compiler.EmitCode($"stloc {ident}");
        }
    }

    class While : SyntaxTree
    {
        SyntaxTree exp;

        public While(int ln, SyntaxTree e) : base(ln)
        {
            exp = e;
        }

        public override void Check()
        {
            Compiler.loopDepth++;
            exp.Check();

            if (exp.type != 'b')
                throw new ArgumentException($"  Semantic error at line {line}: While argument has to be of type bool.");
        }

        public override void GenCode()
        {
            var currentLabels = Compiler.AddNewLoopBlock();
            Compiler.EmitCode($"{currentLabels.Item1}: nop");
            exp.GenCode();
            Compiler.EmitCode($"brfalse {currentLabels.Item2}");
        }
    }

    class EndWhile : SyntaxTree
    {
        public EndWhile(int ln) : base(ln) { }

        public override void Check()
        {
            Compiler.loopDepth--;
        }

        public override void GenCode()
        {
            var currentLabels = Compiler.PopLoopBlock();

            Compiler.EmitCode($"br {currentLabels.Item1}");
            Compiler.EmitCode($"{currentLabels.Item2}: nop");
        }
    }

    class Break : SyntaxTree
    {
        string depthString;
        int depth;

        public Break(int ln, string d) : base(ln) { depthString = d; }

        public override void Check()
        {
            if (!int.TryParse(depthString, out depth))
                throw new ArgumentException($"  Semantic error at line {line}: Couldn't parse break's depth.");

            if (depth < 1)
                throw new ArgumentException($"  Semantic error at line {line}: Break's argument has to at least equal to 1.");

            if (depth > Compiler.loopDepth)
                throw new ArgumentException($"  Semantic error at line {line}: Not enough loops to break out of.");
        }

        public override void GenCode()
        {
            var currentLabels = Compiler.GetLoopBlock(depth);
            Compiler.EmitCode($"br {currentLabels.Item2}");
        }
    }

    class Continue : SyntaxTree
    {
        public Continue(int ln) : base(ln) { }

        public override void Check() { }

        public override void GenCode()
        {
            var currentLabels = Compiler.GetLoopBlock();
            Compiler.EmitCode($"br {currentLabels.Item1}");
        }
    }

    class If : SyntaxTree
    {
        SyntaxTree exp;

        public If(int ln, SyntaxTree e) : base(ln)
        {
            exp = e;
        }

        public override void Check()
        {
            exp.Check();

            if (exp.type != 'b')
                throw new ArgumentException($"  Semantic error at line {line}: If argument has to be of type bool.");
        }

        public override void GenCode()
        {
            var currentLabels = Compiler.AddNewIfBlock();
            exp.GenCode();
            Compiler.EmitCode($"brfalse {currentLabels.Item1}");
        }
    }

    class EndIf : SyntaxTree
    {
        public EndIf(int ln) : base(ln) { }

        public override void Check() { }

        public override void GenCode()
        {
            var currentLabels = Compiler.PopIfBlock();

            Compiler.EmitCode($"{currentLabels.Item1}: nop");
        }
    }

    class Else : SyntaxTree
    {
        public Else(int ln) : base(ln) { }

        public override void Check() { }

        public override void GenCode()
        {
            var currentLabels = Compiler.GetIfBlock();

            Compiler.EmitCode($"br {currentLabels.Item2}");
            Compiler.EmitCode($"{currentLabels.Item1}: nop");
        }
    }

    class EndElse : SyntaxTree
    {
        public EndElse(int ln) : base(ln) { }

        public override void Check() { }

        public override void GenCode()
        {
            var currentLabels = Compiler.PopIfBlock();

            Compiler.EmitCode($"{currentLabels.Item2}: nop");
        }
    }

    class Return : SyntaxTree
    {
        public Return(int ln) : base(ln) { }

        public override void Check() { }

        public override void GenCode()
        {
            Compiler.EmitCode("leave EndMain");
        }
    }

    class StandaloneExpression : SyntaxTree
    {
        SyntaxTree exp;

        public StandaloneExpression(int ln, SyntaxTree e) : base(ln) { exp = e; }

        public override void Check()
        {
            exp.Check();
        }

        public override void GenCode()
        {
            exp.GenCode();
            Compiler.EmitCode("pop");
        }
    }

    class Assign : SyntaxTree
    {
        private string ident;
        private SyntaxTree exp;

        public Assign(int ln, string id, SyntaxTree e) : base(ln) { ident = id; exp = e; }

        public override void Check()
        {
            if (!Compiler.symbols.Any(x => x.ident == ident))
                throw new ArgumentException($"  Semantic error at line {line}: {ident} undeclared.");

            type = Compiler.symbols.First(x => x.ident == ident).type;

            exp.Check();

            if (type != exp.type)
                if (type != 'd' && exp.type != 'i')
                    throw new ArgumentException($"  Semantic error at line {line}: Cannot assign expression type to ident type.");
        }

        public override void GenCode()
        {
            exp.GenCode();

            if (type == 'd' && exp.type == 'i')
                Compiler.EmitCode("conv.r8");

            Compiler.EmitCode($"dup");
            Compiler.EmitCode($"stloc {ident}");
        }
    }

    class Logical : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public Logical(int ln, Tokens k, SyntaxTree l, SyntaxTree r) : base(ln) { kind = k; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type != 'b' || right.type != 'b')
                throw new ArgumentException($"  Semantic error at line {line}: For logical operations both arguments have to be of type bool.");

            type = 'b';
        }

        public override void GenCode()
        {
            var first = Compiler.NewLabel();
            var second = Compiler.NewLabel();

            if (kind == Tokens.And)
            {
                left.GenCode();
                Compiler.EmitCode($"brfalse {first}");
                right.GenCode();
                Compiler.EmitCode($"br {second}");
                Compiler.EmitCode($"{first}: ldc.i4.0");
                Compiler.EmitCode($"{second}: nop");
            }
            else if (kind == Tokens.Or)
            {
                left.GenCode();
                Compiler.EmitCode($"brtrue {first}");
                right.GenCode();
                Compiler.EmitCode($"br {second}");
                Compiler.EmitCode($"{first}: ldc.i4.1");
                Compiler.EmitCode($"{second}: nop");
            }
        }
    }

    class Relational : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public Relational(int ln, Tokens k, SyntaxTree l, SyntaxTree r) : base(ln) { kind = k; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if ((left.type == 'b') ^ (right.type == 'b'))
                throw new ArgumentException($"  Semantic error at line {line}: For relational operations both arguments or none of them have to be of type bool.");
            
            if (left.type == 'b' && (kind != Tokens.Equal && kind != Tokens.NotEqual))
                throw new ArgumentException($"  Semantic error at line {line}: The only accepted comparisons between two bools are == and !=.");

            type = 'b';
        }

        public override void GenCode()
        {
            left.GenCode();
            if (right.type == 'd' && left.type == 'i')
                Compiler.EmitCode("conv.r8");
            right.GenCode();
            if (left.type == 'd' && right.type == 'i')
                Compiler.EmitCode("conv.r8");

            switch (kind)
            {
                case Tokens.Equal:
                    Compiler.EmitCode("ceq");
                    break;
                case Tokens.NotEqual:
                    Compiler.EmitCode("ceq");
                    Compiler.EmitCode("ldc.i4.0");
                    Compiler.EmitCode("ceq");
                    break;
                case Tokens.Greater:
                    Compiler.EmitCode("cgt");
                    break;
                case Tokens.Less:
                    Compiler.EmitCode("clt");
                    break;
                case Tokens.GreaterEqual:
                    Compiler.EmitCode("clt");
                    Compiler.EmitCode("ldc.i4.0");
                    Compiler.EmitCode("ceq");
                    break;
                case Tokens.LessEqual:
                    Compiler.EmitCode("cgt");
                    Compiler.EmitCode("ldc.i4.0");
                    Compiler.EmitCode("ceq");
                    break;
            }
        }
    }

    class AdditiveMultiplicative : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public AdditiveMultiplicative(int ln, Tokens k, SyntaxTree l, SyntaxTree r) : base(ln) { kind = k; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type == 'b' || right.type == 'b')
                throw new ArgumentException($"  Semantic error at line {line}: Bool is not a proper type for additive and multiplivative operations.");

            type = left.type == 'i' && right.type == 'i' ? 'i' : 'd';
        }

        public override void GenCode()
        {
            left.GenCode();
            if (type == 'd' && left.type == 'i')
                Compiler.EmitCode("conv.r8");
            right.GenCode();
            if (type == 'd' && right.type == 'i')
                Compiler.EmitCode("conv.r8");

            switch (kind)
            {
                case Tokens.Plus:
                    Compiler.EmitCode("add");
                    break;
                case Tokens.Minus:
                    Compiler.EmitCode("sub");
                    break;
                case Tokens.Multiply:
                    Compiler.EmitCode("mul");
                    break;
                case Tokens.Divide:
                    Compiler.EmitCode("div");
                    break;
            }
        }
    }

    class Bit : SyntaxTree
    {
        private SyntaxTree left;
        private SyntaxTree right;
        private Tokens kind;

        public Bit(int ln, Tokens k, SyntaxTree l, SyntaxTree r) : base(ln) { kind = k; left = l; right = r; }

        public override void Check()
        {
            left.Check();
            right.Check();

            if (left.type != 'i' || right.type != 'i')
                throw new ArgumentException($"  Semantic error at line {line}: For bit operations both arguments have to be of type int.");

            type = 'i';
        }

        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();

            if (kind == Tokens.BitAnd)
                Compiler.EmitCode("and");
            else if (kind == Tokens.BitAnd)
                Compiler.EmitCode("or");
        }
    }

    class Unary : SyntaxTree
    {
        private SyntaxTree exp;
        private Tokens kind;

        public Unary(int ln, Tokens k, SyntaxTree t) : base(ln) { kind = k; exp = t; }

        public override void Check()
        {
            exp.Check();

            if (kind == Tokens.Minus)
            {
                if (exp.type == 'b')
                    throw new ArgumentException($"  Semantic error at line {line}: Unary minus does not accept arguments of type bool.");

                type = exp.type;
            }
            else if (kind == Tokens.BitNeg)
            {
                if (exp.type != 'i')
                    throw new ArgumentException($"  Semantic error at line {line}: Bitwise negation accepts only arguments of type int.");

                type = exp.type;
            }
            else if (kind == Tokens.Neg)
            {
                if (exp.type != 'b')
                    throw new ArgumentException($"  Semantic error at line {line}: Logical negation accepts only arguments of type bool.");

                type = exp.type;
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

        public override void GenCode()
        {
            exp.GenCode();

            if (kind == Tokens.Minus)
            {
                Compiler.EmitCode("neg");
            }
            else if (kind == Tokens.BitNeg)
            {
                Compiler.EmitCode("not");
            }
            else if (kind == Tokens.Neg)
            {
                Compiler.EmitCode("ldc.i4.0");
                Compiler.EmitCode("ceq");
            }
            else if (kind == Tokens.IntCast)
            {
                Compiler.EmitCode("conv.i4");
            }
            else if (kind == Tokens.DoubleCast)
            {
                Compiler.EmitCode("conv.r8");
            }
        }
    }

    class BoolValue : SyntaxTree
    {
        private bool val;

        public BoolValue(int ln, bool v) : base(ln) { val = v; }

        public override void Check()
        {
            type = 'b';
        }

        public override void GenCode()
        {
            if (val)
                Compiler.EmitCode("ldc.i4.1");
            else
                Compiler.EmitCode("ldc.i4.0");
        }
    }

    class NumericValue : SyntaxTree
    {
        private string val;

        public NumericValue(int ln, char t, string v) : base(ln) { type = t; val = v; }

        public override void Check() { }

        public override void GenCode()
        {
            if (type == 'i')
                Compiler.EmitCode($"ldc.i4 {val}");
            else
                Compiler.EmitCode($"ldc.r8 {val}");
        }
    }
}
