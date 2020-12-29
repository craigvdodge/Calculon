using System;
using System.Numerics;

namespace Calculon.Types
{
    public static class ComparisonExtensions
    {
        public static bool Equals(this Number lhs, Number rhs)
        {
            return (lhs.Numerator == rhs.Numerator && lhs.Denominator == rhs.Denominator);
        }

        public static bool Equals(this Number lhs, int rhs)
        {
            Number newRhs = new Number(new BigInteger(rhs));
            return Equals(lhs, newRhs);
        }

        public static bool LessThan(this Number lhs, Number rhs)
        {
            BigInteger newDenom = lhs.Denominator.LCM(rhs.Denominator);
            BigInteger newLhsNum = lhs.Numerator * (newDenom / lhs.Denominator);
            BigInteger newRhsNum = rhs.Numerator * (newDenom / rhs.Denominator);
            return newLhsNum < newRhsNum;
        }

        public static bool LessThan(this Number lhs, int rhs)
        {
            Number newRhs = new Number(new BigInteger(rhs));
            return LessThan(lhs, newRhs);
        }

        public static bool GreaterThan(this Number lhs, Number rhs)
        {
            BigInteger newDenom = lhs.Denominator.LCM(rhs.Denominator);
            BigInteger newLhsNum = lhs.Numerator * (newDenom / lhs.Denominator);
            BigInteger newRhsNum = rhs.Numerator * (newDenom / rhs.Denominator);
            return newLhsNum > newRhsNum;
        }

        public static bool GreaterThan(this Number lhs, int rhs)
        {
            Number newRhs = new Number(new BigInteger(rhs));
            return GreaterThan(lhs, newRhs);
        }

    }
}