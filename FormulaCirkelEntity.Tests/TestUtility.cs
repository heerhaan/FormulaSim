using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaCirkelEntity.Tests
{
    class TestUtility
    {
    }

    public class StaticRandom : Random
    {
        readonly int _staticValue;

        public StaticRandom(int staticValue)
        {
            _staticValue = staticValue;
        }

        public override int Next()
        {
            return _staticValue;
        }

        public override int Next(int maxValue)
        {
            return Next();
        }

        public override int Next(int minValue, int maxValue)
        {
            return Next();
        }
    }
}
