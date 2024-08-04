using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class XConfig_Return_IsNotNull
{
    [Fact]
    public void XConfig_Return_NotNull()
    {
        var xConfig = new XConfig();
        Assert.NotNull(xConfig);
    }
    
}
