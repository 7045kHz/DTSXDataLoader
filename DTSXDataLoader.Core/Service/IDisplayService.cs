using DTSXDataLoaderCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DTSXDataLoader.Service
{
    public interface IDisplayService
    {
        void DisplayVariables(List<DtsVariable> variables);
        void InsertVariables(List<DtsVariable> variables);
        void DisplayElements(List<DtsElement> elements);
        void InsertElements(List<DtsElement> elements);
        void DisplayPackage(List<DtsAttribute> attributes);
        void InsertElementAttributes(DtsElement element);


    }
}
