using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Models;
namespace DTSXDataLoader.Tests.Models;
public class Options_Return_IsNotNull
{
    [Fact]
    public void Options_Return_NotNull()
    {
        var options = new DTSXDataLoader.Models.Options();
        Assert.NotNull(options);
    }
}
