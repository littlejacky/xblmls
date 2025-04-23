using System.Collections.Generic;
using XBLMS.Models;

namespace XBLMS.Dto
{
    public class TmGroup
    {
        public int Id { get; set; }
        public List<int> TmIds { get; set; }
        public double Ratio { get; set; }
    }
}
