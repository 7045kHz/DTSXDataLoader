using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class DtsAttribute_Return_IsNotNull
{
    [Fact]
    public void DtsAttribute_Return_NotNull()
    {
        var dtsAttribute = new DtsAttribute();
        Assert.NotNull(dtsAttribute);
    }
    
}
