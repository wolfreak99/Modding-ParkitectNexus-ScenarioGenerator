using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioGenerator
{
	public class FlagUtil
	{
		public static bool HasFlag(Enum e, Enum flag)
		{
			// Check whether the flag was given
			if (flag == null) {
				throw new ArgumentNullException("flag");
			}

			// Get the type code of the enumeration
			var typeCode = e.GetTypeCode();

			// If the underlying type of the flag is signed
			if (typeCode == TypeCode.SByte || typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64) {
				return (Convert.ToInt64(e) & Convert.ToInt64(flag)) != 0;
			}

			// If the underlying type of the flag is unsigned
			if (typeCode == TypeCode.Byte || typeCode == TypeCode.UInt16 || typeCode == TypeCode.UInt32 || typeCode == TypeCode.UInt64) {
				return (Convert.ToUInt64(e) & Convert.ToUInt64(flag)) != 0;
			}

			// Unsupported flag type
			throw new NotImplementedException(string.Format("The comparison of the type {0} is not implemented.", e.GetType().Name));
		}
	}
}
