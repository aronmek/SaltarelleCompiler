// IEnumerator.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Collections {
	/// <summary>
	/// Don't use this interface, use the generic one instead. When implementing, you need not supply a valid implementation of GetEnumerator.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
    [ScriptNamespace("ss")]
	[Imported(IsRealType = true)]
    public interface IEnumerator {
		/// <summary>
		/// Don't call this method, use the generic version instead.
		/// </summary>
        [NonScriptable]
		[EditorBrowsable(EditorBrowsableState.Never)]
		object Current { get; }

		/// <summary>
		/// Don't call this method, use the generic version instead.
		/// </summary>
        [NonScriptable]
		[EditorBrowsable(EditorBrowsableState.Never)]
        bool MoveNext();

		/// <summary>
		/// Don't call this method, use the generic version instead.
		/// </summary>
        [NonScriptable]
		[EditorBrowsable(EditorBrowsableState.Never)]
        void Reset();
    }
}
