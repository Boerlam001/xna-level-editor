using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace Generator
{
    public class ClassManager
    {
        DTE2 applicationObject;
        CodeElement codeElement;

        public ClassManager(DTE2 applicationObject)
        {
            this.applicationObject = applicationObject;
        }        
    }
}
