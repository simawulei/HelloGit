using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.JScript;  

namespace UtilModel
{
    public class EvalModel
    {
        //static Microsoft.JScript.Vsa.VsaEngine vEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
        public int Eval(string sCode, out string sResult)
        {
            int nRet = 0;
            //sResult = EvalMethod.Eval(sCode).ToString();
            
            Microsoft.JScript.Vsa.VsaEngine vEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
            try
            {
                sResult = Microsoft.JScript.Eval.JScriptEvaluate(sCode, vEngine).ToString();
            }
            catch (System.Exception ex)
            {
                nRet = -1;
                sResult = ex.Message;
            }
            GC.Collect();
            return nRet;
        }

        /// <summary>
        /// 计算字符串表达式的通用类。
        /// 引用Microsoft.JScript.Eval和Microsoft.JScript.Vsa.VsaEngine来实现公式计算方法
        /// </summary>
        /// <param name="strequation">字符串表达式</param>
        /// <param name="paras">传入的参数</param>
        /// <param name="paravalues">传入参数对应的值</param>
        /// <param name="repara">返回的参数名</param>
        /// <returns></returns>
        public object RunFormula(string strequation, List<string> paras, List<string> paravalues, string repara)
        {

            StringBuilder myCode = new StringBuilder();
            myCode.Append("function getValue() : double {");
            for (int i = 0; i < paras.Count; i++)
            {
                myCode.Append("var " + paras[i] + " = " + paravalues[i] + " ;");
            }
            myCode.Append("var " + repara + "=0;  with (Math){ ");
            myCode.Append(repara + "=" + strequation + "};");
            myCode.Append("  return " + repara + ";");
            myCode.Append("}");
            
            try
            {
                object ret = Microsoft.JScript.Eval.JScriptEvaluate(myCode + "getValue()",
                    Microsoft.JScript.Vsa.VsaEngine.CreateEngine());
                return ret;
            }
            catch
            {
                return "false";
            }

        }


    }

    public class JSEvaluator
    {
        public static int EvalToInteger(string statement)
        {
            string s = EvalToString(statement);
            return int.Parse(s.ToString());
        }

        public static double EvalToDouble(string statement)
        {
            string s = EvalToString(statement);
            return double.Parse(s);
        }

        public static string EvalToString(string statement)
        {
            object o = EvalToObject(statement);
            return o.ToString();
        }


        // current version with JScriptCodeProvider BEGIN       
        ///*       

        public static object EvalToObject(string statement)
        {
            return _evaluatorType.InvokeMember(
                  "Eval",
                  BindingFlags.InvokeMethod,
                  null,
                  _evaluator,
                  new object[] { statement }
                 );
        }

        static JSEvaluator()
        {
            JScriptCodeProvider compiler = new JScriptCodeProvider();

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(
                                            parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("JSEvaluator.JSEvaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly string _jscriptSource =
          @"package JSEvaluator     
      {     
         class JSEvaluator     
         {     
          public function Eval(expr : String) : Object     
          {     
           return eval(expr);     
          }     
         }     
      }";

        //*/       
        // current version with JScriptCodeProvider END       


        // deprecated version with Vsa BEGIN       
        /*     
    
        public static Microsoft.JScript.Vsa.VsaEngine Engine =     
                      Microsoft.JScript.Vsa.VsaEngine.CreateEngine();     
    
        public static object EvalToObject(string JScript)     
        {     
          object Result = null;     
          try     
          {     
            Result = Microsoft.JScript.Eval.JScriptEvaluate(     
                                                    JScript, Engine);     
          }     
          catch (Exception ex)     
          {     
            return ex.Message;     
          }     
          return Result;     
        }     
    
            */
        // deprecated version with Vsa END       
    }

    public class EvalMethod
    {
        //初始化加载程序的字符串;
        public static readonly String _jsClass = @"class theEval{ public function Eval(str:String):String {return eval(str)}}";
        //定义对象
        public static object _evalObject = null;
        public static Type _evalType = null;

        //构造函数
        static EvalMethod()
        {
            CodeDomProvider _provider = new JScriptCodeProvider();
            ICodeCompiler _iCode = _provider.CreateCompiler();
            CompilerParameters _parameters = new CompilerParameters();
            _parameters.GenerateInMemory = true;
            CompilerResults _result;
            _result = _iCode.CompileAssemblyFromSource(_parameters, _jsClass);

            Assembly _assembly = _result.CompiledAssembly;
            _evalType = _assembly.GetType("theEval");
            _evalObject = Activator.CreateInstance(_evalType);
        }

        public static object Eval(string str)
        {
            return _evalType.InvokeMember("Eval", BindingFlags.InvokeMethod, null, _evalObject, new object[] { str });
        }
    }

}
