// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  ACER-VN7-591G
// DateTime: 16.06.2020 12:57:12
// UserName: Marcel
// Input file <../../kompilator.y - 16.06.2020 12:49:04>

// options: lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace Compiler
{
public enum Tokens {error=2,EOF=3,Program=4,Semicolon=5,IntCast=6,
    DoubleCast=7,Int=8,Real=9,Bool=10,Assign=11,Ident=12,
    RealValue=13,IntegerValue=14,String=15,True=16,False=17,If=18,
    Else=19,While=20,Read=21,Write=22,Plus=23,Minus=24,
    Multiply=25,Divide=26,BitNeg=27,BitOr=28,BitAnd=29,Neg=30,
    Or=31,And=32,Equal=33,NotEqual=34,Greater=35,Less=36,
    GreaterEqual=37,LessEqual=38,OpenPar=39,ClosePar=40,OpenCurly=41,CloseCurly=42,
    Return=43,Eof=44,Error=45};

public struct ValueType
#line 5 "../../kompilator.y"
{
    public string       val;
    public char         type;
    public SyntaxTree   subtree;
}
#line default
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[65];
  private static State[] states = new State[122];
  private static string[] nonTerms = new string[] {
      "start", "declarations", "declaration", "commands", "command", "write", 
      "read", "return", "assign", "if", "while", "expression", "logical", "relational", 
      "additive", "multiplicative", "bit", "unary", "term", "$accept", "Anon@1", 
      "Anon@2", "Anon@3", };

  static Parser() {
    states[0] = new State(new int[]{4,3,2,121},new int[]{-1,1});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{41,4});
    states[4] = new State(new int[]{8,105,9,109,10,113,2,117,41,-5,22,-5,21,-5,12,-5,18,-5,20,-5,43,-5,42,-5},new int[]{-2,5,-3,103});
    states[5] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99,42,-15},new int[]{-4,6,-5,9,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[6] = new State(new int[]{42,7});
    states[7] = new State(new int[]{44,8});
    states[8] = new State(-2);
    states[9] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99,42,-15},new int[]{-4,10,-5,9,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[10] = new State(-14);
    states[11] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99,42,-15},new int[]{-4,12,-5,9,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[12] = new State(new int[]{42,13});
    states[13] = new State(-16);
    states[14] = new State(-17);
    states[15] = new State(new int[]{15,16,24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-12,18,-13,20,-14,44,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[16] = new State(new int[]{5,17});
    states[17] = new State(-25);
    states[18] = new State(new int[]{5,19});
    states[19] = new State(-26);
    states[20] = new State(new int[]{31,21,32,71,5,-33,40,-33});
    states[21] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-14,22,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[22] = new State(new int[]{33,23,34,45,35,62,36,64,37,66,38,68,31,-34,32,-34,5,-34,40,-34});
    states[23] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,24,-16,61,-17,60,-18,59,-19,53});
    states[24] = new State(new int[]{23,25,24,47,33,-37,34,-37,35,-37,36,-37,37,-37,38,-37,31,-37,32,-37,5,-37,40,-37});
    states[25] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-16,26,-17,60,-18,59,-19,53});
    states[26] = new State(new int[]{25,27,26,49,23,-44,24,-44,33,-44,34,-44,35,-44,36,-44,37,-44,38,-44,31,-44,32,-44,5,-44,40,-44});
    states[27] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-17,28,-18,59,-19,53});
    states[28] = new State(new int[]{28,29,29,51,25,-47,26,-47,23,-47,24,-47,33,-47,34,-47,35,-47,36,-47,37,-47,38,-47,31,-47,32,-47,5,-47,40,-47});
    states[29] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,30,-19,53});
    states[30] = new State(-50);
    states[31] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,32,-19,53});
    states[32] = new State(-53);
    states[33] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,34,-19,53});
    states[34] = new State(-54);
    states[35] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,36,-19,53});
    states[36] = new State(-55);
    states[37] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,38,-19,53});
    states[38] = new State(-56);
    states[39] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,40,-19,53});
    states[40] = new State(-57);
    states[41] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-12,42,-13,20,-14,44,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[42] = new State(new int[]{40,43});
    states[43] = new State(-58);
    states[44] = new State(new int[]{33,23,34,45,35,62,36,64,37,66,38,68,31,-36,32,-36,5,-36,40,-36});
    states[45] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,46,-16,61,-17,60,-18,59,-19,53});
    states[46] = new State(new int[]{23,25,24,47,33,-38,34,-38,35,-38,36,-38,37,-38,38,-38,31,-38,32,-38,5,-38,40,-38});
    states[47] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-16,48,-17,60,-18,59,-19,53});
    states[48] = new State(new int[]{25,27,26,49,23,-45,24,-45,33,-45,34,-45,35,-45,36,-45,37,-45,38,-45,31,-45,32,-45,5,-45,40,-45});
    states[49] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-17,50,-18,59,-19,53});
    states[50] = new State(new int[]{28,29,29,51,25,-48,26,-48,23,-48,24,-48,33,-48,34,-48,35,-48,36,-48,37,-48,38,-48,31,-48,32,-48,5,-48,40,-48});
    states[51] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-18,52,-19,53});
    states[52] = new State(-51);
    states[53] = new State(-59);
    states[54] = new State(-60);
    states[55] = new State(-61);
    states[56] = new State(-62);
    states[57] = new State(-63);
    states[58] = new State(-64);
    states[59] = new State(-52);
    states[60] = new State(new int[]{28,29,29,51,25,-49,26,-49,23,-49,24,-49,33,-49,34,-49,35,-49,36,-49,37,-49,38,-49,31,-49,32,-49,5,-49,40,-49});
    states[61] = new State(new int[]{25,27,26,49,23,-46,24,-46,33,-46,34,-46,35,-46,36,-46,37,-46,38,-46,31,-46,32,-46,5,-46,40,-46});
    states[62] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,63,-16,61,-17,60,-18,59,-19,53});
    states[63] = new State(new int[]{23,25,24,47,33,-39,34,-39,35,-39,36,-39,37,-39,38,-39,31,-39,32,-39,5,-39,40,-39});
    states[64] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,65,-16,61,-17,60,-18,59,-19,53});
    states[65] = new State(new int[]{23,25,24,47,33,-40,34,-40,35,-40,36,-40,37,-40,38,-40,31,-40,32,-40,5,-40,40,-40});
    states[66] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,67,-16,61,-17,60,-18,59,-19,53});
    states[67] = new State(new int[]{23,25,24,47,33,-41,34,-41,35,-41,36,-41,37,-41,38,-41,31,-41,32,-41,5,-41,40,-41});
    states[68] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-15,69,-16,61,-17,60,-18,59,-19,53});
    states[69] = new State(new int[]{23,25,24,47,33,-42,34,-42,35,-42,36,-42,37,-42,38,-42,31,-42,32,-42,5,-42,40,-42});
    states[70] = new State(new int[]{23,25,24,47,33,-43,34,-43,35,-43,36,-43,37,-43,38,-43,31,-43,32,-43,5,-43,40,-43});
    states[71] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-14,72,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[72] = new State(new int[]{33,23,34,45,35,62,36,64,37,66,38,68,31,-35,32,-35,5,-35,40,-35});
    states[73] = new State(-18);
    states[74] = new State(new int[]{12,75});
    states[75] = new State(new int[]{5,76});
    states[76] = new State(-27);
    states[77] = new State(-19);
    states[78] = new State(new int[]{11,79});
    states[79] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-12,80,-13,20,-14,44,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[80] = new State(new int[]{5,81});
    states[81] = new State(-29);
    states[82] = new State(-20);
    states[83] = new State(new int[]{39,84});
    states[84] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-12,85,-13,20,-14,44,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[85] = new State(new int[]{40,86});
    states[86] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99},new int[]{-5,87,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[87] = new State(new int[]{19,88,41,-31,22,-31,21,-31,12,-31,18,-31,20,-31,43,-31,2,-31,42,-31});
    states[88] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99},new int[]{-5,89,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[89] = new State(-30);
    states[90] = new State(-21);
    states[91] = new State(new int[]{39,92});
    states[92] = new State(new int[]{24,31,27,33,30,35,6,37,7,39,39,41,12,54,13,55,14,56,16,57,17,58},new int[]{-12,93,-13,20,-14,44,-15,70,-16,61,-17,60,-18,59,-19,53});
    states[93] = new State(new int[]{40,94});
    states[94] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99},new int[]{-5,95,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[95] = new State(-32);
    states[96] = new State(-22);
    states[97] = new State(new int[]{5,98});
    states[98] = new State(-28);
    states[99] = new State(new int[]{5,100,44,102});
    states[100] = new State(new int[]{41,11,22,15,21,74,12,78,18,83,20,91,43,97,2,99},new int[]{-5,101,-6,14,-7,73,-9,77,-10,82,-11,90,-8,96});
    states[101] = new State(-23);
    states[102] = new State(-24);
    states[103] = new State(new int[]{8,105,9,109,10,113,2,117,41,-5,22,-5,21,-5,12,-5,18,-5,20,-5,43,-5,42,-5},new int[]{-2,104,-3,103});
    states[104] = new State(-4);
    states[105] = new State(new int[]{12,106});
    states[106] = new State(-6,new int[]{-21,107});
    states[107] = new State(new int[]{5,108});
    states[108] = new State(-7);
    states[109] = new State(new int[]{12,110});
    states[110] = new State(-8,new int[]{-22,111});
    states[111] = new State(new int[]{5,112});
    states[112] = new State(-9);
    states[113] = new State(new int[]{12,114});
    states[114] = new State(-10,new int[]{-23,115});
    states[115] = new State(new int[]{5,116});
    states[116] = new State(-11);
    states[117] = new State(new int[]{5,118,44,120});
    states[118] = new State(new int[]{8,105,9,109,10,113,2,117},new int[]{-3,119});
    states[119] = new State(-12);
    states[120] = new State(-13);
    states[121] = new State(-3);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-20, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{4,41,-2,-4,42,44});
    rules[3] = new Rule(-1, new int[]{2});
    rules[4] = new Rule(-2, new int[]{-3,-2});
    rules[5] = new Rule(-2, new int[]{});
    rules[6] = new Rule(-21, new int[]{});
    rules[7] = new Rule(-3, new int[]{8,12,-21,5});
    rules[8] = new Rule(-22, new int[]{});
    rules[9] = new Rule(-3, new int[]{9,12,-22,5});
    rules[10] = new Rule(-23, new int[]{});
    rules[11] = new Rule(-3, new int[]{10,12,-23,5});
    rules[12] = new Rule(-3, new int[]{2,5,-3});
    rules[13] = new Rule(-3, new int[]{2,44});
    rules[14] = new Rule(-4, new int[]{-5,-4});
    rules[15] = new Rule(-4, new int[]{});
    rules[16] = new Rule(-5, new int[]{41,-4,42});
    rules[17] = new Rule(-5, new int[]{-6});
    rules[18] = new Rule(-5, new int[]{-7});
    rules[19] = new Rule(-5, new int[]{-9});
    rules[20] = new Rule(-5, new int[]{-10});
    rules[21] = new Rule(-5, new int[]{-11});
    rules[22] = new Rule(-5, new int[]{-8});
    rules[23] = new Rule(-5, new int[]{2,5,-5});
    rules[24] = new Rule(-5, new int[]{2,44});
    rules[25] = new Rule(-6, new int[]{22,15,5});
    rules[26] = new Rule(-6, new int[]{22,-12,5});
    rules[27] = new Rule(-7, new int[]{21,12,5});
    rules[28] = new Rule(-8, new int[]{43,5});
    rules[29] = new Rule(-9, new int[]{12,11,-12,5});
    rules[30] = new Rule(-10, new int[]{18,39,-12,40,-5,19,-5});
    rules[31] = new Rule(-10, new int[]{18,39,-12,40,-5});
    rules[32] = new Rule(-11, new int[]{20,39,-12,40,-5});
    rules[33] = new Rule(-12, new int[]{-13});
    rules[34] = new Rule(-13, new int[]{-13,31,-14});
    rules[35] = new Rule(-13, new int[]{-13,32,-14});
    rules[36] = new Rule(-13, new int[]{-14});
    rules[37] = new Rule(-14, new int[]{-14,33,-15});
    rules[38] = new Rule(-14, new int[]{-14,34,-15});
    rules[39] = new Rule(-14, new int[]{-14,35,-15});
    rules[40] = new Rule(-14, new int[]{-14,36,-15});
    rules[41] = new Rule(-14, new int[]{-14,37,-15});
    rules[42] = new Rule(-14, new int[]{-14,38,-15});
    rules[43] = new Rule(-14, new int[]{-15});
    rules[44] = new Rule(-15, new int[]{-15,23,-16});
    rules[45] = new Rule(-15, new int[]{-15,24,-16});
    rules[46] = new Rule(-15, new int[]{-16});
    rules[47] = new Rule(-16, new int[]{-16,25,-17});
    rules[48] = new Rule(-16, new int[]{-16,26,-17});
    rules[49] = new Rule(-16, new int[]{-17});
    rules[50] = new Rule(-17, new int[]{-17,28,-18});
    rules[51] = new Rule(-17, new int[]{-17,29,-18});
    rules[52] = new Rule(-17, new int[]{-18});
    rules[53] = new Rule(-18, new int[]{24,-18});
    rules[54] = new Rule(-18, new int[]{27,-18});
    rules[55] = new Rule(-18, new int[]{30,-18});
    rules[56] = new Rule(-18, new int[]{6,-18});
    rules[57] = new Rule(-18, new int[]{7,-18});
    rules[58] = new Rule(-18, new int[]{39,-12,40});
    rules[59] = new Rule(-18, new int[]{-19});
    rules[60] = new Rule(-19, new int[]{12});
    rules[61] = new Rule(-19, new int[]{13});
    rules[62] = new Rule(-19, new int[]{14});
    rules[63] = new Rule(-19, new int[]{16});
    rules[64] = new Rule(-19, new int[]{17});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // start -> Program, OpenCurly, declarations, commands, CloseCurly, Eof
#line 39 "../../kompilator.y"
                           { YYAccept(); }
#line default
        break;
      case 3: // start -> error
#line 40 "../../kompilator.y"
                  { Compiler.syntaxErrors.Add(Compiler.lineno); YYAbort(); }
#line default
        break;
      case 6: // Anon@1 -> /* empty */
#line 51 "../../kompilator.y"
                  { new IntIdent(ValueStack[ValueStack.Depth-1].val); }
#line default
        break;
      case 8: // Anon@2 -> /* empty */
#line 54 "../../kompilator.y"
                  { new RealIdent(ValueStack[ValueStack.Depth-1].val); }
#line default
        break;
      case 10: // Anon@3 -> /* empty */
#line 57 "../../kompilator.y"
                  { new BoolIdent(ValueStack[ValueStack.Depth-1].val); }
#line default
        break;
      case 12: // declaration -> error, Semicolon, declaration
#line 59 "../../kompilator.y"
                                        { Compiler.syntaxErrors.Add(Compiler.lineno); }
#line default
        break;
      case 13: // declaration -> error, Eof
#line 60 "../../kompilator.y"
                      { Compiler.syntaxErrors.Add(Compiler.lineno); YYAbort(); }
#line default
        break;
      case 23: // command -> error, Semicolon, command
#line 77 "../../kompilator.y"
                                    { Compiler.syntaxErrors.Add(Compiler.lineno); }
#line default
        break;
      case 24: // command -> error, Eof
#line 78 "../../kompilator.y"
                      { Compiler.syntaxErrors.Add(Compiler.lineno); YYAbort(); }
#line default
        break;
      case 25: // write -> Write, String, Semicolon
#line 84 "../../kompilator.y"
                      { Compiler.AddNewNode(new WriteString(ValueStack[ValueStack.Depth-2].val)); }
#line default
        break;
      case 26: // write -> Write, expression, Semicolon
#line 87 "../../kompilator.y"
                      { /* Compiler.AddNewNode(new WriteExpression($2)); */ }
#line default
        break;
      case 27: // read -> Read, Ident, Semicolon
#line 93 "../../kompilator.y"
                      { Compiler.AddNewNode(new Read(ValueStack[ValueStack.Depth-2].val)); }
#line default
        break;
      case 33: // expression -> logical
#line 132 "../../kompilator.y"
                    { CurrentSemanticValue.subtree = new BoolIdent("qwe"); }
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 188 "../../kompilator.y"

public Parser(Scanner scanner) : base(scanner) { }
#line default
}
}
