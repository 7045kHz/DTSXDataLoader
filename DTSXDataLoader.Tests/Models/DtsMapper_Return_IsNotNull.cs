using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Tests.Models;
public class DtsMapper_Return_IsNotNull
{
    [Fact]
    public void DtsMapper_Return_NotNull()
    {
        var dtsMapper = new DtsMapper();
        Assert.NotNull(dtsMapper);
    }
    
}
