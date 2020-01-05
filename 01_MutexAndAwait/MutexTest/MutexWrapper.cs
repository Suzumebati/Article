using System;
using System.Threading;
using System.Threading.Tasks;

namespace MutexTest
{
    public class MutexWrapper : IDisposable
    {

        /// <summary>
        /// 実インスタンス
        /// </summary>
        private Mutex instance;

        /// <summary>
        /// 取得完了イベント
        /// </summary>
        private CountdownEvent waitEndEvent = new CountdownEvent(1);
        
        /// <summary>
        /// 開放イベント
        /// </summary>
        private CountdownEvent releaseEvent = new CountdownEvent(1);

        /// <summary>
        /// 取得結果
        /// </summary>
        private bool waitResult;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="initiallyOwned"></param>
        /// <param name="name"></param>
        public MutexWrapper(bool initiallyOwned, string name)
        {
            instance = new Mutex(initiallyOwned, name);
        }

        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public virtual bool WaitOne()
        {
            return instance.WaitOne();
        }

        // この書き方だとawaitで開放できない
        ///// <summary>
        ///// 非同期取得
        ///// </summary>
        ///// <returns></returns>
        //public virtual Task<bool> WaitOneAsync()
        //{
        //    return Task.Run(() =>
        //    {
        //        return instance.WaitOne();
        //    });
        //}

        /// <summary>
        /// 非同期取得
        /// </summary>
        /// <returns></returns>
        public virtual Task<bool> WaitOneAsync()
        {
            Task.Factory.StartNew(() =>
            {
                MutexControlTask();
            }, TaskCreationOptions.LongRunning);

            return Task.Factory.StartNew(() =>
            {
                // 取得完了まで待受
                waitEndEvent.Wait();
                waitEndEvent.Reset();
                return waitResult;
            },TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Mutexの取得、開放タスク
        /// プロセス間で1多重にするために１スレッドをシグナル処理する。
        /// なお超絶遅いので頻繁にMutexでなにかするのには向いていない。
        /// （プロセス跨がないのならlockステートメントなりセマフォなりで良い）
        /// </summary>
        private void MutexControlTask()
        {
            // Mutex取得開始
            waitResult = instance.WaitOne();
            waitEndEvent.Signal();

            // Mutexの開放まで待機
            releaseEvent.Wait();
            releaseEvent.Dispose();

            // Mutex開放
            instance.ReleaseMutex();
            instance.Dispose();
            instance = null;

        }

        #region "Disposeサポート"

        private bool disposedValue = false;

        //この書き方だとawaitで開放できない
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (instance != null)
        //        {
        //            instance.ReleaseMutex();
        //            instance.Dispose();
        //            instance = null;
        //        }
        //        disposedValue = true;
        //    }
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                releaseEvent.Signal();
                waitEndEvent.Dispose();
                disposedValue = true;
            }
        }

        ~MutexWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
