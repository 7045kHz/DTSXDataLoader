using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class DtsVariable_Return_IsNotNull
{
    [Fact]
    public void DtsVariable_Return_NotNull()
    {
        var dtsVariable = new DtsVariable();
        Assert.NotNull(dtsVariable);
    }
    
}
