using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class DtsBase_Return_IsNotNull
{
    [Fact]
    public void DtsBase_Return_NotNull()
    {
        var dtsBase = new DtsBase();
        Assert.NotNull(dtsBase);
    }
}
