// Copyright © 2003 by Jeffrey Sax
// All rights reserved.
// http://www.extremeoptimization.com/
// Filename: IPCountryLookup.cs + BinaryTrie.cs //[mod]
// First version: April 18, 2003.
// Changes:
//   May 16, 2003. Replaced GetKeyLength
//   May 24, 2003. Extracted binary trie code. (see BinaryTrie.cs)
//   May 24, 2003. Expanded root of trie to 256 nodes.
//	 May 26, 2003. Inlined 'Match' function.
//	 May 26, 2003. Cleaned up interface.
//	 May 27, 2003. Expanded root of trie to user-supplied number of nodes.

using System;
using System.Collections;
using System.IO;
using System.Net;

namespace CrawlWave.Common
{
	/// <summary>
	/// Represents a trie with keys that are binary values of length up to 32.
	/// </summary>
	public class BinaryTrie
	{
		internal BinaryTrieNode[] _roots;	// Roots of the trie
		private Int32 _indexLength = 0;
		private Int32 _count = 0;	// Number of entries in the trie

		#region Public instance constructors
		
		/// <summary>
		/// Constructs a <see cref="BinaryTrie"/> with an index length of 1.
		/// </summary>
		public BinaryTrie()
		{
			_indexLength = 1;
			_roots = new BinaryTrieNode[2];
		}

		/// <summary>
		/// Constructs a <see cref="BinaryTrie"/> with a given index length.
		/// </summary>
		/// <param name="indexLength">The index length.</param>
		public BinaryTrie(Int32 indexLength)
		{
			if ((indexLength < 1) || (indexLength > 18))
				throw new ArgumentOutOfRangeException("indexLength");
			_indexLength = indexLength;
			_roots = new BinaryTrieNode[1 << indexLength];
		}

		#endregion

		#region Protected instance members

		/// <summary>
		/// Gets the collection of root <see cref="BinaryTrieNode"/> objects in this 
		/// <see cref="BinaryTrie"/>.
		/// </summary>
		protected BinaryTrieNode[] Roots
		{
			get { return _roots; }
		}

		/// <summary>
		/// Gets or sets the number of keys in the trie.
		/// </summary>
		protected Int32 CountInternal
		{
			get { return _count; }
			set { _count = value; }
		}

		/// <summary>
		/// Adds a key with the given index to the trie.
		/// </summary>
		/// <param name="index">
		/// The index of the root <see cref="BinaryTrieNode"/> for the given key value.
		/// </param>
		/// <param name="key">An <see cref="Int32"/> key value.</param>
		/// <param name="keyLength">
		/// The length in bits of the significant portion of the key.
		/// </param>
		/// <returns>The <see cref="BinaryTrieNode"/> that was added to the trie.</returns>
		protected BinaryTrieNode AddInternal(Int32 index, Int32 key, Int32 keyLength)
		{
			CountInternal++;
			BinaryTrieNode root = Roots[index];
			if (null == root)
			{
				// Create the new root.
				return _roots[index] = new BinaryTrieNode(key, keyLength);
			}
			else
			{
				// Add the record to the trie.
				return root.AddInternal(key, keyLength);
			}
		}

		/// <summary>
		/// Finds the best internal match for a key.
		/// </summary>
		/// <param name="index">
		/// The index of the root <see cref="BinaryTrieNode"/> for the given key value.
		/// </param>
		/// <param name="key">An <see cref="Int32"/> key value.</param>
		/// <returns>A reference to the <see cref="Object"/> containing the key.</returns>
		protected Object FindBestMatchInternal(Int32 index, Int32 key)
		{
			BinaryTrieNode root = _roots[index];
			if (null == root)
				return null;
			return root.FindBestMatch(key).UserData;
		}

		/// <summary>
		/// Finds the exact internal match for a key.
		/// </summary>
		/// <param name="index">
		/// The index of the root <see cref="BinaryTrieNode"/> for the given key value.
		/// </param>
		/// <param name="key">An <see cref="Int32"/> key value.</param>
		/// <returns>A reference to the <see cref="Object"/> containing the key.</returns>
		protected Object FindExactMatchInternal(Int32 index, Int32 key)
		{
			BinaryTrieNode root = _roots[index];
			if (null == root)
				return null;
			return root.FindExactMatch(key).UserData;
		}

		#endregion

		#region Public instance properties

		/// <summary>
		/// Gets the index length of this <see cref="BinaryTrie"/>.
		/// </summary>
		/// <remarks>
		/// The index length indicates the number of bits that is to be used to preselect
		/// the root nodes.
		/// </remarks>
		public Int32 IndexLength
		{
			get { return _indexLength; }
		}

		/// <summary>
		/// Gets the number of keys in the trie.
		/// </summary>
		public Int32 Count
		{
			get { return _count; }
		}

		#endregion

		#region Public instance methods
		
		/// <summary>
		/// Adds a node to the trie.
		/// </summary>
		/// <param name="key">An <see cref="Int32"/> key value.</param>
		/// <param name="keyLength">
		/// The length in bits of the significant portion of the key.
		/// </param>
		/// <returns>The <see cref="BinaryTrieNode"/> that was added to the trie.</returns>
		public BinaryTrieNode Add(Int32 key, Int32 keyLength)
		{
			Int32 index = (Int32)(key >> (32 - _indexLength));
			return AddInternal(index, key, keyLength);
		}

		/// <summary>
		/// Finds the best match for a given key.
		/// </summary>
		/// <param name="key">An <see cref="Int32"/> key.</param>
		/// <returns>The best match.</returns>
		public Object FindBestMatch(Int32 key)
		{
			Int32 index = (Int32)(key >> (32 - _indexLength));
			return FindBestMatchInternal(index, key);
		}

		#endregion

	}

	/// <summary>
	/// Represents an entry in an <see cref="IPCountryTable"/> table.
	/// </summary>
	public class BinaryTrieNode
	{
		/// <summary>
		/// Used to match nodes
		/// </summary>
		protected static readonly Object EmptyData = new Object();

		private static Int32[] _bit
			= {0x7FFFFFFF, 0x7FFFFFFF,0x40000000,0x20000000,0x10000000,
				  0x8000000,0x4000000,0x2000000,0x1000000,
				  0x800000,0x400000,0x200000,0x100000,
				  0x80000,0x40000,0x20000,0x10000,
				  0x8000,0x4000,0x2000,0x1000,
				  0x800,0x400,0x200,0x100,
				  0x80,0x40,0x20,0x10,
				  0x8,0x4,0x2,0x1,0};

		private Int32 _key;		// Key value
		private Int32 _keyLength;	// Length of the key
		private BinaryTrieNode _zero = null;	// First child
		private BinaryTrieNode _one = null;	// Second child
		private Object _userData;

		#region Public instance properties
		
		/// <summary>
		/// Gets or sets the country code for this entry.
		/// </summary>
		public Object UserData
		{
			get
			{
				if (IsKey)
				{
					return _userData; 
				}
				else
				{
					return null;
				}
			}
			set { _userData = value; }
		}

		/// <summary>
		/// Gets the key for this entry.
		/// </summary>
		public Int32 Key
		{
			get { return _key; }
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value indicating whether the entry is a key.
		/// </summary>
		public Boolean IsKey
		{
			get { return (!Object.ReferenceEquals(_userData, EmptyData)); }
		}

		#endregion

		#region Internal instance members
		
		/// <summary>
		/// Constructs a <see cref="BinaryTrieNode"/> object.
		/// </summary>
		/// <param name="key">The key for the new object.</param>
		/// <param name="keyLength">the length of the key</param>
		internal BinaryTrieNode(Int32 key, Int32 keyLength)
		{
			_key = key;
			_keyLength = keyLength;
			_userData = EmptyData;
		}

		/// <summary>
		/// Adds a record to the trie using the internal representation of an IP address.
		/// </summary>
		internal BinaryTrieNode AddInternal(Int32 key, Int32 keyLength)
		{
			// Find the common key keyLength
			Int32 difference = key ^ _key;
			// We are only interested in matches up to the keyLength...
			Int32 commonKeyLength = Math.Min(_keyLength, keyLength);
			// ...so count down from there.
			while (difference >= _bit[commonKeyLength])
				commonKeyLength--;

			// If the new key length is smaller than the common key length, 
			// or equal but smaller than the current key length,
			// the new key should be the parent of the current node.
			if ((keyLength < commonKeyLength) || ((keyLength == commonKeyLength) && (keyLength < _keyLength)))
			{
				// Make a copy that will be the child node.
				BinaryTrieNode copy = (BinaryTrieNode)this.MemberwiseClone(); // new BinaryTrieNode(this);
				// Fill in the child references based on the first bit after the common key.
				if ((_key & _bit[keyLength+1]) != 0)
				{
					_zero = null;
					_one = copy;
				}
				else
				{
					_zero = copy;
					_one = null;
				}
				_key = key;
				_keyLength = keyLength;
				UserData = EmptyData;
				return this;
			}

			// Do we have a complete match?
			if (commonKeyLength == _keyLength)
			{
				if (keyLength == _keyLength)
					return this;

				// Yes. Add the key as a child.
				if ((key & _bit[_keyLength+1]) == 0)
				{
					// The remainder of the key starts with a zero.
					// Do we have a child in this position?
					if (null == _zero)
					{
						// No. Create one.
						return _zero = new BinaryTrieNode(key, keyLength);
					}
					else
					{
						// Yes. Add this key to the child.
						return _zero.AddInternal(key, keyLength);
					}
				}
				else
				{
					// The remainder of the key starts with a one.
					// Do we have a child in this position?
					if (null == _one)
					{
						// No. Create one.
						return _one = new BinaryTrieNode(key, keyLength);
					}
					else
					{
						// Yes. Add this key to the child.
						return _one.AddInternal(key, keyLength);
					}
				}
			}
			else
			{
				// No. The match is only partial, so split this node.
				// Make a copy that will be the first child node.
				BinaryTrieNode copy = (BinaryTrieNode)this.MemberwiseClone(); // new BinaryTrieNode(this);
				// And create the other child node.
				BinaryTrieNode newEntry = new BinaryTrieNode(key, keyLength);
				// Fill in the child references based on the first
				// bit after the common key.
				if ((_key & _bit[commonKeyLength+1]) != 0)
				{
					_zero = newEntry;
					_one = copy;
				}
				else
				{
					_zero = copy;
					_one = newEntry;
				}
				_keyLength = commonKeyLength;
				return newEntry;
			}
		}
		#endregion

		#region Public instance members

		/// <summary>
		/// Looks up a key value in the trie.
		/// </summary>
		/// <param name="key">The key to look up.</param>
		/// <returns>The exact matching <see cref="BinaryTrieNode"/> in the trie.</returns>
		public BinaryTrieNode FindExactMatch(Int32 key)
		{
			if ((key ^ _key) == 0)
				return this;
			
			// Pick the child to investigate.
			if ((key & _bit[_keyLength+1]) == 0)
			{
				// If the key matches the child's key, pass on the request.
				if (null != _zero)
				{
					if ((key ^ _zero._key) < _bit[_zero._keyLength])
						return _zero.FindExactMatch(key);
				}
			}
			else
			{
				// If the key matches the child's key, pass on the request.
				if (null != _one)
				{
					if ((key ^ _one._key) < _bit[_one._keyLength])
						return _one.FindExactMatch(key);
				}
			}
			// If we got here, neither child was a match, so the current
			// node is the best match.
			return null;
		}

		/// <summary>
		/// Looks up a key value in the trie.
		/// </summary>
		/// <param name="key">The key to look up.</param>
		/// <returns>The best matching <see cref="BinaryTrieNode"/> in the trie.</returns>
		public BinaryTrieNode FindBestMatch(Int32 key)
		{
			// Pick the child to investigate.
			if ((key & _bit[_keyLength+1]) == 0)
			{
				// If the key matches the child's key, pass on the request.
				if (null != _zero)
				{
					if ((key ^ _zero._key) < _bit[_zero._keyLength])
						return _zero.FindBestMatch(key);
				}
			}
			else
			{
				// If the key matches the child's key, pass on the request.
				if (null != _one)
				{
					if ((key ^ _one._key) < _bit[_one._keyLength])
						return _one.FindBestMatch(key);
				}
			}
			// If we got here, neither child was a match, so the current
			// node is the best match.
			return this;
		}

		#endregion
	}
	
	/// <summary>
	/// Represents a trie that can be used to look up the country
	/// corresponding to an IP address.
	/// </summary>
	public class IPCountryTable : BinaryTrie
	{
		private Int32 _extraNodes = 0;
		private Int32 _indexOffset; // Number of bits after index part.

		/// <summary>
		/// Calculates the length of the key in bits.
		/// </summary>
		protected static Int32 GetKeyLength(Int32 length)
		{
			if (length < 0)
				return 1;
			Int32 keyLength = 33;
			while (length != 0)
			{
				length >>= 1;
				keyLength--;
			}
			return keyLength;
		}
		
		/// <summary>
		/// Constructs an <see cref="IPCountryTable"/> object.
		/// </summary>
		public IPCountryTable(Int32 indexLength) : base(indexLength)
		{
			_indexOffset = 32 - indexLength;
		}

		/// <summary>
		/// Loads an IP-country database file into the trie.
		/// </summary>
		/// <param name="filename">
		/// The path and filename of the file that holds the database.
		/// </param>
		/// <param name="calculateKeyLength">
		/// A boolean value that indicates whether the <em>size</em> field in the database
		/// contains the total length (<strong>true</strong>) or the exponent of the length
		/// (<strong>false</strong> of the allocated segment.
		/// </param>
		public void LoadStatisticsFile(String filename, Boolean calculateKeyLength)
		{
			StreamReader reader = new StreamReader(filename);
			try 
			{
				String record;
				while (null != (record = reader.ReadLine()))
				{
					String[] fields = record.Split('|');

					// Skip if not the right number of fields
					if (fields.Length != 7)
						continue;
					// Skip if not an IPv4 record
					if (fields[2] != "ipv4")
						continue;
					// Skip if header or info line
					if (fields[1] == "*")
						continue;

					String ip = fields[3];

					Int32 length = Int32.Parse(fields[4]);
					Int32 keyLength;

					// Convert number of available IP's to key length
					if (calculateKeyLength)	
						keyLength = GetKeyLength(length);
					else
						keyLength = (Int32)length;

					// Interning the country strings saves us a little bit of memory.
					String countryCode = String.Intern(fields[1]);

					String [] parts = ip.Split('.');

					// The first IndexLength bits of the IP address get
					// to be the index into our table of roots.
					Int32 indexBase = ((Int32.Parse(parts[0]) << 8)
						+ Int32.Parse(parts[1]));
					Int32 keyBase = (indexBase << 16)
						+ (Int32.Parse(parts[2]) << 8)
						+ Int32.Parse(parts[3]);
					indexBase >>= (_indexOffset - 16);

					// If the keyLength is less than our IndexLength,
					// the current record spans multiple root nodes.
					Int32 count = (1 << (IndexLength - Math.Min(keyLength, IndexLength)));

					// The key length should be at least the IndexLength.
					keyLength = Math.Max(keyLength, IndexLength);

					for(Int32 index = 0; index < count; index++)
					{
						// keyBase already contains the indexBase part,
						// so just add the shifted index.
						Int32 key = (index << _indexOffset) + keyBase;
						base.AddInternal(indexBase + index, key, keyLength).UserData = countryCode;
					}
					// We want the count to reflect the actual number of 
					// networks, so remove the duplicates from the count.
					_extraNodes += count - 1;
				}
			} 
			finally
			{
				reader.Close();
				GC.Collect();
			}
		}

		/// <summary>
		/// Gets the total number of entries in the trie.
		/// </summary>
		public Int32 NetworkCodeCount
		{
			get { return base.Count - _extraNodes; }
		}

		/// <summary>
		/// Attempts to find the country code corresponding to a given IP address.
		/// </summary>
		/// <param name="address">A <see cref="String"/> value representing the IP Address</param>
		/// <returns>The two letter country code corresponding to the IP address, or 
		/// <strong>"??"</strong> if it was not found.
		/// </returns>
		public String GetCountry(String address)
		{
			String [] parts = address.Split('.');

			// The first IndexLength bits form the key into the array of root nodes.
			Int32 indexBase = ((Int32.Parse(parts[0]) << 8)
				+ Int32.Parse(parts[1]));
			Int32 index = indexBase >> (_indexOffset - 16);

			BinaryTrieNode root = base.Roots[index];
			// If we don't have a root, we don't have a value.
			if (null == root)
				return null;

			// Calculate the full key...
			Int32 key = (indexBase << 16)+(Int32.Parse(parts[2]) << 8)+Int32.Parse(parts[3]);
			// ...and look it up.
			return (String)root.FindBestMatch(key).UserData;
		}
	}
}