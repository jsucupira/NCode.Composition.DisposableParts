﻿#region Copyright Preamble
// 
//    Copyright © 2015 NCode Group
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
#endregion

using System;
using System.Threading;

namespace NCode.Composition.DisposableParts.Synchronization
{
	public sealed class Lock : IDisposable
	{
		private int _isDisposed;
		private readonly ReaderWriterLockSlim _lock;
		private static readonly IDisposable EmptyLock = new EmptyLock();

		public Lock(bool isThreadSafe, LockRecursionPolicy recursionPolicy)
		{
			IsThreadSafe = isThreadSafe;
			if (isThreadSafe)
				_lock = new ReaderWriterLockSlim(recursionPolicy);
		}

		~Lock()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0 || !disposing) return;
			_lock?.Dispose();
		}

		public bool IsThreadSafe { get; }

		public IDisposable ReadLock(int millisecondsTimeout = -1)
		{
			return IsThreadSafe ? new ReadLock(_lock, millisecondsTimeout) : EmptyLock;
		}

		public IDisposable WriteLock(int millisecondsTimeout = -1)
		{
			return IsThreadSafe ? new WriteLock(_lock, millisecondsTimeout) : EmptyLock;
		}

	}
}