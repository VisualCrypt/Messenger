﻿using System;

namespace Obsidian.Cryptography.Api.Infrastructure
{
	public class Response
	{
		const string Success = "SUCCESS";
		const string Canceled = "CANCELED";

		string _state;

		public bool IsSuccess
		{
			get
			{
				CheckStateHasBeenSet();
				return _state == Success;
			}
		}

		public bool IsCanceled
		{
			get
			{
				CheckStateHasBeenSet();
				return _state == Canceled;
			}
		}

		public string Error
		{
			get
			{
				CheckStateHasBeenSet();

				if (_state == Success)
					throw new InvalidOperationException(
						"State of Response is '{0}'. Please check IsSuccess before accessing Response.Error unnecessarily."
							.FormatInvariant(_state));

				return _state;
			}
		}


		public void SetSuccess()
		{
			CheckNotSettingStateTwice();
			_state = Success;
		}

		public void SetError(Exception e)
		{
			if(e == null)
				throw new ArgumentNullException("e");
			CheckNotSettingStateTwice();

			if (e is OperationCanceledException)
				_state = Canceled;
			else
			{
				_state = e.Message;
			}
		}

		public void SetError(string errorMessage)
		{
			CheckNotSettingStateTwice();

			if (string.IsNullOrWhiteSpace(errorMessage))
				throw new ArgumentNullException("errorMessage");

			_state = errorMessage;
		}

		void CheckNotSettingStateTwice()
		{
			if (_state != null)
				throw new InvalidOperationException("The state of the response must not be set more than one time.");
		}

		void CheckStateHasBeenSet()
		{
			if (_state == null)
				throw new InvalidOperationException("The state of the response has not been set.");
		}

        public static void CheckSuccessOrThrow(params Response[] responses)
        {
            foreach (var response in responses)
            {
                if(response.IsSuccess)
                    continue;
                throw new Exception(response.Error);
            }
        }
    }

	public sealed class Response<T> : Response
	{
		public T Result { get; set; }
	}

	public sealed class Response<T, T2> : Response
	{
		public T Result { get; set; }
		public T2 Result2 { get; set; }
	}
}