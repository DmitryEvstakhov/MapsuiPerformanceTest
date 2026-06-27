using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui;
using Mapsui.Nts;

namespace AvaloniaMapsUiEditingTest.Models
{
    public interface ICustomFeature : IFeature
    {
        public string Name { get; set; }

        new long Id { get; }
    }
}
