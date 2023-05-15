using System;
using System.Globalization;
using System.Threading;

using Fp.Network.Compression;

using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public sealed class half_tests
	{
		[Test]
		public void all_possible_half_values()
		{
			for(var i = ushort.MinValue; i < ushort.MaxValue; i++)
			{
				var half1 = Half.ToHalf(i);
				var half2 = (Half)(float)half1;

				Assert.IsTrue(half1.Equals(half2));
			}
		}

		/// <summary>
		///     A test for TryParse
		/// </summary>
		[Test]
		public void try_parse_test1()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");

			const string value = "1234,567e-2";
			const float resultExpected = 12.34567f;

			const bool expected = true;
			bool actual = float.TryParse(value, out float result);
			Assert.AreEqual(resultExpected, result);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for TryParse
		/// </summary>
		[Test]
		public void try_parse_test()
		{
			const string value = "777";
			const NumberStyles style = NumberStyles.None;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			var resultExpected = (Half)777f;
			const bool expected = true;
			bool actual = Half.TryParse(value, style, provider, out Half result);
			Assert.AreEqual(resultExpected, result);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToString
		/// </summary>
		[Test]
		public void to_string_test4()
		{
			Half target = Half.Epsilon;
			const string format = "e";
			const string expected = "5.960464e-008";
			var actual = target.ToString(format);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToString
		/// </summary>
		[Test]
		public void to_string_test3()
		{
			var target = (Half)333.333f;
			const string format = "G";
			IFormatProvider formatProvider = CultureInfo.CreateSpecificCulture("cs-CZ");
			const string expected = "333,25";
			var actual = target.ToString(format, formatProvider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToString
		/// </summary>
		[Test]
		public void to_string_test2()
		{
			var target = (Half)0.001f;
			IFormatProvider formatProvider = CultureInfo.CreateSpecificCulture("cs-CZ");
			const string expected = "0,0009994507";
			var actual = target.ToString(formatProvider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToString
		/// </summary>
		[Test]
		public void to_string_test1()
		{
			var target = (Half)10000.00001f;
			const string expected = "10000";
			var actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToHalf
		/// </summary>
		[Test]
		public void to_half_test1()
		{
			byte[] value = { 0x11, 0x22, 0x33, 0x44 };
			const int startIndex = 1;
			var expected = Half.ToHalf(0x3322);
			var actual = Half.ToHalf(value, startIndex);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for ToHalf
		/// </summary>
		[Test]
		public void to_half_test()
		{
			const ushort bits = 0x3322;
			var expected = (Half)0.2229004f;
			var actual = Half.ToHalf(bits);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToUInt64
		/// </summary>
		[Test]
		public void to_u_int64_test()
		{
			IConvertible target = (Half)12345.999f;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const ulong expected = 12344;
			var actual = target.ToUInt64(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToUInt32
		/// </summary>
		[Test]
		public void to_u_int32_test()
		{
			IConvertible target = (Half)9999;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const uint expected = 9992;
			var actual = target.ToUInt32(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToUInt16
		/// </summary>
		[Test]
		public void to_u_int16_test()
		{
			IConvertible target = (Half)33.33;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const ushort expected = 33;
			var actual = target.ToUInt16(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToType
		/// </summary>
		[Test]
		public void to_type_test()
		{
			IConvertible target = (Half)111.111f;
			Type conversionType = typeof(double);
			IFormatProvider provider = CultureInfo.InvariantCulture;
			object expected = 111.0625;
			object actual = target.ToType(conversionType, provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToString
		/// </summary>
		[Test]
		public void to_string_test()
		{
			IConvertible target = (Half)888.888;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const string expected = "888.5";
			var actual = target.ToString(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToSingle
		/// </summary>
		[Test]
		public void to_single_test()
		{
			IConvertible target = (Half)55.77f;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const float expected = 55.75f;
			var actual = target.ToSingle(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToSByte
		/// </summary>
		[Test]
		public void to_s_byte_test()
		{
			IConvertible target = 123.5678f;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const sbyte expected = 124;
			var actual = target.ToSByte(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToInt64
		/// </summary>
		[Test]
		public void to_int64_test()
		{
			IConvertible target = (Half)8562;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const long expected = 8560;
			var actual = target.ToInt64(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToInt32
		/// </summary>
		[Test]
		public void to_int32_test()
		{
			IConvertible target = (Half)555.5;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const int expected = 556;
			var actual = target.ToInt32(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToInt16
		/// </summary>
		[Test]
		public void to_int16_test()
		{
			IConvertible target = (Half)365;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const short expected = 365;
			var actual = target.ToInt16(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToChar
		/// </summary>
		[Test]
		public void to_char_test()
		{
			IConvertible target = (Half)64UL;
			IFormatProvider provider = CultureInfo.InvariantCulture;

			Assert.Throws<InvalidCastException>(
				() =>
				{
					var _ = target.ToChar(provider);
				}
			);
		}

		/// <summary>
		///     A test for System.IConvertible.ToDouble
		/// </summary>
		[Test]
		public void to_double_test()
		{
			IConvertible target = Half.MaxValue;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const double expected = 65504;
			var actual = target.ToDouble(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToDecimal
		/// </summary>
		[Test]
		public void to_decimal_test()
		{
			IConvertible target = (Half)146.33f;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			var expected = new decimal(146.25f);
			var actual = target.ToDecimal(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToDateTime
		/// </summary>
		[Test]
		public void to_date_time_test()
		{
			IConvertible target = (Half)0;
			IFormatProvider provider = CultureInfo.InvariantCulture;

			Assert.Throws<InvalidCastException>(
				() =>
				{
					var _ = target.ToDateTime(provider);
				}
			);
		}

		/// <summary>
		///     A test for System.IConvertible.ToByte
		/// </summary>
		[Test]
		public void to_byte_test()
		{
			IConvertible target = (Half)111;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			byte expected = 111;
			var actual = target.ToByte(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.ToBoolean
		/// </summary>
		[Test]
		public void to_boolean_test()
		{
			IConvertible target = (Half)77;
			IFormatProvider provider = CultureInfo.InvariantCulture;
			const bool expected = true;
			var actual = target.ToBoolean(provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for System.IConvertible.GetTypeCode
		/// </summary>
		[Test]
		public void get_type_code_test1()
		{
			IConvertible target = (Half)33;
			var expected = (TypeCode)255;
			TypeCode actual = target.GetTypeCode();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Subtract
		/// </summary>
		[Test]
		public void subtract_test()
		{
			var half1 = (Half)1.12345f;
			var half2 = (Half)0.01234f;
			var expected = (Half)1.11111f;
			Half actual = Half.Subtract(half1, half2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Sign
		/// </summary>
		[Test]
		public void sign_test()
		{
			Assert.AreEqual(1, Half.Sign((Half)333.5));
			Assert.AreEqual(1, Half.Sign(10));
			Assert.AreEqual(-1, Half.Sign((Half)(-333.5)));
			Assert.AreEqual(-1, Half.Sign(-10));
			Assert.AreEqual(0, Half.Sign(0));
		}

		/// <summary>
		///     A test for Parse
		/// </summary>
		[Test]
		public void parse_test3()
		{
			const string value = "112,456e-1";
			IFormatProvider provider = new CultureInfo("cs-CZ");
			var expected = (Half)11.2456;
			Half actual = Half.Parse(value, provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Parse
		/// </summary>
		[Test]
		public void parse_test2()
		{
			const string value = "55.55";
			var expected = (Half)55.55;
			Half actual = Half.Parse(value);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Parse
		/// </summary>
		[Test]
		public void parse_test1()
		{
			const string value = "-1.063E-02";
			const NumberStyles style = NumberStyles.AllowExponent | NumberStyles.Number;
			IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-US");
			var expected = (Half)(-0.01062775);
			Half actual = Half.Parse(value, style, provider);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Parse
		/// </summary>
		[Test]
		public void parse_test()
		{
			const string value = "-7";
			const NumberStyles style = NumberStyles.Number;
			Half expected = -7;
			Half actual = Half.Parse(value, style);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_UnaryPlus
		/// </summary>
		[Test]
		public void op_UnaryPlusTest()
		{
			Half half = 77;
			Half expected = 77;
			Half actual = +half;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_UnaryNegation
		/// </summary>
		[Test]
		public void op_UnaryNegationTest()
		{
			Half half = 77;
			Half expected = -77;
			Half actual = -half;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Subtraction
		/// </summary>
		[Test]
		public void op_SubtractionTest()
		{
			var half1 = (Half)77.99;
			var half2 = (Half)17.88;
			var expected = (Half)60.0625;
			Half actual = half1 - half2;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Multiply
		/// </summary>
		[Test]
		public void op_MultiplyTest()
		{
			var half1 = (Half)11.1;
			Half half2 = 5;
			var expected = (Half)55.46879;
			Half actual = half1 * half2;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_LessThanOrEqual
		/// </summary>
		[Test]
		public void op_LessThanOrEqualTest()
		{
			{
				Half half1 = 111;
				Half half2 = 120;
				const bool expected = true;
				bool actual = half1 <= half2;
				Assert.AreEqual(expected, actual);
			}
			{
				Half half1 = 111;
				Half half2 = 111;
				const bool expected = true;
				bool actual = half1 <= half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_LessThan
		/// </summary>
		[Test]
		public void op_LessThanTest()
		{
			{
				Half half1 = 111;
				Half half2 = 120;
				const bool expected = true;
				bool actual = half1 <= half2;
				Assert.AreEqual(expected, actual);
			}
			{
				Half half1 = 111;
				Half half2 = 111;
				const bool expected = true;
				bool actual = half1 <= half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_Inequality
		/// </summary>
		[Test]
		public void op_InequalityTest()
		{
			{
				Half half1 = 0;
				Half half2 = 1;
				const bool expected = true;
				bool actual = half1 != half2;
				Assert.AreEqual(expected, actual);
			}
			{
				var half1 = Half.MaxValue;
				var half2 = Half.MaxValue;
				const bool expected = false;
				bool actual = half1 != half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_Increment
		/// </summary>
		[Test]
		public void op_IncrementTest()
		{
			var half = (Half)125.33f;
			var expected = (Half)126.33f;
			Half actual = ++half;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest10()
		{
			var value = (Half)55.55f;
			const float expected = 55.53125f;
			float actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest9()
		{
			const long value = 1295;
			Half expected = 1295;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest8()
		{
			const sbyte value = -15;
			Half expected = -15;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest7()
		{
			Half value = Half.Epsilon;
			const double expected = 5.9604644775390625e-8;
			double actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest6()
		{
			const short value = 15555;
			Half expected = 15552;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest5()
		{
			const byte value = 77;
			Half expected = 77;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest4()
		{
			const int value = 7777;
			Half expected = 7776;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest3()
		{
			const char value = '@';
			Half expected = 64;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest2()
		{
			ushort value = 546;
			Half expected = 546;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest1()
		{
			var value = 123456UL;
			Half expected = Half.PositiveInfinity;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Implicit
		/// </summary>
		[Test]
		public void op_ImplicitTest()
		{
			const uint value = 728;
			Half expected = 728;
			Half actual = value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_GreaterThanOrEqual
		/// </summary>
		[Test]
		public void op_GreaterThanOrEqualTest()
		{
			{
				Half half1 = 111;
				Half half2 = 120;
				const bool expected = false;
				bool actual = half1 >= half2;
				Assert.AreEqual(expected, actual);
			}
			{
				Half half1 = 111;
				Half half2 = 111;
				const bool expected = true;
				bool actual = half1 >= half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_GreaterThan
		/// </summary>
		[Test]
		public void op_GreaterThanTest()
		{
			{
				Half half1 = 111;
				Half half2 = 120;
				const bool expected = false;
				bool actual = half1 > half2;
				Assert.AreEqual(expected, actual);
			}
			{
				Half half1 = 111;
				Half half2 = 111;
				const bool expected = false;
				bool actual = half1 > half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest12()
		{
			Half value = 1245;
			const uint expected = 1245;
			var actual = (uint)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest11()
		{
			Half value = 3333;
			const ushort expected = 3332;
			var actual = (ushort)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest10()
		{
			const float value = 0.1234f;
			var expected = (Half)0.1234f;
			var actual = (Half)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest9()
		{
			Half value = 9777;
			const decimal expected = 9776;
			var actual = (decimal)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest8()
		{
			var value = (Half)5.5;
			const sbyte expected = 5;
			var actual = (sbyte)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest7()
		{
			Half value = 666;
			const ulong expected = 666;
			var actual = (ulong)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest6()
		{
			const double value = -666.66;
			var expected = (Half)(-666.66);
			var actual = (Half)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest5()
		{
			var value = (Half)33.3;
			const short expected = 33;
			var actual = (short)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest4()
		{
			Half value = 12345;
			const long expected = 12344;
			var actual = (long)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest3()
		{
			var value = (Half)15.15;
			const int expected = 15;
			var actual = (int)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest2()
		{
			var value = new decimal(333.1);
			var expected = (Half)333.1;
			var actual = (Half)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest1()
		{
			Half value = -77;
			const byte expected = unchecked((byte)-77);
			var actual = (byte)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Explicit
		/// </summary>
		[Test]
		public void op_ExplicitTest()
		{
			Half value = 64;
			const char expected = '@';
			var actual = (char)value;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Equality
		/// </summary>
		[Test]
		public void op_EqualityTest()
		{
			{
				var half1 = Half.MaxValue;
				var half2 = Half.MaxValue;
				const bool expected = true;
				bool actual = half1 == half2;
				Assert.AreEqual(expected, actual);
			}
			{
				Half half1 = Half.NaN;
				Half half2 = Half.NaN;
				const bool expected = false;
				bool actual = half1 == half2;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for op_Division
		/// </summary>
		[Test]
		public void op_DivisionTest()
		{
			Half half1 = 333;
			Half half2 = 3;
			Half expected = 111;
			Half actual = half1 / half2;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Decrement
		/// </summary>
		[Test]
		public void op_DecrementTest()
		{
			Half half = 1234;
			Half expected = 1233;
			Half actual = --half;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for op_Addition
		/// </summary>
		[Test]
		public void op_AdditionTest()
		{
			var half1 = (Half)1234.5f;
			var half2 = (Half)1234.5f;
			var expected = (Half)2469f;
			Half actual = half1 + half2;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Negate
		/// </summary>
		[Test]
		public void negate_test()
		{
			var half = new Half(658.51);
			var expected = new Half(-658.51);
			Half actual = Half.Negate(half);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Multiply
		/// </summary>
		[Test]
		public void multiply_test()
		{
			Half half1 = 7;
			Half half2 = 12;
			Half expected = 84;
			Half actual = Half.Multiply(half1, half2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Min
		/// </summary>
		[Test]
		public void min_test()
		{
			Half val1 = -155;
			Half val2 = 155;
			Half expected = -155;
			Half actual = Half.Min(val1, val2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Max
		/// </summary>
		[Test]
		public void max_test()
		{
			var val1 = new Half(333);
			var val2 = new Half(332);
			var expected = new Half(333);
			Half actual = Half.Max(val1, val2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for IsPositiveInfinity
		/// </summary>
		[Test]
		public void is_positive_infinity_test()
		{
			{
				Half half = Half.PositiveInfinity;
				const bool expected = true;
				bool actual = Half.IsPositiveInfinity(half);
				Assert.AreEqual(expected, actual);
			}
			{
				var half = (Half)1234.5678f;
				const bool expected = false;
				bool actual = Half.IsPositiveInfinity(half);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for IsNegativeInfinity
		/// </summary>
		[Test]
		public void is_negative_infinity_test()
		{
			{
				Half half = Half.NegativeInfinity;
				const bool expected = true;
				bool actual = Half.IsNegativeInfinity(half);
				Assert.AreEqual(expected, actual);
			}
			{
				var half = (Half)1234.5678f;
				const bool expected = false;
				bool actual = Half.IsNegativeInfinity(half);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for IsNaN
		/// </summary>
		[Test]
		public void is_na_n_test()
		{
			{
				Half half = Half.NaN;
				const bool expected = true;
				bool actual = Half.IsNaN(half);
				Assert.AreEqual(expected, actual);
			}
			{
				var half = (Half)1234.5678f;
				const bool expected = false;
				bool actual = Half.IsNaN(half);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for IsInfinity
		/// </summary>
		[Test]
		public void is_infinity_test()
		{
			{
				Half half = Half.NegativeInfinity;
				const bool expected = true;
				bool actual = Half.IsInfinity(half);
				Assert.AreEqual(expected, actual);
			}
			{
				Half half = Half.PositiveInfinity;
				const bool expected = true;
				bool actual = Half.IsInfinity(half);
				Assert.AreEqual(expected, actual);
			}
			{
				var half = (Half)1234.5678f;
				const bool expected = false;
				bool actual = Half.IsInfinity(half);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for GetTypeCode
		/// </summary>
		[Test]
		public void get_type_code_test()
		{
			var target = new Half();
			const TypeCode expected = (TypeCode)255;
			TypeCode actual = target.GetTypeCode();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for GetHashCode
		/// </summary>
		[Test]
		public void get_hash_code_test()
		{
			Half target = 777;
			const int expected = 25106;
			int actual = target.GetHashCode();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for GetBytes
		/// </summary>
		[Test]
		public void get_bytes_test()
		{
			var value = Half.ToHalf(0x1234);
			byte[] expected = { 0x34, 0x12 };
			byte[] actual = Half.GetBytes(value);
			Assert.AreEqual(expected[0], actual[0]);
			Assert.AreEqual(expected[1], actual[1]);
		}

		/// <summary>
		///     A test for GetBits
		/// </summary>
		[Test]
		public void get_bits_test()
		{
			var value = new Half(555.555);
			const ushort expected = 24663;
			ushort actual = Half.GetBits(value);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Equals
		/// </summary>
		[Test]
		public void equals_test1()
		{
			{
				var target = Half.MinValue;
				var half = Half.MinValue;
				const bool expected = true;
				bool actual = target.Equals(half);
				Assert.AreEqual(expected, actual);
			}
			{
				Half target = 12345;
				Half half = 12345;
				const bool expected = true;
				bool actual = target.Equals(half);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for Equals
		/// </summary>
		[Test]
		public void equals_test()
		{
			{
				var target = new Half();
				object obj = new float();
				const bool expected = false;
				bool actual = target.Equals(obj);
				Assert.AreEqual(expected, actual);
			}
			{
				var target = new Half();
				object obj = (Half)111;
				const bool expected = false;
				bool actual = target.Equals(obj);
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///     A test for Divide
		/// </summary>
		[Test]
		public void divide_test()
		{
			var half1 = (Half)626.046f;
			var half2 = (Half)8790.5f;
			var expected = (Half)0.07122803f;
			Half actual = Half.Divide(half1, half2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for CompareTo
		/// </summary>
		[Test]
		public void compare_to_test1()
		{
			Half target = 1;
			Half half = 2;
			const int expected = -1;
			int actual = target.CompareTo(half);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for CompareTo
		/// </summary>
		[Test]
		public void compare_to_test()
		{
			Half target = 666;
			object obj = (Half)555;
			const int expected = 1;
			int actual = target.CompareTo(obj);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Add
		/// </summary>
		[Test]
		public void add_test()
		{
			var half1 = (Half)33.33f;
			var half2 = (Half)66.66f;
			var expected = (Half)99.99f;
			Half actual = Half.Add(half1, half2);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Abs
		/// </summary>
		[Test]
		public void abs_test()
		{
			Half value = -55;
			Half expected = 55;
			Half actual = Half.Abs(value);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test6()
		{
			const long value = 44;
			var target = new Half(value);
			Assert.AreEqual((long)target, 44);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test5()
		{
			const int value = 789; // TODO: Initialize to an appropriate value
			var target = new Half(value);
			Assert.AreEqual((int)target, 789);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test4()
		{
			const float value = -0.1234f;
			var target = new Half(value);
			Assert.AreEqual(target, (Half)(-0.1233521f));
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test3()
		{
			const double value = 11.11;
			var target = new Half(value);
			Assert.AreEqual((double)target, 11.109375);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test2()
		{
			const ulong value = 99999999;
			var target = new Half(value);
			Assert.AreEqual(target, Half.PositiveInfinity);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test1()
		{
			const uint value = 3330;
			var target = new Half(value);
			Assert.AreEqual((uint)target, (uint)3330);
		}

		/// <summary>
		///     A test for Half Constructor
		/// </summary>
		[Test]
		public void half_constructor_test()
		{
			var value = new decimal(-11.11);
			var target = new Half(value);
			Assert.AreEqual((decimal)target, (decimal)-11.10938);
		}
	}
}