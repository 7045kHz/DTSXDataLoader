using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class DtsElement_Return_IsNotNull
{
    [Fact]
    public void DtsElement_Return_NotNull()
    {
        var dtsElement = new DtsElement();
        Assert.NotNull(dtsElement);
    }
    
}
