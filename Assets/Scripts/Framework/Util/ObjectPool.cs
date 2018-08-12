using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

namespace Sorumi.Util {
	public class ObjectPool<T>
    {
		const int MAX_CAPACITY = 100;
		public delegate T Func();

		public delegate void Action(T t);

        Stack<T> buffer;
        Func createFunc;
        Action resetFunc;

		int index;

        public ObjectPool(Func createFunc, Action resetFunc, int size = 0, int capacity = MAX_CAPACITY) {
            if (createFunc == null) {
				return;
			}
			this.buffer = new Stack<T>();
            this.createFunc = createFunc;
            this.resetFunc = resetFunc;

            this.Capacity = capacity;

			for (int i = 0; i < size; i ++) {
				PutObject(createFunc());
			}

        }

        public int Capacity { get; private set; }
        public int Count { get { return buffer.Count; } }

        public T GetObject() {
            if (Count <= 0)
                return createFunc();
            else
                return buffer.Pop();
        }

        public void PutObject(T obj) {
            if (Count >= Capacity)
                return;

            if (resetFunc != null)
                resetFunc(obj);

            buffer.Push(obj);
        }
    }
}