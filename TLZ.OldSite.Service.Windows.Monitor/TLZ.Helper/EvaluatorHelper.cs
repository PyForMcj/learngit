#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  EvaluatorHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  Administrator 
     * 创建时间：2014/12/2 18:56:50 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/12/2 18:56:50 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;

namespace TLZ.Helper
{
    /// <summary>
    /// 动态求值
    /// </summary>
    public class EvaluatorHelper
    {
        /// <summary>
        /// 计算结果,如果表达式出错则抛出异常
        /// </summary>
        /// <param name="statement">表达式,如"1+2+3+4"</param>
        /// <returns>结果</returns>
        public static string Eval(string statement)
        {
            //return _evaluatorType.InvokeMember(
            //            "Eval",
            //            BindingFlags.InvokeMethod,
            //            null,
            //            _evaluator,
            //            new object[] { statement }
            //         );
            return _evaluatorFunc(statement);
        }

        static EvaluatorHelper()
        {
            //构造JScript的编译驱动代码
            CodeDomProvider provider = CodeDomProvider.CreateProvider("JScript");

            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true };
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, _jscriptSource);
            Assembly assembly = results.CompiledAssembly;
            
            _evaluatorType = assembly.GetType("Evaluator");
            _evaluator = Activator.CreateInstance(_evaluatorType);
            _evaluatorFunc = (Func<string, string>)Delegate.CreateDelegate(typeof(Func<string, string>), _evaluator, "Eval", false);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static Func<string, string> _evaluatorFunc = null;

        /// <summary>
        /// JScript代码
        /// </summary>
        private static readonly string _jscriptSource =
             @"class Evaluator
               {
                   public function Eval(expr : String) : String 
                   { 
                      return eval(expr); 
                   }
               }";


        private delegate TResult Func<in T1, out TResult>(T1 arg1);
    }
}
