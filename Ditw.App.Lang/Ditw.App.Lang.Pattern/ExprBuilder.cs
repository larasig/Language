using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ditw.App.Lang.Pattern
{
	
    public class ExprBuilder
    {
    	private const String ZeroOrMoreChar = ".*";
    	#if false
        public static Regex GetRegex(String s1, String s2)
        {
            Regex regex = new Regex(
                String.Format("{0}.*{1}", s1, s2)
                );

            return regex;
        }
        #else
        public static Regex GetRegex(params String[] wordsInOrder)
        {
        	#if false
        	StringBuilder builder = new StringBuilder();
        	foreach(String w in wordsInOrder)
        	{
        		if (builder.Length == 0)
        			builder.Append(w);
        		else
        			builder.AppendFormat("{0}{1}", ZeroOrMoreChar, w);
        	}
        	Regex regex = new Regex(builder.ToString());

            return regex;
            #else
            return new AndExpr(
            	null,
            	wordsInOrder.Select(
            		w => LitExpr.FromString(w)
            	).ToArray()
            ).ToRegex();
            #endif
        }
        #endif
        
        public static Regex GetRegex(String[] separators, params String[] wordsInOrder)
        {
        	OrExpr orExpr = new OrExpr(
        		separators.Select(
        			s => LitExpr.FromString(s)
        		).ToArray()
        	);
        	//String negSeparators = String.Format("[^({0})]*", builder);
        	
            return new AndExpr(
        		new NotExpr(orExpr),
            	wordsInOrder.Select(
            		w => LitExpr.FromString(w)
            	).ToArray()
            ).ToRegex();
        }

        
        public static Regex GetRegexNoClauseSeparators(Char[] clauseSeparators, params String[] wordsInOrder)
        {
        	StringBuilder builder;
        	builder = new StringBuilder();
        	foreach (Char c in clauseSeparators)
        	{
        		builder.Append(c);
        	}
        	String negSeparators = String.Format("[^{0}]*", builder);
        	
        	builder = new StringBuilder();
        	foreach (String w in wordsInOrder)
        	{
        		if (builder.Length == 0)
        			builder.Append(w);
        		else
        			builder.AppendFormat("{0}{1}", negSeparators, w);
        	}
        	Regex regex = new Regex(builder.ToString());

            return regex;
        }

        public static Regex GetRegexNoClauseSeparators(String[] separators, params String[] wordsInOrder)
        {
        	StringBuilder builder;
        	builder = new StringBuilder();
        	foreach (String s in separators)
        	{
        		if (builder.Length == 0)
        			builder.Append(s);
        		else
        			builder.AppendFormat("|{0}", s);
        	}
        	String negSeparators = String.Format("[^({0})]*", builder);
        	
        	builder = new StringBuilder();
        	foreach (String w in wordsInOrder)
        	{
        		if (builder.Length == 0)
        			builder.Append(w);
        		else
        			builder.AppendFormat("{0}{1}", negSeparators, w);
        	}
        	Regex regex = new Regex(builder.ToString());

            return regex;
        }

        //public static Regex
    }
}
