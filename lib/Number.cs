using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;


namespace Calculon.Types
{
    public class Number
    {
        public Number(BigInteger Num, BigInteger Denom) =>
            (Numerator, Denominator) = (Num, Denom);

        public Number(BigInteger WholeNum) => (Numerator, Denominator) = (WholeNum, 1);
        public BigInteger Numerator { get; set; }
        public BigInteger Denominator { get; set; }
    }
}
