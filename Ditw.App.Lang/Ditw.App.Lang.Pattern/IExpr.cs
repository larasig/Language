using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ditw.App.Lang.Pattern
{
    public interface IExpr
    {
    	String Text
    	{
    		get;
    	}
    	
    	//Regex ToRegex();
    	//Boolean Match(String text);
    }
    

}
