namespace AngularTemplate.Core;

public enum GeneralExecStatus
{
	success,
	failure,
	not_found
}

public class Exec<TResult> : Exec<TResult, GeneralExecStatus>
	where TResult : class
{
	public Exec(TResult result, GeneralExecStatus status) : base(result, status)
	{
	}

	public Exec(GeneralExecStatus status) : base(status)
	{
	}

	public Exec() : base(GeneralExecStatus.success)
	{
	}

	public new Exec<TResult> Set(GeneralExecStatus status)
	{
		Status = status;

		return this;
	}

	public new Exec<TResult> Set(TResult? result)
	{
		Result = result;
		Status = result == null ? GeneralExecStatus.failure : GeneralExecStatus.success;

		return this;
	}
}

public class Exec<TResult, TStatus>
	where TResult : class
	where TStatus : Enum
{
	public static Exec<TResult, TStatus> Start(TResult result, TStatus status)
	{
		return new Exec<TResult, TStatus>(result, status);
	}

	public static Exec<TResult, TStatus> Start(TStatus status)
	{
		return new Exec<TResult, TStatus>(status);
	}

	public static Exec<TResult, GeneralExecStatus> StartSuccess()
	{
		return new Exec<TResult, GeneralExecStatus>(GeneralExecStatus.success);
	}

	public static Exec<TResult, GeneralExecStatus> StartFailure()
	{
		return new Exec<TResult, GeneralExecStatus>(GeneralExecStatus.failure);
	}

	public static Exec<TResult, GeneralExecStatus> StartNotFound()
	{
		return new Exec<TResult, GeneralExecStatus>(GeneralExecStatus.not_found);
	}

	public Exec(TResult result, TStatus status)
	{
		Result = result;
		Status = status;
	}

	public Exec(TStatus status)
	{
		Status = status;
	}

	public TResult? Result { get; internal set; }
	public TStatus Status { get; internal set; }

	public Exec<TResult, TStatus> Set(TResult result, TStatus status)
	{
		Result = result;
		Status = status;

		return this;
	}

	public Exec<TResult, TStatus> Set(TStatus status)
	{
		Status = status;

		return this;
	}

	public Exec<TResult, TStatus> Set(TResult result)
	{
		Result = result;

		return this;
	}
}