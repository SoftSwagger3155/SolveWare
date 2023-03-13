using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.General
{
    public class MathCalculateHelper
    {
        private static string _token; /*全局变量*/
        private static string _inputString = string.Empty;
        private static IList<string> _inputStringList = new List<string>();
        private static int _index = 0;
        public static double CalcMath(string input)
        {
            if (string.IsNullOrEmpty(input)) throw new ArgumentNullException("input", "输⼊的数学公式字符串不能为空"); _inputString = input.Trim();
            _inputStringList = new List<string>();
            _index = 0;
            double result;
            #region 字符串解释
            Regex reg = new Regex("\\+|\\-|\\*|/|\\(|\\)|\\[|\\]|\\{|\\}", RegexOptions.Singleline);
            string[] strList = reg.Split(_inputString);
            MatchCollection mc = reg.Matches(_inputString);
            if (mc.Count > 0)
            {
                for (int i = 0; i < strList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(strList[i]))
                        _inputStringList.Add(strList[i]);
                    if (i < mc.Count)
                    {
                        Match mat = mc[i];
                        if (mat.Success)
                        {
                            string s = mat.Value;
                            if (!string.IsNullOrEmpty(s))
                                _inputStringList.Add(s);
                        }
                    }
                }
            }
            #endregion
            _token = GetNext(); /*载⼊第⼀个单元*/
            result = Low(); /*进⾏计算*/
            return result;
        }
        /*对当前的标志进⾏匹配*/
        private static void MatchMarker(string expectedToken)
        {
            if (_token == expectedToken)
            {
                _token = GetNext();/*匹配成功，获取下⼀个*/
            }
            else
            {
                throw new ArgumentException("运算符不匹配", "token");
            }
        }
        /*⽤于计算表达式中的+、-运算*/
        private static double Low()
        {
            double result = Mid();
            while (_token == "+" || _token == "-")
                if (_token == "+")
                {
                    MatchMarker("+");
                    result += Mid();
                }
                else if (_token == "-")
                {
                    MatchMarker("-");
                    result -= Mid();
                }
            return result;
        }
        /*⽤于计算表达式中的*、/运算*/
        private static double Mid()
        {
            double div;
            double result = High();
            while (_token == "*" || _token == "/")
                if (_token == "*")
                {
                    MatchMarker("*");
                    div = High();
                    result *= div;
                }
                else if (_token == "/")
                {
                    MatchMarker("/");
                    div = High();
                    if (div == 0) div = 1;
                    result /= div;
                }
            return result;
        }
        /*⽤于计算表达式中带{}的运算*/
        private static double High()
        {
            double result = High2();
            if (_token == "{")
            {
                MatchMarker("{");
                result = Low(); /*递归计算表达式*/
                MatchMarker("}");
            }
            return result;
        }
        /*⽤于计算表达式中带[]的运算*/
        private static double High2()
        {
            double result = High3();
            if (_token == "[")
            {
                MatchMarker("[");
                result = Low(); /*递归计算表达式*/
                MatchMarker("]");
            }
            return result;
        }
        /*⽤于计算表达式中带()的运算*/
        private static double High3()
        {
            double result = MathInput();
            if (_token == "(")
            {
                MatchMarker("(");
                result = Low(); /*递归计算表达式*/
                MatchMarker(")");
            }
            return result;
        }
        /*获取计算数值*/
        private static double MathInput()
        {
            double result = 0;
            if (double.TryParse(_token, out result))
            {
                _token = GetNext();
            }
            return result;
        }
        private static string GetNext()
        {
            string ret = string.Empty;
            if (_index < _inputStringList.Count)
            {
                ret = _inputStringList[_index];
                _index++;
            }
            return ret;
        }
    }
}
