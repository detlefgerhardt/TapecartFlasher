using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapecartFlasher
{
	class TapecartInfo
	{
		public string TCrtName { get; set; }

		/// <summary>
		/// Physical flash size in bytes
		/// </summary>
		public int TotalSize { get; set; }

		/// <summary>
		/// Page size in bytes
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Erase block size in pages
		/// </summary>
		public int ErasePages { get; set; }

		public int EraseSize { get { return PageSize * ErasePages; } }

		public int Version { get; set; }
	}
}
