// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace N
{
    public class C
    {
        int f = 2;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool cross(int k, int y, C c)
        {
            bool b = false;

            for (int i = 0; i < k; ++i)
            {
                // Here "c.f" is invariant, but is evaluated after "i / y"
                // which may throw a different kind of exception, so can't
                // be hoisted without potentially changing the exception kind
                // thrown.
                b = (i / y < i + c.f);
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool swap(int k, int x, int y, C c)
        {
            bool b = false;

            for (int i = 0; i < k; ++i)
            {
                // Sub-expressions "x / y" and "c.f" are both invariant
                // w.r.t. this loop, and can be hoisted.  Since each can
                // raise an exception, and the exceptions have different
                // types, their relative order must be preserved -- the
                // hoisted copy of "x / y" must be evaluated before the
                // hoisted copy of "c.f"
                b = (x / y < i + c.f);
            }

            return b;
        }

        public static int Main(string[] args)
        {
            int errors = 0;

#if NOT_FIXED
            try
            {
                cross(10, 0, null);
                // DivByZero should be raised from 'swap'; normal return
                // is an error.
                errors |= 1;
            }
            catch (DivideByZeroException)
            {
                // This is the correct result -- i / y should be evaluated and
                // raise this exception (before c.f raises nulllref).
            }
            catch
            {
                // Any exception other than DivideByZero is a failure
                errors |= 2;
            }
#endif

            try
            {
                swap(10, 11, 0, null);
                // DivByZero should be raised from 'swap'; normal return
                // is an error.
                errors |= 4;
            }
            catch (DivideByZeroException)
            {
                // This is the correct result -- x / y should be evaluated and
                // raise this exception (before c.f raises nulllref).
            }
            catch
            {
                // Any exception other than DivideByZero is a failure
                errors |= 8;
            }

            return 100 + errors;
        }
    }
}
